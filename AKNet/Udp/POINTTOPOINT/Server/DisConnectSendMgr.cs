﻿/************************************Copyright*****************************************
*        ProjectName:AKNet
*        Web:https://github.com/825126369/AKNet
*        Description:AKNet 网络库, 兼容 C#8.0 和 .Net Standard 2.1
*        Author:阿珂
*        CreateTime:2024/10/30 21:55:41
*        Copyright:MIT软件许可证
************************************Copyright*****************************************/
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using AKNet.Common;
using AKNet.Udp.POINTTOPOINT.Common;

namespace AKNet.Udp.POINTTOPOINT.Server
{
    internal class DisConnectSendMgr
    {
        readonly SocketAsyncEventArgs SendArgs = new SocketAsyncEventArgs();
        readonly ConcurrentQueue<NetUdpFixedSizePackage> mSendPackageQueue = new ConcurrentQueue<NetUdpFixedSizePackage>();
        private UdpServer mNetServer = null;
        private bool bSendIOContexUsed = false;

        public DisConnectSendMgr(UdpServer mNetServer)
        {
            this.mNetServer = mNetServer;
            SendArgs.Completed += ProcessSend;
            SendArgs.SetBuffer(new byte[Config.nUdpPackageFixedSize], 0, Config.nUdpPackageFixedSize);
        }

        public void SendInnerNetData(EndPoint removeEndPoint)
        {
            UInt16 id = UdpNetCommand.COMMAND_DISCONNECT;
            NetLog.Assert(UdpNetCommand.orInnerCommand(id));
            NetUdpFixedSizePackage mPackage = mNetServer.GetObjectPoolManager().NetUdpFixedSizePackage_Pop();
            mPackage.nPackageId = id;
            mPackage.Length = Config.nUdpPackageFixedHeadSize;
            mPackage.remoteEndPoint = removeEndPoint;
            NetPackageEncryption.Encryption(mPackage);
            SendNetPackage(mPackage);
        }

        private void ProcessSend(object sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                SendNetPackage2(e);
            }
            else
            {
                bSendIOContexUsed = false;
            }
        }

        private void SendNetPackage(NetUdpFixedSizePackage mPackage)
        {
            MainThreadCheck.Check();
            mSendPackageQueue.Enqueue(mPackage);

            if (!bSendIOContexUsed)
            {
                bSendIOContexUsed = true;
                SendNetPackage2(SendArgs);
            }
        }

        private void SendNetPackage2(SocketAsyncEventArgs e)
        {
            NetUdpFixedSizePackage mPackage = null;
            if (mSendPackageQueue.TryDequeue(out mPackage))
            {
                NetPackageEncryption.Encryption(mPackage);
                Array.Copy(mPackage.buffer, e.Buffer, mPackage.Length);
                e.SetBuffer(0, mPackage.Length);
                e.RemoteEndPoint = mPackage.remoteEndPoint;
                mNetServer.GetObjectPoolManager().NetUdpFixedSizePackage_Recycle(mPackage);
                mNetServer.GetSocketMgr().SendNetPackage(e, ProcessSend);
            }
            else
            {
                bSendIOContexUsed = false;
            }
        }
    }
}