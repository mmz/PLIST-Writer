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
		public void WriteBoxedBooleanTrueTest()
		{
			object value = true;
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			
			var sequence = new MockSequence();
			mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.TrueValueTag, null));
			mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			pWriter.Write(value);
		}

		[Fact]
		public void WriteBoxedBooleanFalseTest()
		{
			object value = false;
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			var sequence = new MockSequence();
			//mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.TrueValueTag, null));
			//mock.InSequence(sequence).Setup(w => w.WriteEndElement());

			
				mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.FalseValueTag, null));
				mock.InSequence(sequence).Setup(w => w.WriteEndElement());

				pWriter.Write(value);
		
		}

		[Fact]
		public void WriteBoxedDateTimeTest()
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			var date = DateTime.Now;
			object value = date;

			var sequence = new MockSequence();
			{
				mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.DateValueTag, null));
				mock.InSequence(sequence).Setup(w => w.WriteString(date.ToString(Plist.DateFormat)));
				mock.InSequence(sequence).Setup(w => w.WriteEndElement());

				pWriter.Write(value);
			}
		}

		[Theory]
		[InlineData("234.34")]
		[InlineData("123984709839247098752039.8750")]
		[InlineData("79228162514264337593543950335")]
		[InlineData("-79228162514264337593543950335")]
		[InlineData("0.9999999999999999999999999999")]
		public void WriteBoxedDecimalTest(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			object num = decimal.Parse(str, CultureInfo.InvariantCulture);

			var sequence = new MockSequence();
			{
				mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.RealValueTag, null));
				mock.InSequence(sequence).Setup(w => w.WriteString(str));
				mock.InSequence(sequence).Setup(w => w.WriteEndElement());

				pWriter.Write(num);
			}
		}

		[Theory]
		[InlineData("-1.79769313486231E+308")]
		[InlineData("1.79769313486231E+308")]
		public void WriteBoxedDoubleTest(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			object num = double.Parse(str, CultureInfo.InvariantCulture);

			var sequence = new MockSequence();
			{
				mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.RealValueTag, null));
				mock.InSequence(sequence).Setup(w => w.WriteString(str));
				mock.InSequence(sequence).Setup(w => w.WriteEndElement());

				pWriter.Write(num);
			}
		}

		[Theory]
		[InlineData("2147483647")]
		[InlineData("-2147483648")]
		public void WriteBoxedInt32Test(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			object num = Int32.Parse(str, CultureInfo.InvariantCulture);

			var sequence = new MockSequence();
			{
				mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.IntValueTag, null));
				mock.InSequence(sequence).Setup(w => w.WriteString(str));
				mock.InSequence(sequence).Setup(w => w.WriteEndElement());

				pWriter.Write(num);
			}
		}

		[Theory]
		[InlineData("9223372036854775807")]
		[InlineData("-9223372036854775807")]
		public void WriteBoxedInt64Test(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			object num = Int64.Parse(str, CultureInfo.InvariantCulture);

			var sequence = new MockSequence();
			{
				mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.IntValueTag, null));
				mock.InSequence(sequence).Setup(w => w.WriteString(str));
				mock.InSequence(sequence).Setup(w => w.WriteEndElement());

				pWriter.Write(num);
			}
		}

		[Theory]
		[InlineData("3.402823E+38")]
		[InlineData("-3.402823E+38")]
		public void WriteBoxedSingleTest(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			object num = Single.Parse(str, CultureInfo.InvariantCulture);

			var sequence = new MockSequence();
			{
				mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.RealValueTag, null));
				mock.InSequence(sequence).Setup(w => w.WriteString(str));
				mock.InSequence(sequence).Setup(w => w.WriteEndElement());

				pWriter.Write(num);
			}
		}

		[Theory]
		[InlineData("Abracadabra!")]
		[InlineData("How much wood would a woodchuck chuk if woodchuck could chuck wood?")]
		public void WriteBoxedStringTest(object str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			var sequence = new MockSequence();
			{
				mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.StringValueTag, null));
				mock.InSequence(sequence).Setup(w => w.WriteString((string)str));
				mock.InSequence(sequence).Setup(w => w.WriteEndElement());

				pWriter.Write(str);
			}
		}

		[Theory]
		[InlineData(1)]
		[InlineData(10)]
		[InlineData(100)]
		public void WriteBoxedByteArrayTest(int len)
		{
			Random rnd = new Random();
			Byte[] bytes = new Byte[len];
			rnd.NextBytes(bytes);
			object value = bytes;

			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			var sequence = new MockSequence();
			{
				mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.DataValueTag, null));
				mock.InSequence(sequence).Setup(w => w.WriteBase64(bytes, 0, len));
				mock.InSequence(sequence).Setup(w => w.WriteEndElement());

				pWriter.Write(value);
			}
		}

		[Theory]
		[InlineData(SimpleColors.Blue)]
		[InlineData(SimpleColors.Green)]
		[InlineData(SimpleColors.Red)]
		public void WriteBoxedEnumTest(SimpleColors color)
		{
			object value = color;
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			var sequence = new MockSequence();
			{
				mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.StringValueTag, null));
				mock.InSequence(sequence).Setup(w => w.WriteString(Enum.Format(typeof(SimpleColors), color, "G")));
				mock.InSequence(sequence).Setup(w => w.WriteEndElement());

				pWriter.Write(value);
			}
		}

		[Theory]
		[InlineData(MixedColors.Cyan | MixedColors.Magenta)]
		[InlineData(MixedColors.Yellow)]
		[InlineData(MixedColors.Cyan | MixedColors.Magenta | MixedColors.Yellow)]
		public void WriteBoxedFlagEnumTest(MixedColors color)
		{
			object value = color;
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);

			var sequence = new MockSequence();
			{
				mock.InSequence(sequence).Setup(w => w.WriteStartElement(null, Plist.StringValueTag, null));
				mock.InSequence(sequence).Setup(w => w.WriteString(Enum.Format(typeof(MixedColors), color, "G")));
				mock.InSequence(sequence).Setup(w => w.WriteEndElement());

				pWriter.Write(value);
			}
		}
	}
}