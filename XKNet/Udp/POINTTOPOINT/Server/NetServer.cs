﻿using System;
using XKNet.Common;
using XKNet.Udp.POINTTOPOINT.Common;

namespace XKNet.Udp.POINTTOPOINT.Server
{
    internal class NetServer:ServerBase
	{
        internal PackageManager mPackageManager = null;
        internal ClientPeerManager mClientPeerManager = null;
		internal SocketUdp_Server mSocketMgr;

		public NetServer()
		{
			mClientPeerManager = new ClientPeerManager(this);
			mPackageManager = new PackageManager();
			mSocketMgr = new SocketUdp_Server(this);
		}

		public void Update(double elapsed)
		{
			if (elapsed >= 0.3)
			{
				NetLog.LogWarning("NetServer 帧 时间 太长: " + elapsed);
			}

			mClientPeerManager.Update (elapsed);
		}

		public PackageManager GetPackageManager()
		{
			return mPackageManager;
		}

		public ClientPeerManager GetClientPeerManager ()
		{
			return mClientPeerManager;
		}

        public void InitNet(string Ip, int nPort)
        {
			mSocketMgr.InitNet(Ip, nPort);
        }

        public void addNetListenFun(ushort id, Action<ClientPeerBase, NetPackage> func)
        {
			mPackageManager.addNetListenFun(id, func);
        }

        public void removeNetListenFun(ushort id, Action<ClientPeerBase, NetPackage> func)
        {
           mPackageManager.removeNetListenFun(id, func);
        }
    }

}