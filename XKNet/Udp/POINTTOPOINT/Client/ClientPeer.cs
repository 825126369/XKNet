﻿using Google.Protobuf;
using System;
using System.Net;
using XKNet.Common;
using XKNet.Udp.POINTTOPOINT.Common;

namespace XKNet.Udp.POINTTOPOINT.Client
{
    internal class ClientPeer : ClientPeerBase
	{
        internal MsgSendMgr mMsgSendMgr;
        internal MsgReceiveMgr mMsgReceiveMgr;
        internal SocketUdp mSocketMgr;

        internal UdpCheck3Pool mUdpCheckPool = null;
        internal UDPLikeTCPMgr mUDPLikeTCPMgr = null;

        private CLIENT_SOCKET_PEER_STATE mSocketPeerState = CLIENT_SOCKET_PEER_STATE.NONE;
        
        public void Update (double elapsed)
		{
			mMsgReceiveMgr.Update (elapsed);
            mUDPLikeTCPMgr.Update(elapsed);
            mUdpCheckPool.Update (elapsed);
		}

        public void SetSocketState(CLIENT_SOCKET_PEER_STATE mState)
        {
            this.mSocketPeerState = mState;
        }

        public CLIENT_SOCKET_PEER_STATE GetSocketState()
		{
			return mSocketPeerState;
		}

        public void Release()
        {
            mSocketMgr.Release();
            mMsgReceiveMgr.Release();
            mUdpCheckPool.Release();
        }

        public void ConnectServer(string Ip, ushort nPort)
        {
            mSocketMgr.ConnectServer(Ip, nPort);
        }

        public bool DisConnectServer()
        {
            return mSocketMgr.DisConnectServer();
        }

        public void ReConnectServer()
        {
            mSocketMgr.ReConnectServer();
        }

        public void addNetListenFun(ushort nPackageId, Action<ClientPeerBase, NetPackage> fun)
        {
            mMsgReceiveMgr.addNetListenFun(nPackageId, fun);
        }

        public void removeNetListenFun(ushort nPackageId, Action<ClientPeerBase, NetPackage> fun)
        {
            mMsgReceiveMgr.removeNetListenFun(nPackageId, fun);
        }

        public void SendNetData(ushort nPackageId, IMessage data = null)
        {
            mMsgSendMgr.SendNetData(nPackageId, data);
        }

        public void SendLuaNetData(ushort nPackageId, byte[] buffer = null)
        {
            mMsgSendMgr.SendLuaNetData(nPackageId, buffer);
        }

        public EndPoint GetEndPoint()
        {
            throw new NotImplementedException();
        }

        public NetUdpFixedSizePackage GetUdpSystemPackage(UInt16 id, IMessage data = null)
        {
            NetLog.Assert(UdpNetCommand.orNeedCheck(id) == false, "id: " + id);

            NetUdpFixedSizePackage mPackage = ObjectPoolManager.Instance.mUdpFixedSizePackagePool.Pop();
            mPackage.nOrderId = 0;
            mPackage.nGroupCount = 0;
            mPackage.nPackageId = id;
            mPackage.Length = Config.nUdpPackageFixedHeadSize;

            if (data != null)
            {
                byte[] cacheSendBuffer = ObjectPoolManager.Instance.nSendBufferPool.Pop(Config.nUdpCombinePackageFixedSize);
                Span<byte> stream = Protocol3Utility.SerializePackage(data, cacheSendBuffer);
                mPackage.Length += stream.Length;
                for (int i = 0; i < stream.Length; i++)
                {
                    mPackage.buffer[Config.nUdpPackageFixedHeadSize + i] = stream[i];
                }
                ObjectPoolManager.Instance.nSendBufferPool.recycle(cacheSendBuffer);
            }

            NetPackageEncryption.Encryption(mPackage);

            return mPackage;
        }
    }
}
