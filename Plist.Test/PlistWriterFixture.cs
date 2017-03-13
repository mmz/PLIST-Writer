using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using Moq;
using Moq.Protected;
using Plist.Test.Helpers;
using Xunit;
using Plist;

namespace Plist.Test
{
	public partial class PlistWriterFixture
	{

		[Serializable]
		public class NullableTestClass
		{
			public int? NullableIntProp { get; set; }

			public string[] StringArrayProp { get; set; }

			public int IntProp { get; set; }
		}
		[Fact]
		public void Nullable_Property_HasValue_And_Uses_Write_Method()
		{
			var value = new NullableTestClass {NullableIntProp = 98, IntProp = 2, StringArrayProp = new[] {"One", "Two"}};
			var mock = new Mock<XmlWriter>();//
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };
			//value.Age = null;
			using (new Sequence())
			{
				#region Setup
				mockWriter.SetupStep(w => w.WriteDictionaryStartElement());

				mockWriter.SetupStep(w => w.WriteKey("NullableIntProp"));
				mockWriter.SetupStep(w => w.Write(98));
				mockWriter.SetupStep(w => w.WriteKey("StringArrayProp"));
				mockWriter.SetupStep(w => w.WriteArrayStartElement());
				mockWriter.SetupStep("WriteStringImpl", "One");
				mockWriter.SetupStep("WriteStringImpl", "Two");
				mockWriter.SetupStep(w => w.WriteEndElement());

				mockWriter.SetupStep(w => w.WriteKey("IntProp"));
				mockWriter.SetupStep(w => w.Write(2));

				mockWriter.SetupStep(w => w.WriteEndElement());


				#endregion
				mockWriter.Object.Write(value);
				mockWriter.VerifyAll();
				Assert.True(Sequence.Active.Complete);
			}
		}
		[Fact]
		public void Nullable_Property_Without_Value_Writes_Nothing()
		{
			var value = new NullableTestClass { NullableIntProp = null, IntProp = 2, StringArrayProp = null };
			var mock = new Mock<XmlWriter>();//
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };
			//value.Age = null;
			using (new Sequence())
			{
				#region Setup
				mockWriter.SetupStep(w => w.WriteDictionaryStartElement());
				
				mockWriter.SetupStep(w => w.WriteKey("IntProp"));
				mockWriter.SetupStep(w => w.Write(2));

				mockWriter.SetupStep(w => w.WriteEndElement());


				#endregion
				mockWriter.Object.Write(value);
				mockWriter.VerifyAll();
				Assert.True(Sequence.Active.Complete);
				mockWriter.Verify(w => w.WriteKey(It.IsAny<string>()), Times.Once);
			}
		}

		[Fact]
		public void WriteDictionary()
		{
			var value = new Dictionary<string, object>
							{
								{"Id", 5},
								{"Name", "John Smith"},
								{"Age", 30},
								{"Height", (decimal) 1.75},
								{"Agee",(int?) 30}
							};
			var mock = new Mock<XmlWriter>();//
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };

			using (new Sequence())
			{
				#region Setup
				mockWriter.SetupStep(w => w.WriteDictionaryStartElement());

				mockWriter.SetupStep(w => w.WriteKey("Id"));
				mockWriter.SetupStep("WriteIntegerImpl", ItExpr.Is<object>(v => (int) v == 5));
				mockWriter.SetupStep(w => w.WriteKey("Name"));
				mockWriter.SetupStep("WriteStringImpl", "John Smith");
				mockWriter.SetupStep(w => w.WriteKey("Age"));
				mockWriter.SetupStep("WriteIntegerImpl", ItExpr.Is<object>(v => (int)v == 30));
				mockWriter.SetupStep(w => w.WriteKey("Height"));
				mockWriter.SetupStep("WriteRealImpl", ItExpr.Is<object>(v => (decimal)v == 1.75m)); 
				mockWriter.SetupStep(w => w.WriteKey("Agee"));
				mockWriter.SetupStep("WriteIntegerImpl", ItExpr.Is<object>(v => (int)v == 30));

				mockWriter.SetupStep(w => w.WriteEndElement());


				#endregion
				mockWriter.Object.Write(value);
				mockWriter.VerifyAll();
				//Assert.True(Sequence.Active.Complete);
			}

			mockWriter.Reset();
			mockWriter.Object.Write(value);
			mock.Verify(w => w.WriteStartElement(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
				Times.Exactly(value.Count * 2 + 1));
		}



		[Theory]
		[InlineData("First Element", "Second Element", "Third Element", "Fourth Element", "Fifth Elephant")]
		[InlineData("uno", "dos", "tres", "quatro", "cinquo", "senco", "ses")]
		public void WriteArrayOfSting(params string[] p)
		{
			var array = p.ToArray();
			var mock = new Mock<XmlWriter>();//
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true };

			using (new Sequence())
			{
				mockWriter.SetupStep(w => w.WriteArrayStartElement());

				foreach (var str in p)
					mockWriter.SetupStep("WriteStringImpl", str);
				//mockWriter.SetupStep(w => w.Write(str));

				mockWriter.SetupStep(w => w.WriteEndElement());

				mockWriter.Object.Write(array);
				mockWriter.VerifyAll();
			}

			//mockWriter.Reset();
			//mockWriter.Object.Write(array);
			//mock.Verify(w => w.WriteStartElement(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),
			//	Times.Exactly(p.Length + 1));
		}
	}
}
