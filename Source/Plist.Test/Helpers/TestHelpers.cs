
using System;
using System.Diagnostics;

namespace Plist.Test
{
	static class TH
	{
		public static Stopwatch Run(this Action action, int loops, Action before, Action after)
		{

			var sw = new Stopwatch();
			for (var i = 0; i < loops; i++)
			{
				before?.Invoke();

				sw.Start();
				action();
				sw.Stop();

				after?.Invoke();
			}
			return sw;
		}

		public static Stopwatch Run(this Action<long> action, long loops, Action<long> before, Action<long> after)
		{

			var sw = new Stopwatch();
			for (long i = 0; i < loops; i++)
			{
				before?.Invoke(i);

				sw.Start();
				action(i);
				sw.Stop();

				after?.Invoke(i);
			}
			return sw;
		}
	}
}