using System.IO;
using System.Text;
using System.Xml;

namespace Plist
{
	public static class ObjectExtensions
	{
		public static void WritePlistDocument(this object value, TextWriter writer)
		{
			var settings =
				new XmlWriterSettings
				{
					NewLineChars = "\r\n",
					IndentChars = "\t",
					Indent = true,
					CloseOutput = false,
					NewLineHandling = NewLineHandling.Replace
				};

			using (var xwr = XmlWriter.Create(writer, settings))
			{
				value.WritePlistDocument(xwr);
				xwr.Close();
			}
		}

		public static void WritePlistDocument(this object value, Stream stream)
		{
			var settings =
				new XmlWriterSettings
				{
					NewLineChars = "\r\n",
					IndentChars = "\t",
					Indent = true,
					CloseOutput = false,
					Encoding = Encoding.UTF8,
					NewLineHandling = NewLineHandling.Replace
				};

			using (XmlWriter xwr = XmlWriter.Create(stream, settings))
			{
				value.WritePlistDocument(xwr);
				xwr.Close();
			}
		}
		public static void WritePlistDocument(this object value, XmlWriter writer)
		{
			var wr = new PlistWriter(writer);
			wr.WriteStartDocument();
			wr.Write(value);
			wr.WriteEndDocument();
		}
		public static string ToPlistDocument(this object value)
		{
			//   if (value == null)
			//      throw new ArgumentNullException();
			TextWriter xml = new StringWriterWithEncoding(new StringBuilder(), Encoding.UTF8);
			value.WritePlistDocument(xml);
			return xml.ToString();
		}
	}
}
