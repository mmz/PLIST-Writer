using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using Moq;
using Moq.Protected;
using Xunit;

namespace Plist.Test
{
	public partial class PlistWriterFixture
	{

		#region PlistValueWriterAttribute

		public class KeepItRealAttribute : PlistValueWriterAttribute
		{
			public override void WriteValue(PlistWriter writer, object value)
			{
				if (value is string)
				{
					var str = Convert.ToString(value, CultureInfo.InvariantCulture);
					writer.Write($"I'm {str}, yes I'm the real {str.Split(' ').Last()}");
				}
				var val = value as ClassWithValueWriterAttribute;
				if (val != null)
				{
					writer.Write($"I'm {val.FirstName} {val.LastName}, yes I'm the real {val.LastName}");
				}
			}
		}

		[Serializable]
		public class ValueWriterAttributedClass
		{
			[KeepItReal]
			public string StrProp { get; set; }
		}

		[Fact]
		public void PlistValueWriterAttribute_For_Property_Test()
		{
			var value = new ValueWriterAttributedClass { StrProp = "Slim Shady" };
			var mock = new Mock<XmlWriter>();
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };

			mockWriter.Object.Write(value);


			mockWriter.Verify(m => m.WriteKey("StrProp"), Times.Once);
			mockWriter.Verify(m => m.Write("I'm Slim Shady, yes I'm the real Shady"), Times.Once);


		}


		[KeepItReal]
		public class ClassWithValueWriterAttribute
		{
			public string FirstName { get; set; }
			public string LastName { get; set; }
		}
		[Fact]
		public void PlistValueWriterAttribute_For_Class_Test()
		{
			var value = new ClassWithValueWriterAttribute { FirstName = "Slim", LastName = "Shady" };
			var mock = new Mock<XmlWriter>();
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };

			mockWriter.Object.Write(value);

			mockWriter.Verify(m => m.Write("I'm Slim Shady, yes I'm the real Shady"), Times.Once);

		}

		#endregion

		#region PlistSerializableAttribute
		[PlistSerializable]
		public class WithAttribute
		{
			public string StrProp { get; set; }
		}

		[Fact]
		public void Serializing_Class_With_Attributes_Yields()
		{
			var value = new WithAttribute { StrProp = "Slim" };
			var mock = new Mock<XmlWriter>();
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };

			mockWriter.Object.Write(value);

			mockWriter.Verify(w => w.WriteDictionaryStartElement(), Times.Once);
			mockWriter.Verify(w => w.WriteKey("StrProp"), Times.Once);
			mockWriter.Verify(w => w.Write("Slim"), Times.Once);
		}

		public class WithOutAttribute
		{
			public string StrProp { get; set; }
		}

		[Fact]
		public void Serializing_Class_Without_Attributes_Yields_Nothing()
		{
			var value = new WithOutAttribute { StrProp = "Slim" };
			var mock = new Mock<XmlWriter>();
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };

			mockWriter.Object.Write(value);

			mockWriter.Verify(w => w.WriteDictionaryStartElement(), Times.Never);
			mockWriter.Verify(w => w.WriteKey("StrProp"), Times.Never);
		}

		#endregion

		#region PlistIgnoreAttribute
		[Serializable]
		public class PlistIgnorePropAttributedTestClass
		{
			public int IntProp { get; set; }

			[PlistIgnore]
			public int IgnoredProp { get; set; }
		}

		[Fact]
		public void PlistIgnore_Attribute_Hides_Property()
		{
			var value = new PlistIgnorePropAttributedTestClass { IgnoredProp = 2, IntProp = 3 };
			var mock = new Mock<XmlWriter>();
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };

			mockWriter.Object.Write(value);

			mockWriter.Verify(m => m.WriteKey("IntProp"), Times.Once);
			mockWriter.Verify(m => m.Write(3), Times.Once);

			mockWriter.Verify(m => m.WriteKey("IgnoredProp"), Times.Never);
			mockWriter.Verify(m => m.Write(2), Times.Never);
			mockWriter.Protected().Verify("WriteIntegerImpl", Times.Never(), ItExpr.Is<object>(v => (int)v == 2));
		}

		[Serializable]
		[PlistIgnore]
		public class IgnoredClass
		{
			public int PropOne { get; set; }
		}

		[Fact]
		public void PlistIgnore_Attribute_Hides_Class_In_Dictionary()
		{
			var value = new Dictionary<string, object>
							{
								{"First", 1},
								{"Second", new IgnoredClass {PropOne = 11} },
								{"Third", "Three"}
							};
			var mock = new Mock<XmlWriter>();
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };

			mockWriter.Object.Write(value);
			mockWriter.Verify(w => w.WriteKey("First"), Times.Once);
			mockWriter.Verify(w => w.WriteKey("Third"), Times.Once);


			mockWriter.Verify(w => w.WriteKey("Second"), Times.Never);
			mockWriter.Verify(m => m.Write(11), Times.Never);
			mockWriter.Protected().Verify("WriteIntegerImpl", Times.Never(), ItExpr.Is<object>(v => (int)v == 11));
		}

		[Serializable]
		public class ClassWithIgnored
		{
			public int NormalProp1 { get; set; }
			public object GeneralProp { get; set; }
			public IgnoredClass IgnoredProp { get; set; }
		}

		[Fact]
		public void PlistIgnore_Attribute_Hides_Class_In_Property()
		{
			var value = new ClassWithIgnored
			{
				NormalProp1 = 1,
				GeneralProp = new IgnoredClass { PropOne = 2 },
				IgnoredProp = new IgnoredClass { PropOne = 3 }
			};
			var mock = new Mock<XmlWriter>();
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };

			mockWriter.Object.Write(value);
			mockWriter.Verify(w => w.WriteKey("NormalProp1"), Times.Once);


			mockWriter.Verify(w => w.WriteKey("GeneralProp"), Times.Never);
			mockWriter.Verify(w => w.WriteKey("IgnoredProp"), Times.Never);
			mockWriter.Verify(m => m.Write(2), Times.Never);
			mockWriter.Protected().Verify("WriteIntegerImpl", Times.Never(), ItExpr.Is<object>(v => (int)v == 2));
			mockWriter.Verify(m => m.Write(3), Times.Never);
			mockWriter.Protected().Verify("WriteIntegerImpl", Times.Never(), ItExpr.Is<object>(v => (int)v == 3));
		}

		[Fact]
		public void PlistIgnore_Attribute_Hides_Class_In_ObjectArray()
		{
			object[] value = new[]
			{
				(object) 1,
				new IgnoredClass {PropOne = 2},
				new PlistIgnorePropAttributedTestClass {IgnoredProp = 3, IntProp = 4}
			};

			var mock = new Mock<XmlWriter>();
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };

			mockWriter.Object.Write(value);
			mockWriter.Verify(w => w.WriteArrayStartElement(), Times.Once);
			mockWriter.Protected().Verify("WriteIntegerImpl", Times.Once(), ItExpr.Is<object>(v => (int)v == 1));
			mockWriter.Verify(w => w.WriteDictionaryStartElement(), Times.Once);
			mockWriter.Verify(m => m.WriteKey("IntProp"), Times.Once);
			mockWriter.Verify(m => m.Write(4), Times.Once);


			mockWriter.Verify(w => w.WriteKey("PropOne"), Times.Never);
			mockWriter.Verify(m => m.Write(2), Times.Never);
			mockWriter.Protected().Verify("WriteIntegerImpl", Times.Never(), ItExpr.Is<object>(v => (int)v == 2));

		}
		#endregion

		#region PlistKeyAttribute
		[Serializable]
		public class PlistKeyAttributeTestClass
		{
			[PlistKey("KeyFromAttribute")]
			public int IntProp { get; set; }

			[PlistKey("KeyFromAttribute2")]
			public string StrProp { get; set; }
		}
		[Fact]
		public void PlistKey_Changes_Property_Key()
		{
			var value = new PlistKeyAttributeTestClass { IntProp = 3, StrProp = "Howdy!" };
			var mock = new Mock<XmlWriter>();
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };

			mockWriter.Object.Write(value);

			mockWriter.Verify(m => m.WriteKey("KeyFromAttribute"), Times.Once);
			mockWriter.Verify(m => m.WriteKey("IntProp"), Times.Never);
			mockWriter.Verify(m => m.Write(3), Times.Once);

			mockWriter.Verify(m => m.WriteKey("KeyFromAttribute2"), Times.Once);
			mockWriter.Verify(m => m.WriteKey("StrProp"), Times.Never);
			mockWriter.Verify(m => m.Write("Howdy!"), Times.Once);
		}
		#endregion
	}
}