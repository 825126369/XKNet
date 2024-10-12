﻿using System;
using XKNet.Common;

namespace XKNet.Tcp.Common
{
    /// <summary>
    /// 把数据拿出来
    /// </summary>
    internal static class NetPackageEncryption
	{
        private static byte[] mCheck = new byte[4] { (byte)'$', (byte)'$', (byte)'$', (byte)'$' };
		private static byte[] mCacheSendBuffer = new byte[Config.nMsgPackageBufferMaxLength];
		private static byte[] mCacheReceiveBuffer = new byte[Config.nMsgPackageBufferMaxLength];

		public static bool DeEncryption(CircularBuffer<byte> mReceiveStreamList, NetPackage mPackage)
		{
			if (mReceiveStreamList.Length < Config.nPackageFixedHeadSize)
			{
				return false;
			}

			for (int i = 0; i < 4; i++)
			{
				if (mReceiveStreamList[i] != mCheck[i])
				{
					return false;
				}
			}

			ushort nPackageId = (ushort)(mReceiveStreamList[4] | mReceiveStreamList[5] << 8);
			int nLength = mReceiveStreamList[6] | mReceiveStreamList[7] << 8;
			NetLog.Assert(nLength >= 0);

			if (mReceiveStreamList.Length < nLength + Config.nPackageFixedHeadSize)
			{
				return false;
			}

			mPackage.nPackageId = nPackageId;
			
			if (mCacheReceiveBuffer.Length < nLength)
			{
				byte[] mOldBuffer = mCacheReceiveBuffer;
				int newSize = mOldBuffer.Length * 2;
				while (newSize < nLength)
				{
					newSize *= 2;
				}

				mCacheReceiveBuffer = new byte[newSize];
			}

			mReceiveStreamList.WriteTo(Config.nPackageFixedHeadSize, mCacheReceiveBuffer, 0, nLength);
			mPackage.mMsgBuffer = new ArraySegment<byte>(mCacheReceiveBuffer, 0, nLength);

			return true;
		}

		public static ArraySegment<byte> Encryption(int nPackageId, ReadOnlySpan<byte> mBufferSegment)
		{
			int nSumLength = mBufferSegment.Length + Config.nPackageFixedHeadSize;
			if (mCacheSendBuffer.Length < nSumLength)
			{
				byte[] mOldBuffer = mCacheSendBuffer;
				int newSize = mOldBuffer.Length * 2;
				while (newSize < nSumLength)
				{
					newSize *= 2;
				}

				mCacheSendBuffer = new byte[newSize];
			}

			Array.Copy(mCheck, mCacheSendBuffer, 4);
			mCacheSendBuffer[4] = (byte)nPackageId;
			mCacheSendBuffer[5] = (byte)(nPackageId >> 8);
			mCacheSendBuffer[6] = (byte)mBufferSegment.Length;
			mCacheSendBuffer[7] = (byte)(mBufferSegment.Length >> 8);

			for (int i = 0; i < mBufferSegment.Length; i++)
			{
				mCacheSendBuffer[Config.nPackageFixedHeadSize + i] = mBufferSegment[i];
			}

			return new ArraySegment<byte>(mCacheSendBuffer, 0, nSumLength);
		}

	}
}
