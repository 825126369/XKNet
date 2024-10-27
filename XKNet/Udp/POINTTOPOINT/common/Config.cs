﻿using XKNet.Common;

namespace XKNet.Udp.POINTTOPOINT.Common
{
	internal static class Config
	{
		//Udp Package OrderId
		public const ushort nUdpMinOrderId = 1;
		public const ushort nUdpMaxOrderId = ushort.MaxValue;

        public const int nUdpPackageFixedSize = 1024;
		public const int nUdpPackageFixedHeadSize = 10;
		public const int nUdpPackageFixedBodySize = nUdpPackageFixedSize - nUdpPackageFixedHeadSize;
		public const int nUdpCombinePackageFixedSize = 1024 * 8; //合并包是可变的

		public const double fReceiveHeartBeatTimeOut = 3.5;
		public const double fMySendHeartBeatMaxTime = 1.0;

        //Server
        public const int numConnections = 10000;
    }
}
