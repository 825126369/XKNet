﻿/************************************Copyright*****************************************
*        ProjectName:AKNet
*        Web:https://github.com/825126369/AKNet
*        Description:AKNet 网络库, 兼容 C#8.0 和 .Net Standard 2.1
*        Author:阿珂
*        CreateTime:2024/10/30 21:55:41
*        Copyright:MIT软件许可证
************************************Copyright*****************************************/
using System;

namespace AKNet.Udp.POINTTOPOINT.Common
{
	internal static class UdpNetCommand
    {
		public const ushort COMMAND_PACKAGECHECK = 1;
		public const ushort COMMAND_HEARTBEAT = 2;
		public const ushort COMMAND_CONNECT = 3;
		public const ushort COMMAND_DISCONNECT = 4;

		public const ushort COMMAND_TESTCHAT = 51;

		public static bool orNeedCheck(ushort id)
		{
			return !orInnerCommand(id);
		}

		public static bool orInnerCommand(ushort id)
		{
			return id >= 1 && id <= 10;
		}
    }
}
