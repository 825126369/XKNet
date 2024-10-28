using TestProtocol;
using XKNet.Common;
using XKNet.Tcp.Client;

namespace TestTcpClient
{
    public class NetHandler
    {
        TcpNetClientMain mNetClient = null;
        const int TcpNetCommand_COMMAND_TESTCHAT = 1000;
        public void Init()
        {
            mNetClient = new TcpNetClientMain();
            mNetClient.addNetListenFun(TcpNetCommand_COMMAND_TESTCHAT, receive_csChat);
            mNetClient.ConnectServer("127.0.0.1", 1002);
        }

        public void Update(double fElapsedTime)
        {
            mNetClient.Update(fElapsedTime);
            SendChatInfo();
        }

        Random mRandom = new Random();
        uint gId = 1;
        private void SendChatInfo()
        {
            TESTChatMessage mData = new TESTChatMessage();
            mData.NSortId = gId++;
            mData.NClientId = 1;
            if (mRandom.Next(2, 3) == 1)
            {
                mData.TalkMsg = "Begins..........End";
            }
            else
            {
                mData.TalkMsg = "Begin。。。。。。。。。。。。............................................" +
                    "...................................................................................." +
                    "...................................................................." +
                    "sdfsfsf.s.fsfsfds.df.s.fwqerqweprijqwperqwerqowheropwheporpwerjpo qjwepowiopeqwoerpowqejoqwejoqwjeo  " +
                     "sdfsfsf.s.fsfsfds.df.s.fwqerqweprijqwperqwerqowheropwheporpwerjpo qjwepowiopeqwoerpowqejoqwejoqwjeo  " +
                    "sdfsfsf.s.fsfsfds.df.s.fwqerqweprijqwperqwerqowheropwheporpwerjpo qjwepowiopeqwoerpowqejoqwejoqwjeo  " +
                    "sdfsfsf.s.fsfsfds.df.s.fwqerqweprijqwperqwerqowheropwheporpwerjpo qjwepowiopeqwoerpowqejoqwejoqwjeo  " +
                    " qweopqwjeop opqweuq opweuo  eqwup   quweopiquowequoewuqowe" +

                    "床前明月光，疑是地上霜。\r\n\r\n举头望明月，低头思故乡。" +
                    "床前明月光，疑是地上霜。\r\n\r\n举头望明月，低头思故乡。" +
                    ".........................................End";
            }
            mNetClient.SendNetData(TcpNetCommand_COMMAND_TESTCHAT, mData);
        }

        private static void receive_csChat(ClientPeerBase clientPeer, NetPackage package)
        {
            TESTChatMessage mSendMsg = Protocol3Utility.getData<TESTChatMessage>(package);
            Console.WriteLine(mSendMsg.NSortId + " " + mSendMsg.TalkMsg);
            IMessagePool<TESTChatMessage>.recycle(mSendMsg);
        }
    }
}