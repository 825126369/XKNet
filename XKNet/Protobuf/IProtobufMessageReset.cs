﻿/************************************Copyright*****************************************
*        ProjectName:XKNet
*        Web:https://github.com/825126369/XKNet
*        Description:XKNet 网络库, 兼容 C#8.0 和 .Net Standard 2.1
*        Author:阿珂
*        CreateTime:2024/10/30 12:14:19
*        Copyright:MIT软件许可证
************************************Copyright*****************************************/
using XKNet.Common;

namespace TestProtocol
{
    public sealed partial class TESTChatMessage : IProtobufResetInterface
    {
        public void Reset()
        {
            NClientId = 0;
            NSortId = 0;
            TalkMsg = string.Empty;
        }
    }
}

namespace TcpProtocol
{
    internal sealed partial class HeartBeat : IProtobufResetInterface
    {
        public void Reset()
        {
            
        }
    }
}

namespace UdpPointtopointProtocols
{
    internal sealed partial class PackageCheckResult : IProtobufResetInterface
    {
        public void Reset()
        {
            MSureOrderIdList.Clear();
        }
    }
}

