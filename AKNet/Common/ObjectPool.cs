﻿/************************************Copyright*****************************************
*        ProjectName:AKNet
*        Web:https://github.com/825126369/AKNet
*        Description:AKNet 网络库, 兼容 C#8.0 和 .Net Standard 2.1
*        Author:阿珂
*        CreateTime:2024/10/30 21:55:40
*        Copyright:MIT软件许可证
************************************Copyright*****************************************/
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AKNet.Common
{
    internal interface IPoolItemInterface
    {
		void Reset();
    }

    //Object 池子
    internal class ObjectPool<T> where T : class, IPoolItemInterface, new()
    {
		readonly Stack<T> mObjectPool = null;

		public ObjectPool(int initCapacity = 0)
		{
			mObjectPool = new Stack<T>(initCapacity);
			for (int i = 0; i < initCapacity; i++)
			{
				mObjectPool.Push(new T());
			}
		}

		public int Count()
		{
			return mObjectPool.Count;
		}

		public T Pop()
		{
			T t = null;

			if (!mObjectPool.TryPop(out t))
			{
				t = new T();
			}

			return t;
		}

		public void recycle(T t)
		{ 
#if DEBUG
		NetLog.Assert(!mObjectPool.Contains(t));
#endif
			t.Reset();
			mObjectPool.Push(t);
		}

		public void release()
		{
			mObjectPool.Clear();
		}
	}

	internal class SafeObjectPool<T> where T : class, IPoolItemInterface, new()
	{
		private readonly ConcurrentStack<T> mObjectPool = new ConcurrentStack<T>();

		public SafeObjectPool(int initCapacity = 0)
		{
			for (int i = 0; i < initCapacity; i++)
			{
				mObjectPool.Push(new T());
			}
		}

		public int Count()
		{
			return mObjectPool.Count;
		}

		public T Pop()
		{
			T t = null;
			if (!mObjectPool.TryPop(out t))
			{
				t = new T();
			}
			return t;
		}

		public void recycle(T t)
		{
#if DEBUG
			NetLog.Assert(t.GetType().Name == typeof(T).Name, $"{t.GetType()} : {typeof(T)} ");
			NetLog.Assert(!mObjectPool.Contains(t), "重复回收！！！");
#endif
			t.Reset();
			mObjectPool.Push(t);
		}

		public void release()
		{
			mObjectPool.Clear();
		}
	}
}