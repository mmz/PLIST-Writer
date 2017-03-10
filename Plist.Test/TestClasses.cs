using System;
using System.Collections.Generic;

namespace Plist.Test
{

	#region Test classes
	[Serializable]
	class CustomDictionary : Dictionary<string, string> { }

	public enum SimpleColors
	{
		Red = 1,
		Green = 2,
		Blue = 3
	}
	[Flags]
	public enum MixedColors
	{
		Cyan = 0x1,
		Yellow = 0x10,
		Magenta = 0x100
	}

	public class TestPlistWriterAttribute : PlistValueWriterAttribute
	{
		public override void WriteValue(PlistWriter writer, object value)
		{
			Type t = value.GetType();
			if (t.IsEnum)
				writer.WriteInteger(Enum.Format(t, value, "D"));
			else
				writer.Write(value);
		}
	}

	public class CustomClassWriterAttribute : PlistValueWriterAttribute
	{
		public override void WriteValue(PlistWriter writer, object value)
		{
			var t = value as TestClassWithCustom;
			writer.Write(t != null ? String.Format("{0}, {1}", t.Name, t.City) : null);
		}
	}

	public class CustomPropWriterAttribute : PlistValueWriterAttribute
	{
		public override void WriteValue(PlistWriter writer, object value)
		{
			if (value is DateTime)
				writer.WriteString(((DateTime)value).ToString("yyyy"));
			else
				writer.Write(value);

		}
	}

	[CustomClassWriter]
	public class TestClassWithCustom
	{
		public string Name { get; set; }
		public string City { get; set; }
	}
	[Serializable]
	public class TestClassWithCustomPropWriter
	{
		public string Name { get; set; }
		[CustomPropWriter]
		public DateTime Year { get; set; }
	}
	[Serializable]
	public class TestClassWithIgnorePropAtt
	{
		public string Visible { get; set; }
		[PlistIgnore]
		public string IgnoreMe { get; set; }
	}

	[PlistSerializableAttribute]
	public class AttributesTestClass
	{
		[TestPlistWriter]
		public SimpleColors SColors { get; set; }
		[TestPlistWriter]
		public MixedColors MColors { get; set; }

		public MixedColors MoreColors { get; set; }
		[PlistIgnore]
		public TestClass Empty { get; set; }
		[PlistIgnore]
		public string UnusableData { get; set; }
		[PlistIgnore]
		public string[] UnusableDataToo { get; set; }

		[PlistKey("Name")]
		public string FullName { get; set; }

		public TestClass Person { get; set; }

		public TestClass2 Person2 { get; set; }

		public int? NIntEmpty { get; set; }
		public DateTime? NdateEmpty { get; set; }

		public int? NInt { get; set; }

		public Dictionary<string, object> Dict { get; set; }

		[PlistIgnore]
		public byte[] Image { get; set; }


	}

	[Serializable]
	public class TestClass
	{
		public int Id { get; private set; }

		public string Name { get; private set; }

		public int Age { get; private set; }

		public string[] Cars { get; private set; }

		public TestClass(int id, string name, int age, string[] cars)
		{
			Id = id;
			Name = name;
			Age = age;
			Cars = cars;
		}
	}
	[Serializable]
	public class TestClass2
	{
		public int Id { get; private set; }

		public string Name { get; private set; }

		public int? Age { get;  set; }

		public string[] Cars { get; private set; }

		public TestClass2(int id, string name, int age, string[] cars)
		{
			Id = id;
			Name = name;
			Age = age;
			Cars = cars;
		}
	}
	#endregion

	[Serializable]
	public class ModelItem
	{
		[PlistKey("Id")]
		public Guid PersonId { get; set; }

		public string Name { get; set; }

		public int Age { get; set; }

		[PlistIgnore]
		public string Password { get; set; }
	}

	[Serializable]
	public class RecursiveClass
	{
		//public string Name { get; set; }

		//public int Age { get; set; }

		public RecursiveClass Parent { get; set; }
	}
}