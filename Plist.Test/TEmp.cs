using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Moq;

namespace Plist.Test
{
	public class TEmp
	{
		public void One()
		{
			var value = new TestClass2(7, "Joe Richardson", 39, new[] { "DeLorean", "BMW Z8" });
			var mock = new Mock<XmlWriter>();//
			var mockWriter = new Mock<PlistWriter>(mock.Object) { CallBase = true, DefaultValue = DefaultValue.Mock };
			value.Age = null;

			if (value.Age != null) 
				mockWriter.Object.Write(value.Age.Value);
		}
	}
}
