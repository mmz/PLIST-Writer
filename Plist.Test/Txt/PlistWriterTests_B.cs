using System.Diagnostics;
using Xunit;

namespace Plist.Test
{
	public partial class PlistWriterTests
	{
		[Fact]
		public void CheckRecursiveBehaviour()
		{
			var a = new RecursiveClass();// { Age = 1, Name = "One" }
			var b = new RecursiveClass { Parent = a };//Age = 2, Name = "Two",
			a.Parent = b;
			var r = a.ToPlistDocument();
			Trace.Write(r);
		}
	}
}