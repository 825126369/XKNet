﻿/************************************Copyright*****************************************
*        ProjectName:AKNet
*        Web:https://github.com/825126369/AKNet
*        Description:AKNet 网络库, 兼容 C#8.0 和 .Net Standard 2.1
*        Author:阿珂
*        CreateTime:2024/10/30 21:55:41
*        Copyright:MIT软件许可证
************************************Copyright*****************************************/
//#define SOCKET_LOCK

using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using AKNet.Common;
using AKNet.Udp.POINTTOPOINT.Common;

namespace AKNet.Udp.POINTTOPOINT.Client
{
    internal class SocketUdp
    {
        private readonly SocketAsyncEventArgs ReceiveArgs;
        private readonly SocketAsyncEventArgs SendArgs;
        private readonly object lock_mSocket_object = new object();
        private readonly ConcurrentQueue<NetUdpFixedSizePackage> mSendPackageQueue = new ConcurrentQueue<NetUdpFixedSizePackage>();

        private Socket mSocket = null;
        private IPEndPoint remoteEndPoint = null;
        private string ip;
        private UInt16 port;
        
        bool bReceiveIOContexUsed = false;
        bool bSendIOContexUsed = false;

        ClientPeer mClientPeer;

        public SocketUdp(ClientPeer mClientPeer)
        {
            this.mClientPeer = mClientPeer;
            mClientPeer.SetSocketState(SOCKET_PEER_STATE.NONE);

            mSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            mSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, 1);

            ReceiveArgs = new SocketAsyncEventArgs();
            ReceiveArgs.SetBuffer(new byte[Config.nUdpPackageFixedSize], 0, Config.nUdpPackageFixedSize);
            ReceiveArgs.Completed += ProcessReceive;

            SendArgs = new SocketAsyncEventArgs();
            SendArgs.SetBuffer(new byte[Config.nUdpPackageFixedSize], 0, Config.nUdpPackageFixedSize);
            SendArgs.Completed += ProcessSend;

            bReceiveIOContexUsed = false;
            bSendIOContexUsed = false;
        }

        public void ConnectServer(string ip, UInt16 nPort)
        {
            this.port = nPort;
            this.ip = ip;
            remoteEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            ReceiveArgs.RemoteEndPoint = remoteEndPoint;
            SendArgs.RemoteEndPoint = remoteEndPoint;
            
            ConnectServer();
            StartReceiveFromAsync();
        }

        public void ConnectServer()
        {
            mClientPeer.mUDPLikeTCPMgr.SendConnect();
        }

        public void ReConnectServer()
        {
            mClientPeer.mUDPLikeTCPMgr.SendConnect();
        }

        public IPEndPoint GetIPEndPoint()
        {
            return remoteEndPoint;
        }

        public bool DisConnectServer()
        {
            var mSocketPeerState = mClientPeer.GetSocketState();
            if (mSocketPeerState == SOCKET_PEER_STATE.CONNECTED || mSocketPeerState == SOCKET_PEER_STATE.CONNECTING)
            {
                mClientPeer.mUDPLikeTCPMgr.SendDisConnect();
                return false;
            }
            else
            {
                return true;
            }
        }

        private void StartReceiveFromAsync()
        {
            ReceiveFromAsync();
        }

        private void ReceiveFromAsync()
        {
            bool bIOSyncCompleted = false;
#if SOCKET_LOCK
            lock (lock_mSocket_object)
#endif
            {
                if (mSocket != null)
                {
                    bIOSyncCompleted = !mSocket.ReceiveFromAsync(ReceiveArgs);
                }
            }
            if (bIOSyncCompleted)
            {
                ProcessReceive(null, ReceiveArgs);
            }
        }

        private void ProcessReceive(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                NetUdpFixedSizePackage mPackage = mClientPeer.GetObjectPoolManager().NetUdpFixedSizePackage_Pop();
                Buffer.BlockCopy(e.Buffer, e.Offset, mPackage.buffer, 0, e.BytesTransferred);
                mPackage.Length = e.BytesTransferred;
                mClientPeer.mUdpPackageMainThreadMgr.MultiThreadingReceiveNetPackage(mPackage);
            }
            ReceiveFromAsync();
        }

        private void ProcessSend(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                SendNetPackage2();
            }
            else
            {
                bSendIOContexUsed = false;
                DisConnectedWithException(e.SocketError);
            }
        }
        
        public void SendNetPackage(NetUdpFixedSizePackage mPackage)
        {
            MainThreadCheck.Check();
            mSendPackageQueue.Enqueue(mPackage);
            if (!bSendIOContexUsed)
            {
                bSendIOContexUsed = true;
                SendNetPackage2();
            }
        }

        private void SendNetPackage2()
        {
            NetUdpFixedSizePackage mPackage = null;
            if (mSendPackageQueue.TryDequeue(out mPackage))
            {
                Array.Copy(mPackage.buffer, SendArgs.Buffer, mPackage.Length);
                SendArgs.SetBuffer(0, mPackage.Length);
                mClientPeer.GetObjectPoolManager().NetUdpFixedSizePackage_Recycle(mPackage);

                bool bIOSyncCompleted = false;
#if SOCKET_LOCK
                lock (lock_mSocket_object)
#endif
                {
                    if (mSocket != null)
                    {
                        bIOSyncCompleted = !mSocket.SendToAsync(SendArgs);
                    }
                    else
                    {
                        bSendIOContexUsed = false;
                    }
                }
                if (bIOSyncCompleted)
                {
                    ProcessSend(null, SendArgs);
                }
            }
            else
            {
                bSendIOContexUsed = false;
            }
        }
        
        public void DisConnectedWithNormal()
        {
            NetLog.Log("客户端 正常 断开服务器 ");
            mClientPeer.SetSocketState(SOCKET_PEER_STATE.DISCONNECTED);
        }

        private void DisConnectedWithException(SocketError e)
        {
            var mSocketPeerState = mClientPeer.GetSocketState();
            if (mSocketPeerState == SOCKET_PEER_STATE.DISCONNECTING)
            {
                mClientPeer.SetSocketState(SOCKET_PEER_STATE.DISCONNECTED);
            }
            else if (mSocketPeerState == SOCKET_PEER_STATE.CONNECTED || mSocketPeerState == SOCKET_PEER_STATE.CONNECTING)
            {
                mClientPeer.SetSocketState(SOCKET_PEER_STATE.RECONNECTING);
            }
        }

        private void CloseSocket()
        {
#if SOCKET_LOCK
            lock (lock_mSocket_object)
#endif
            {
                if (mSocket != null)
                {
                    try
                    {
                        mSocket.Close();
                    }
                    catch (Exception) { }
                    mSocket = null;
                }
            }
        }

        public void Reset()
        {
            NetUdpFixedSizePackage mPackage = null;
            while (mSendPackageQueue.TryDequeue(out mPackage))
            {
                mClientPeer.GetObjectPoolManager().NetUdpFixedSizePackage_Recycle(mPackage);
            }
        }

        public void Release()
        {
            DisConnectServer();
            CloseSocket();
            NetLog.Log("--------------- Client Release ----------------");
        }
    }
}









