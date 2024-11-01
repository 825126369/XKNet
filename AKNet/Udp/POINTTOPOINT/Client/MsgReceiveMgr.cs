﻿/************************************Copyright*****************************************
*        ProjectName:AKNet
*        Web:https://github.com/825126369/AKNet
*        Description:AKNet 网络库, 兼容 C#8.0 和 .Net Standard 2.1
*        Author:阿珂
*        CreateTime:2024/10/30 21:55:40
*        Copyright:MIT软件许可证
************************************Copyright*****************************************/
using System;
using AKNet.Common;
using AKNet.Udp.POINTTOPOINT.Common;

namespace AKNet.Udp.POINTTOPOINT.Client
{
    internal class MsgReceiveMgr
    {
		internal readonly PackageManager mPackageManager = new PackageManager();
        internal ClientPeer mClientPeer = null;

		public MsgReceiveMgr(ClientPeer mClientPeer)
		{
			this.mClientPeer = mClientPeer;
		}

		public void AddLogicHandleQueue(NetPackage mPackage)
		{
            NetPackageExecute(mClientPeer, mPackage);
        }

		public void NetPackageExecute(ClientPeerBase peer, NetPackage mPackage)
		{
			mPackageManager.NetPackageExecute(peer, mPackage);
            mClientPeer.GetObjectPoolManager().Recycle(mPackage);
        }

		public void Update(double elapsed)
		{
			
		}

		public void addNetListenFun(UInt16 id, Action<ClientPeerBase, NetPackage> func)
		{
			mPackageManager.addNetListenFun(id, func);
		}

		public void removeNetListenFun(UInt16 id, Action<ClientPeerBase, NetPackage> func)
		{
			mPackageManager.removeNetListenFun(id, func);
		}

		public void Reset()
		{
			
		}

		public void Release()
		{
			Reset();
		}

	}
}