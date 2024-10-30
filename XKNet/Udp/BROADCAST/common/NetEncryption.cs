﻿/************************************Copyright*****************************************
*        ProjectName:XKNet
*        Web:https://github.com/825126369/XKNet
*        Description:XKNet 网络库, 兼容 C#8.0 和 .Net Standard 2.1
*        Author:阿珂
*        CreateTime:2024/10/30 12:14:19
*        Copyright:MIT软件许可证
************************************Copyright*****************************************/
using System;
using XKNet.Common;

namespace XKNet.Udp.BROADCAST.COMMON
{
    /// <summary>
    /// 把数据拿出来
    /// </summary>
    internal static class NetPackageEncryption
	{
		private static byte[] mCheck = new byte[4] { (byte)'A', (byte)'B', (byte)'C', (byte)'D' };
	   
		public static bool DeEncryption (NetUdpFixedSizePackage mPackage)
		{
			if (mPackage.Length < Config.nUdpPackageFixedHeadSize) {
				NetLog.LogError ("mPackage Length： " + mPackage.Length);
				return false;
			}

			for (int i = 0; i < 4; i++) {
				if (mPackage.buffer [i] != mCheck [i]) {
					NetLog.LogError ("22222222222222222222222222");
					return false;
				}
			}
				
			mPackage.nPackageId = BitConverter.ToUInt16 (mPackage.buffer, 4);
			return true;
		}

		public static void Encryption (NetUdpFixedSizePackage mPackage)
		{
			UInt16 nPackageId = mPackage.nPackageId;
			Array.Copy (mCheck, 0, mPackage.buffer, 0, 4);
			byte[] byCom = BitConverter.GetBytes (nPackageId);
			Array.Copy (byCom, 0, mPackage.buffer, 4, byCom.Length);
		}
	}
}
