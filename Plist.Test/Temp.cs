using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plist.Test
{
	public class Temp
	{
		public void TestThis(TestClass inpTestClass)
		{
			string a = "";
			if (inpTestClass.Cars == null)
				a = "true";
			else
				a = "false";
			var CC = inpTestClass.Cars;
		}
	}
}
