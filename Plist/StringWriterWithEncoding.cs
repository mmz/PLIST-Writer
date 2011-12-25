namespace Plist
{
	using System.IO;
	using System.Text;

	/// <summary>
	/// Custom StringWriter that exposes itself with the encoding specified.
	/// A normal <see cref="System.IO.StringWriter"/> always exposes itself as UTF-16.
	/// </summary>
	internal class StringWriterWithEncoding : StringWriter
	{
		readonly Encoding _encoding;

		public StringWriterWithEncoding(StringBuilder builder, Encoding encoding)
			: base(builder)
		{
			_encoding = encoding;
		}

		public override Encoding Encoding
		{
			get { return _encoding; }
		}
	}
}
