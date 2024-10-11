﻿using System;
using XKNet.Common;
using XKNet.Udp.POINTTOPOINT.Common;

namespace XKNet.Udp.POINTTOPOINT.Server
{
    public class UdpNetServerMain:ServerBase
	{
		private NetServer mNetServer;
		public UdpNetServerMain()
		{
            mNetServer = new NetServer();
        }

		public void Update(double elapsed)
		{
			if (elapsed >= 0.3)
			{
				NetLog.LogWarning("NetServer 帧 时间 太长: " + elapsed);
			}

            mNetServer.Update (elapsed);
		}

        public void InitNet(string Ip, int nPort)
        {
            mNetServer.InitNet(Ip, nPort);
        }

        public void addNetListenFun(ushort id, Action<ClientPeerBase, NetPackage> func)
        {
            mNetServer.addNetListenFun(id, func);
        }

        public void removeNetListenFun(ushort id, Action<ClientPeerBase, NetPackage> func)
        {
            mNetServer.removeNetListenFun(id, func);
        }
    }

}