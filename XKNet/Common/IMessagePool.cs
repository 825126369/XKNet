﻿using Google.Protobuf;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace XKNet.Common
{
    internal static class MessageParserPool<T> where T : class, IMessage, IMessage<T>, new()
	{
		static ConcurrentStack<MessageParser<T>> mObjectPool = new ConcurrentStack<MessageParser<T>>();

		public static int Count()
		{
			return mObjectPool.Count;
		}

		public static MessageParser<T> Pop()
		{
			MessageParser<T> t = null;
			if (!mObjectPool.TryPop(out t))
			{
				t = new MessageParser<T>(factory);
			}

			return t;
		}

        private static T factory()
        {
            return IMessagePool<T>.Pop();
        }

        public static void recycle(MessageParser<T> t)
		{
#if DEBUG
            NetLog.Assert(!mObjectPool.Contains(t));
#endif
            mObjectPool.Push(t);
		}

		public static void release()
		{
			
		}
	}

	public static class IMessagePool<T> where T : class, IMessage, IMessage<T>, new()
	{
		static ConcurrentStack<T> mObjectPool = new ConcurrentStack<T>();

		public static int Count()
		{
			return mObjectPool.Count;
		}

		public static T Pop()
		{
			T t = null;
			if (!mObjectPool.TryPop(out t))
			{
				t = new T();
			}

			return t;
		}

#if DEBUG
		//Protobuf内部实现了相等器,所以不能直接通过 == 来比较是否包含 
		private static bool orContain(T t)
		{
			foreach (var v in mObjectPool)
			{
				if (Object.ReferenceEquals(v, t))
				{
					return true;
				}
			}
			return false;
		}
#endif

		public static void recycle(T t)
		{
#if DEBUG
			bool bContain = orContain(t);
			NetLog.Assert(!bContain);
			if (!bContain)
#endif
			{
				mObjectPool.Push(t);
			}
		}

		public static void release()
		{

		}
	}
}
