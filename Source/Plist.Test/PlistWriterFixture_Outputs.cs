using System;
using System.Globalization;
using System.Xml;
using Moq;
using Xunit;

namespace Plist.Test
{
	public partial class PlistWriterFixture
	{
		[Fact]
		public void WriteStartDocumentTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartDocument());
			mock.InSequence(sequence).Setup(w => w.WriteDocType(Plist.PlistTag, Plist.DocTypePubid, Plist.DocTypeSysid, null));
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.PlistTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteStartAttribute(null, "version", null));

			mock.InSequence(sequence).Setup(w => w.WriteString(Plist.PlistVersion));

			mock.InSequence(sequence).Setup(w => w.WriteEndAttribute());

			pWriter.WriteStartDocument();
		}

		[Fact]
		public void WriteEndDocumentTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			mock.Setup(w => w.WriteEndDocument());

			pWriter.WriteEndDocument();

			mock.Verify(w => w.WriteEndDocument(), Times.Once);
		}

		[Fact]
		public void WriteArrayStartElementTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			mock.Setup(w => w.WriteStartElement(null, Plist.ArrayValueTag, null));

			pWriter.WriteArrayStartElement();

			mock.Verify(w => w.WriteStartElement(null, Plist.ArrayValueTag, null), Times.Once);
		}

		[Fact]
		public void WriteIntStartElementTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			mock.Setup(w => w.WriteStartElement(null, Plist.IntValueTag, null));

			pWriter.WriteIntegerStartElement();

			mock.Verify(w => w.WriteStartElement(null, Plist.IntValueTag, null), Times.Once);
		}

		[Fact]
		public void WriteRealStartElementTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			mock.Setup(w => w.WriteStartElement(null, Plist.RealValueTag, null));

			pWriter.WriteRealStartElement();

			mock.Verify(w => w.WriteStartElement(null, Plist.RealValueTag, null), Times.Once);
		}

		[Fact]
		public void WriteDateStartElementTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			mock.Setup(w => w.WriteStartElement(null, Plist.DateValueTag, null));

			pWriter.WriteDateStartElement();

			mock.Verify(w => w.WriteStartElement(null, Plist.DateValueTag, null), Times.Once);
		}

		[Fact]
		public void WriteStringStartElementTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			mock.Setup(w => w.WriteStartElement(null, Plist.StringValueTag, null));

			pWriter.WriteStringStartElement();

			mock.Verify(w => w.WriteStartElement(null, Plist.StringValueTag, null), Times.Once);
		}

		[Fact]
		public void WriteEndElementTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			mock.Setup(w => w.WriteEndElement());

			pWriter.WriteEndElement();

			mock.Verify(w => w.WriteEndElement(), Times.Once);
		}

		[Theory]
		[InlineData("name")]
		[InlineData("value")]
		[InlineData("any")]
		public void WriteKey(string name)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.KeyTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteString(name));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.WriteKey(name);
		}

		[Fact]
		public void WriteBooleanTrueTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);


			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.TrueValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(true);
			//writer.XmlWriter.WriteElementString(((obj as bool?) == true ? Plist.TrueValueTag : Plist.FalseValueTag), null);)]
		}

		[Fact]
		public void WriteBooleanFalseTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);


			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.FalseValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(false);
		}

		[Fact]
		public void WriteDateTimeTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			var date = DateTime.Now;

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.DateValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteString(date.ToString(Plist.DateFormat)));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(date);
		}

		[Theory]
		[InlineData("234.34")]
		[InlineData("123984709839247098752039.8750")]
		[InlineData("79228162514264337593543950335")]
		[InlineData("-79228162514264337593543950335")]
		[InlineData("0.9999999999999999999999999999")]
		public void WriteDecimalTest(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			var num = decimal.Parse(str, CultureInfo.InvariantCulture);

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.RealValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteString(str));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(num);
		}

		[Theory]
		[InlineData("-1.79769313486231E+308")]
		[InlineData("1.79769313486231E+308")]
		public void WriteDoubleTest(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			var num = double.Parse(str, CultureInfo.InvariantCulture);

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.RealValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteString(str));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(num);
		}

		[Theory]
		[InlineData("2147483647")]
		[InlineData("-2147483648")]
		public void WriteInt32Test(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			var num = Int32.Parse(str, CultureInfo.InvariantCulture);

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.IntValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteString(str));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(num);
		}

		[Theory]
		[InlineData("9223372036854775807")]
		[InlineData("-9223372036854775807")]
		public void WriteInt64Test(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			var num = Int64.Parse(str, CultureInfo.InvariantCulture);

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.IntValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteString(str));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(num);
		}

		[Theory]
		[InlineData("3.402823E+38")]
		[InlineData("-3.402823E+38")]
		public void WriteSingleTest(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			var num = Single.Parse(str, CultureInfo.InvariantCulture);

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.RealValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteString(str));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(num);
		}

		[Theory]
		[InlineData("Abracadabra!")]
		[InlineData("How much wood would a woodchuck chuk if woodchuck could chuck wood?")]
		public void WriteStringTest(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.StringValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteString(str));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(str);
		}

		[Theory]
		[InlineData("Abracadabra!")]
		[InlineData("How much wood would a woodchuck chuk if woodchuck could chuck wood?")]
		public void WriteRawStringTest(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			mock.Setup(w => w.WriteString(str));			

			pWriter.WriteRawString(str);
			mock.Verify(w => w.WriteString(str), Times.Once);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(100)]
		public void WriteByteArrayTest(int len)
		{
			Random rnd = new Random();
			Byte[] bytes = new Byte[len];
			rnd.NextBytes(bytes);

			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.DataValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteBase64(bytes, 0, len));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(bytes);
		}

		[Theory]
		[InlineData(SimpleColors.Blue)]
		[InlineData(SimpleColors.Green)]
		[InlineData(SimpleColors.Red)]
		public void WriteEnumTest(SimpleColors color)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.StringValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteString(Enum.Format(typeof(SimpleColors), color, "G")));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(color);
		}

		[Theory]
		[InlineData(MixedColors.Cyan | MixedColors.Magenta)]
		[InlineData(MixedColors.Yellow)]
		[InlineData(MixedColors.Cyan | MixedColors.Magenta | MixedColors.Yellow)]
		public void WriteFlagEnumTest(MixedColors color)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.StringValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteString(Enum.Format(typeof(MixedColors), color, "G")));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(color);
		}
	}
}