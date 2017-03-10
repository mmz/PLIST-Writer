using System;
using System.Collections.Generic;
using System.Threading;

namespace Plist.Writers
{
	internal class TypeComparer : IComparer<Type>
	{
		public int Compare(Type x, Type y)
		{
			if (x == y)
				return 0;
			var xh = x.GetHashCode();
			var yh = x.GetHashCode();
			if (xh < yh)
				return -1;
			if (xh > yh)
				return 1;
			return string.CompareOrdinal(x.FullName, y.FullName);
		}
	}
	internal static class WriterStore
	{
		private static readonly ReaderWriterLockSlim CacheLock = new ReaderWriterLockSlim();
		private static readonly SortedList<Type, TypeWriterBase> InnerCache = new SortedList<Type, TypeWriterBase>(new TypeComparer());

		public static TypeWriterBase GetWriter(Type key, Func<TypeWriterBase> createWriter)
		{
			TypeWriterBase result;
			CacheLock.EnterReadLock();
			try
			{
				if (InnerCache.TryGetValue(key, out result))
					return result;
			}
			finally
			{
				CacheLock.ExitReadLock();
			}

			CacheLock.EnterUpgradeableReadLock();
			try
			{
				if (InnerCache.TryGetValue(key, out result))
					return result;
				else
				{
					CacheLock.EnterWriteLock();
					try
					{
						result = createWriter();
						InnerCache.Add(key, result);
						return result;
					}
					finally
					{
						CacheLock.ExitWriteLock();
					}
				}
			}
			finally
			{
				CacheLock.ExitUpgradeableReadLock();
			}
		}

	}
}