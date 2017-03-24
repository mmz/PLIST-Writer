using System;
using System.Globalization;
using System.Xml;
using Moq;
using Plist.Test.Helpers;
using Xunit;

namespace Plist.Test
{
	public partial class PlistWriterFixture
	{
		[Fact]
		public void WriteNullableBooleanTrueTest()
		{
			bool? value = true;
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);


			using (new Sequence())
			{
				mock.SetupStep(w => w.WriteStartElement(null, Plist.TrueValueTag, null));
				mock.SetupStep(w => w.WriteEndElement());

				pWriter.Write(value);

				Sequence.AssertCompleted();
			}
		}

		[Fact]
		public void WriteNullableBooleanFalseTest()
		{
			bool? value = false;
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);


			using (new Sequence())
			{
				mock.SetupStep(w => w.WriteStartElement(null, Plist.FalseValueTag, null));
				mock.SetupStep(w => w.WriteEndElement());

				pWriter.Write(value);

				Sequence.AssertCompleted();
			}
		}

		[Theory]
		[InlineData("234.34")]
		[InlineData("123984709839247098752039.8750")]
		[InlineData("79228162514264337593543950335")]
		[InlineData("-79228162514264337593543950335")]
		[InlineData("0.9999999999999999999999999999")]
		public void WriteNullableDecimalTest(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			decimal? num = decimal.Parse(str, CultureInfo.InvariantCulture);

			using (new Sequence())
			{
				mock.SetupStep(w => w.WriteStartElement(null, Plist.RealValueTag, null));
				mock.SetupStep(w => w.WriteString(str));
				mock.SetupStep(w => w.WriteEndElement());

				pWriter.Write(num);

				Sequence.AssertCompleted();
			}
		}

		[Theory]
		[InlineData("-1.79769313486231E+308")]
		[InlineData("1.79769313486231E+308")]
		public void WriteNullableDoubleTest(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			double? num = double.Parse(str, CultureInfo.InvariantCulture);

			using (new Sequence())
			{
				mock.SetupStep(w => w.WriteStartElement(null, Plist.RealValueTag, null));
				mock.SetupStep(w => w.WriteString(str));
				mock.SetupStep(w => w.WriteEndElement());

				pWriter.Write(num);

				Sequence.AssertCompleted();
			}
		}

		[Theory]
		[InlineData("2147483647")]
		[InlineData("-2147483648")]
		public void WriteNullableInt32Test(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			int? num = Int32.Parse(str, CultureInfo.InvariantCulture);

			using (new Sequence())
			{
				mock.SetupStep(w => w.WriteStartElement(null, Plist.IntValueTag, null));
				mock.SetupStep(w => w.WriteString(str));
				mock.SetupStep(w => w.WriteEndElement());

				pWriter.Write(num);

				Sequence.AssertCompleted();
			}
		}

		[Theory]
		[InlineData("9223372036854775807")]
		[InlineData("-9223372036854775807")]
		public void WriteNullableInt64Test(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			long? num = Int64.Parse(str, CultureInfo.InvariantCulture);

			using (new Sequence())
			{
				mock.SetupStep(w => w.WriteStartElement(null, Plist.IntValueTag, null));
				mock.SetupStep(w => w.WriteString(str));
				mock.SetupStep(w => w.WriteEndElement());

				pWriter.Write(num);

				Sequence.AssertCompleted();
			}
		}

		[Theory]
		[InlineData("3.402823E+38")]
		[InlineData("-3.402823E+38")]
		public void WriteNullableSingleTest(string str)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			Single? num = Single.Parse(str, CultureInfo.InvariantCulture);

			using (new Sequence())
			{
				mock.SetupStep(w => w.WriteStartElement(null, Plist.RealValueTag, null));
				mock.SetupStep(w => w.WriteString(str));
				mock.SetupStep(w => w.WriteEndElement());

				pWriter.Write(num);

				Sequence.AssertCompleted();
			}
		}

		[Theory]
		[InlineData(SimpleColors.Blue)]
		[InlineData(SimpleColors.Green)]
		[InlineData(SimpleColors.Red)]
		public void WriteNullableEnumTest(SimpleColors color)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			SimpleColors? value = color;

			using (new Sequence())
			{
				mock.SetupStep(w => w.WriteStartElement(null, Plist.StringValueTag, null));
				mock.SetupStep(w => w.WriteString(Enum.Format(typeof(SimpleColors), color, "G")));
				mock.SetupStep(w => w.WriteEndElement());

				pWriter.Write(value);

				Sequence.AssertCompleted();
			}
		}

		[Theory]
		[InlineData(MixedColors.Cyan | MixedColors.Magenta)]
		[InlineData(MixedColors.Yellow)]
		[InlineData(MixedColors.Cyan | MixedColors.Magenta | MixedColors.Yellow)]
		public void WriteNullableFlagEnumTest(MixedColors color)
		{
			var mock = new Mock<XmlWriter>(MockBehavior.Strict);
			var pWriter = new PlistWriter(mock.Object);
			MixedColors? value = color;

			using (new Sequence())
			{
				mock.SetupStep(w => w.WriteStartElement(null, Plist.StringValueTag, null));
				mock.SetupStep(w => w.WriteString(Enum.Format(typeof(MixedColors), color, "G")));
				mock.SetupStep(w => w.WriteEndElement());

				pWriter.Write(value);

				Sequence.AssertCompleted();
			}
		}
	}
}