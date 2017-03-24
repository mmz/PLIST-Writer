using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using Xunit;
using Xunit.Abstractions;

namespace Plist.Test
{
	class NullStream : Stream
	{
		public override void Flush()
		{
			//Position = 0;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new System.NotImplementedException();
		}

		public override void SetLength(long value)
		{

		}

		public override int Read(byte[] buffer, int offset, int count)
		{

			Random rnd = new Random();
			rnd.NextBytes(buffer);
			return count;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			Position += count;
		}

		public override bool CanRead => false;
		public override bool CanSeek => false;
		public override bool CanWrite => true;
		public override long Length => Position;
		public override long Position { get; set; }
	}
	public class Timing
	{


		private readonly ITestOutputHelper output;

		public Timing(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact(Skip = "no use")]
		public void Test()
		{
			int iterations = 1000000;
			var iobj = (object)iterations;
			var stream = new NullStream();

			string result;
			long overall = 0;
			long totalBytes = 0;
			Stopwatch sw;// = new Stopwatch();

			var writer = XmlWriter.Create(stream, new XmlWriterSettings { ConformanceLevel = ConformanceLevel.Fragment });
			var pwriter = new PlistWriter(writer);
		}
	}
}