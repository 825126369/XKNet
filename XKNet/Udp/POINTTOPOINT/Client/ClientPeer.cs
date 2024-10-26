﻿using Google.Protobuf;
using System;
using XKNet.Common;
using XKNet.Udp.POINTTOPOINT.Common;

namespace XKNet.Udp.POINTTOPOINT.Client
{
    internal class ClientPeer : UdpClientPeerBase, ClientPeerBase
    {
        internal MsgSendMgr mMsgSendMgr;
        internal MsgReceiveMgr mMsgReceiveMgr;
        internal SocketUdp mSocketMgr;

        internal UdpCheckMgr mUdpCheckPool = null;
        internal UDPLikeTCPMgr mUDPLikeTCPMgr = null;

        private SOCKET_PEER_STATE mSocketPeerState = SOCKET_PEER_STATE.NONE;
        private Action<ClientPeerBase> mListenSocketStateFunc = null;

        public ClientPeer()
        {
            mMsgSendMgr = new MsgSendMgr(this);
            mMsgReceiveMgr = new MsgReceiveMgr(this);
            mSocketMgr = new SocketUdp(this);
            mUdpCheckPool = new UdpCheckMgr(this);
            mUDPLikeTCPMgr = new UDPLikeTCPMgr(this);
        }
        
        public void Update (double elapsed)
		{
            if (elapsed >= 0.3)
            {
                NetLog.LogWarning("NetClient 帧 时间 太长: " + elapsed);
            }

            mMsgReceiveMgr.Update (elapsed);
            mUDPLikeTCPMgr.Update(elapsed);
		}

        public void SetSocketState(SOCKET_PEER_STATE mState)
        {
            if (this.mSocketPeerState != mState)
            {
                this.mSocketPeerState = mState;
                mListenSocketStateFunc?.Invoke(this);
            }
        }

        public SOCKET_PEER_STATE GetSocketState()
		{
			return mSocketPeerState;
		}

        public void Reset()
        {
            mSocketMgr.Reset();
            mMsgReceiveMgr.Reset();
            mUdpCheckPool.Reset();
        }

        public void Release()
        {
            mSocketMgr.Release();
            mMsgReceiveMgr.Release();
            mUdpCheckPool.Release();

            SetSocketState(SOCKET_PEER_STATE.NONE);
            mListenSocketStateFunc = null;
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

        public void SendInnerNetData(UInt16 id, IMessage data = null)
        {
            mMsgSendMgr.SendInnerNetData(id, data);
        }

        public void SendNetData(ushort nPackageId)
        {
            mMsgSendMgr.SendNetData(nPackageId);
        }

        public void SendNetData(ushort nPackageId, IMessage data)
        {
            mMsgSendMgr.SendNetData(nPackageId, data);
        }

        public void SendNetData(ushort nPackageId, byte[] data)
        {
            mMsgSendMgr.SendNetData(nPackageId, data);
        }

        public void SendNetPackage(NetUdpFixedSizePackage mPackage)
        {
            mSocketMgr.SendNetPackage(mPackage);
        }

        public string GetIPAddress()
        {
           return mSocketMgr.GetIPEndPoint().Address.ToString();
        }

        public void SendNetData(NetPackage mNetPackage)
        {
            mMsgSendMgr.SendNetData(mNetPackage);
        }

        public void SendNetData(ushort nPackageId, ReadOnlySpan<byte> buffer)
        {
            mMsgSendMgr.SendNetData(nPackageId, buffer);
        }

        public void addListenClientPeerStateFunc(Action<ClientPeerBase> mFunc)
        {
            mListenSocketStateFunc += mFunc;
        }

        public void removeListenClientPeerStateFunc(Action<ClientPeerBase> mFunc)
        {
            mListenSocketStateFunc -= mFunc;
        }
    }
}
