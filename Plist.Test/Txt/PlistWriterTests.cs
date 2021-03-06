﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace Plist.Test
{
	public partial class PlistWriterTests
	{

		private readonly ITestOutputHelper output;

		public PlistWriterTests(ITestOutputHelper output)
		{
			this.output = output;
		}

		[Fact]//(Skip = "4")
		public void Timing()
		{
			var stream = new NullStream();


			int iterations = 100000;
			var obj = new TestClass(4, "Piter Miles", 34, new[] { "Ford GT", "69 Camaro" });
			#region huge class
			var obj2 = new AttributesTestClass
			{
				Empty = new TestClass(1, "q", 20, null),
				SColors = SimpleColors.Green,
				MColors = MixedColors.Cyan | MixedColors.Magenta,
				UnusableData = "This should not be seen",
				UnusableDataToo = new[] { "This should not be seen", "And another one" },
				FullName = "John Doe",
				MoreColors = MixedColors.Cyan | MixedColors.Yellow,
				Person = new TestClass(2, "Joe Cox", 39, new[] { "Prius", "SkyLine" }),
				Person2 = new TestClass2(7, "Joe Richardson", 39, new[] { "DeLorean", "BMW Z8" }),
				NInt = 10
				,
				Dict = new Dictionary<string, object>{
					{"Test", new[] {new TestClass(1,"One", 11,null), new TestClass(2,"Two", 12,null)}},
					{"Two", new Dictionary<string, object>
					        	{
					        		{"TestArray", new List<TestClass>(){
													   	new TestClass(21,"Two One", 31,null),
															new TestClass(22,"Two Two", 32,null)
													   }
									}
					        	}
					}
				}
			};
			#endregion

			var arr = new[] { "First Element", "Second Element", "Third Element", "Fourth Element", "Fifth Elephant" };
			var dict = new Dictionary<string, object>
			            	{
			            		{"Id", 5},
			            		{"Name", "John Smith"},
			            		{"Age", 30},
			            		{"Height", (decimal) 1.75}
			            	};

			string result;
			long overall = 0;
			long totalBytes = 0;
			Stopwatch sw;// = new Stopwatch();


			sw = TH.Run(() => obj.WritePlistDocument(stream), iterations, null, () => { totalBytes += stream.Position; stream.Position = 0; });
			output.WriteLine("Class complete, {1} bytes writed in {0}ms.", sw.ElapsedMilliseconds, totalBytes);
			totalBytes = 0;
			overall += sw.ElapsedMilliseconds;

			sw = TH.Run(() => obj2.WritePlistDocument(stream), iterations, null, () => { totalBytes += stream.Position; stream.Position = 0; });
			output.WriteLine("Class2 complete, {1} bytes writed in {0}ms.", sw.ElapsedMilliseconds, totalBytes);
			totalBytes = 0;
			overall += sw.ElapsedMilliseconds;

			sw = TH.Run(() => arr.WritePlistDocument(stream), iterations, null, () => { totalBytes += stream.Position; stream.Position = 0; });
			output.WriteLine("Array complete, {1} bytes writed in {0}ms.", sw.ElapsedMilliseconds, totalBytes);
			totalBytes = 0;
			overall += sw.ElapsedMilliseconds;

			sw = TH.Run(() => dict.WritePlistDocument(stream), iterations, null, () => { totalBytes += stream.Position; stream.Position = 0; });
			output.WriteLine("Dictionary complete, {1} bytes writed in {0}ms.", sw.ElapsedMilliseconds, totalBytes);
			totalBytes = 0;
			overall += sw.ElapsedMilliseconds;

			output.WriteLine("All complete in {0}ms", overall);

			overall = 0;
			//Assert.InRange(sw.ElapsedMilliseconds, 0, 3000);

		}
		[Serializable]
		public class MyClass
		{
			public object Obj { get; set; }
		}
		[Fact]
		public void CreateDocumentFromObject()
		{

			// Empty String
			var value = new MyClass { Obj = new { PropOne = "Value One", PropTwo = "Value Two" } };


			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<string />
</plist>";

			var actual = value.ToPlistDocument();

			//Assert.Equal(expected, actual);
		}
		[Fact]
		public void CreateDocumentFromEmptyString()
		{
			// Empty String
			const string value = "";

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<string />
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Empty String did not return the expected value."
		}


		[Fact]
		public void CreateDocumentFromString()
		{
			// String with Value
			const string value = "Test String";

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<string>Test String</string>
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing String did not return the expected value."
		}


		[Fact]
		public void CreateDocumentFromIntegers()
		{
			object value = (Int16)32767;

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<integer>32767</integer>
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Int16 did not return the expected value.");

			value = 1839472;

			expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<integer>1839472</integer>
</plist>";

			actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Int32 did not return the expected value.");

			value = (Int64)183947209;

			expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<integer>183947209</integer>
</plist>";

			actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Int64 did not return the expected value.");

			value = (int?)1839472;

			expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<integer>1839472</integer>
</plist>";

			actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);
			int? nInt = null;

			expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"" />";

			actual = nInt.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing nullable Int32 did not return the expected value."
		}


		[Fact]
		public void CreateDocumentFromBoolean()
		{
			bool value = true;

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<true />
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Boolean (true) did not return the expected value.");

			value = false;

			expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<false />
</plist>";

			actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Boolean (true) did not return the expected value.");
		}


		[Fact]
		public void CreateDocumentFromSingle()
		{
			const float value = (Single)3.1415;

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<real>3.1415</real>
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Single did not return the expected value.");
		}


		[Fact]
		public void CreateDocumentFromDouble()
		{
			const double value = 3.141592;

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<real>3.141592</real>
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Double did not return the expected value.");
		}


		[Fact(DisplayName = "Parsing Decimal did not return the expected value.")]
		public void CreateDocumentFromDecimal()
		{
			const decimal value = (Decimal)3.14159;

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<real>3.14159</real>
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//;
		}
		[Fact(DisplayName = "Parsing Byte array did not return the expected value.")]
		public void CreateDocumentFromByteArray()
		{
			var data = new byte[8];
			(new Random()).NextBytes(data);

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<data>" + Convert.ToBase64String(data) + @"</data>
</plist>";

			var actual = data.ToPlistDocument();

			Assert.Equal(expected, actual);
		}


		[Fact]
		public void CreateDocumentFromDateTime()
		{
			var value = new DateTime(2009, 01, 07, 19, 32, 05);

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<date>2009-01-07T19:32:05Z</date>
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing DateTime did not return the expected value.");
		}


		[Fact]
		public void CreateDocumentFromArray()
		{
			var value = new[] { "First Element", "Second Element", "Third Element", "Fourth Element", "Fifth Elephant" };

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<array>
		<string>First Element</string>
		<string>Second Element</string>
		<string>Third Element</string>
		<string>Fourth Element</string>
		<string>Fifth Elephant</string>
	</array>
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Array did not return the expected value.");
		}


		[Fact]
		public void CreateDocumentFromEmptyDictionary()
		{
			var value = new Dictionary<string, object>();

			var expected =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<dict />
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Empty Dictionary did not return the expected value.");
		}


		[Fact]
		public void CreateDocumentFromDictionary()
		{
			var value = new Dictionary<string, object>
			            	{
			            		{"Id", 5},
			            		{"Name", "John Smith"},
			            		{"Age", 30},
			            		{"Height", (decimal) 1.75}
			            	};

			var expected =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<dict>
		<key>Id</key>
		<integer>5</integer>
		<key>Name</key>
		<string>John Smith</string>
		<key>Age</key>
		<integer>30</integer>
		<key>Height</key>
		<real>1.75</real>
	</dict>
</plist>";

			var actual = value.ToPlistDocument();
			actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Dictionary did not return the expected value.");
		}


		[Fact]
		public void CreateDocumentFromCustomDictionaryClass()
		{
			var value = new CustomDictionary { { "Name", "John Smith" }, { "Username", "jsmith" }, { "Password", "secret" } };

			var expected =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<dict>
		<key>Name</key>
		<string>John Smith</string>
		<key>Username</key>
		<string>jsmith</string>
		<key>Password</key>
		<string>secret</string>
	</dict>
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Dictionary did not return the expected value.");
		}


		[Fact]
		public void CreateDocumentFromClass()
		{
			var value = new TestClass(2, "Piter Miles", 34, new[] { "Ford GT", "69 Camaro" });
			//var value = new TestClass(2, null, 34, new[] { "Ford GT", "69 Camaro" });

			var expected =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<dict>
		<key>Id</key>
		<integer>2</integer>
		<key>Name</key>
		<string>Piter Miles</string>
		<key>Age</key>
		<integer>34</integer>
		<key>Cars</key>
		<array>
			<string>Ford GT</string>
			<string>69 Camaro</string>
		</array>
	</dict>
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing Class did not return the expected value.");
		}
		[Fact]
		public void CreateDocumentFromCustomFormatedClass()
		{
			// String with Value
			var value = new TestClassWithCustom { Name = "John Doe", City = "Baltimore" };

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<string>John Doe, Baltimore</string>
</plist>";

			var actual = value.ToPlistDocument();
			

			Assert.Equal(expected, actual);//, "Parsing String did not return the expected value.");
		}
		[Fact]
		public void CreateDocumentFromCustomFormatedProperty()
		{
			// String with Value
			var value = new TestClassWithCustomPropWriter { Name = "John Doe", Year = new DateTime(2013, 3, 1) };

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<dict>
		<key>Name</key>
		<string>John Doe</string>
		<key>Year</key>
		<string>2013</string>
	</dict>
</plist>";

			var actual = value.ToPlistDocument();
			
			Assert.Equal(expected, actual);//, "Parsing String did not return the expected value.");
			var val = new ModelItem
								{
									PersonId = new Guid("C48912B2-6F4D-4C80-90D7-D97E238A26AC"),
									Name = "John Smith",
									Age = 40,
									Password = "Secret"
								};
			expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<dict>
		<key>Id</key>
		<string>c48912b2-6f4d-4c80-90d7-d97e238a26ac</string>
		<key>Name</key>
		<string>John Smith</string>
		<key>Age</key>
		<integer>40</integer>
	</dict>
</plist>";

			var output = val.ToPlistDocument();
			Assert.Equal(expected, output);
		}

		[Fact]
		public void CreateDocumentWithIgnoredByAttributeProperty()
		{
			// String with Value
			var value = new TestClassWithIgnorePropAtt { Visible = "Here i am", IgnoreMe = "This should not be seen" };

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<dict>
		<key>Visible</key>
		<string>Here i am</string>
	</dict>
</plist>";

			var actual = value.ToPlistDocument();
			

			Assert.Equal(expected, actual);//, "Parsing String did not return the expected value.");
		}


		[Fact]//(Skip = "hz")
		public void CreateDocumentFromClassWithAttributes()
		{
			var value = new AttributesTestClass
			{
				Empty = new TestClass(1, "q", 20, null),
				SColors = SimpleColors.Green,
				MColors = MixedColors.Cyan | MixedColors.Magenta,
				UnusableData = "This should not be seen",
				UnusableDataToo = new[] { "This should not be seen", "And another one" },
				FullName = "John Doe",
				MoreColors = MixedColors.Cyan | MixedColors.Yellow,
				Person = new TestClass(2, "Joe Cox", 36, new[] { "Prius", "SkyLine" }),
				Person2 = new TestClass2(7, "Joe Richardson", 39, new[] { "DeLorean", "BMW Z8" }),
				NInt = 10
				,
				Dict = new Dictionary<string, object>{
					{"Test", new[] {new TestClass(1,"One", 11,null), new TestClass(2,"Two", 12,null)}},
					{"Two", new Dictionary<string, object>
					        	{
					        		{"TestArray", new List<TestClass>(){
													   	new TestClass(21,"Two One", 31,null),
															new TestClass(22,"Two Two", 32,null)
													   }
									}
					        	}
					}
				}
			};

			var expected =
@"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<dict>
		<key>SColors</key>
		<integer>2</integer>
		<key>MColors</key>
		<integer>257</integer>
		<key>MoreColors</key>
		<string>Cyan, Yellow</string>
		<key>Name</key>
		<string>John Doe</string>
		<key>Person</key>
		<dict>
			<key>Id</key>
			<integer>2</integer>
			<key>Name</key>
			<string>Joe Cox</string>
			<key>Age</key>
			<integer>36</integer>
			<key>Cars</key>
			<array>
				<string>Prius</string>
				<string>SkyLine</string>
			</array>
		</dict>
		<key>Person2</key>
		<dict>
			<key>Id</key>
			<integer>7</integer>
			<key>Name</key>
			<string>Joe Richardson</string>
			<key>Age</key>
			<integer>39</integer>
			<key>Cars</key>
			<array>
				<string>DeLorean</string>
				<string>BMW Z8</string>
			</array>
		</dict>
		<key>NInt</key>
		<integer>10</integer>
		<key>Dict</key>
		<dict>
			<key>Test</key>
			<array>
				<dict>
					<key>Id</key>
					<integer>1</integer>
					<key>Name</key>
					<string>One</string>
					<key>Age</key>
					<integer>11</integer>
				</dict>
				<dict>
					<key>Id</key>
					<integer>2</integer>
					<key>Name</key>
					<string>Two</string>
					<key>Age</key>
					<integer>12</integer>
				</dict>
			</array>
			<key>Two</key>
			<dict>
				<key>TestArray</key>
				<array>
					<dict>
						<key>Id</key>
						<integer>21</integer>
						<key>Name</key>
						<string>Two One</string>
						<key>Age</key>
						<integer>31</integer>
					</dict>
					<dict>
						<key>Id</key>
						<integer>22</integer>
						<key>Name</key>
						<string>Two Two</string>
						<key>Age</key>
						<integer>32</integer>
					</dict>
				</array>
			</dict>
		</dict>
	</dict>
</plist>";

			var actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//, "Parsing Class did not return the expected value.");
		}
		[Serializable]
		public class DSTest
		{
			public string Title { get; set; }
			public DataSet Data { get; set; }
		}
		[Fact]
		public void CreateDocumentFromDataSet()
		{
			var value = new DataSet();

			var dt = new DataTable();
			dt.Columns.Add("Id", typeof(int));
			dt.Columns.Add("Name", typeof(string));
			dt.Columns.Add("Age", typeof(int));
			dt.Columns.Add("DOB", typeof(DateTime));
			dt.Columns.Add("Height", typeof(Single));
			dt.Columns.Add("IsEmployed", typeof(bool));

			DataRow dr = dt.NewRow();
			dr["Id"] = 6;
			dr["Name"] = "John Whizzle";
			dr["Age"] = 27;
			dr["DOB"] = new DateTime(1978, 01, 03);
			dr["Height"] = (float)1.64;
			dr["IsEmployed"] = true;

			dt.Rows.Add(dr);

			value.Tables.Add(dt);

			var expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<array>
		<array>
			<dict>
				<key>Id</key>
				<integer>6</integer>
				<key>Name</key>
				<string>John Whizzle</string>
				<key>Age</key>
				<integer>27</integer>
				<key>DOB</key>
				<date>1978-01-03T00:00:00Z</date>
				<key>Height</key>
				<real>1.64</real>
				<key>IsEmployed</key>
				<true />
			</dict>
		</array>
	</array>
</plist>";

			string actual = null;

			actual = value.ToPlistDocument();

			Assert.Equal(expected, actual);//"Parsing DataSet did not return the expected value.");

			var dstst = new DSTest {Title = "DataSet Title", Data = value};

			actual = dstst.ToPlistDocument();
			expected = @"<?xml version=""1.0"" encoding=""utf-8""?>
<!DOCTYPE plist PUBLIC ""-//Apple//DTD PLIST 1.0//EN"" ""http://www.apple.com/DTDs/PropertyList-1.0.dtd"">
<plist version=""1.0"">
	<dict>
		<key>Title</key>
		<string>DataSet Title</string>
		<key>Data</key>
		<array>
			<array>
				<dict>
					<key>Id</key>
					<integer>6</integer>
					<key>Name</key>
					<string>John Whizzle</string>
					<key>Age</key>
					<integer>27</integer>
					<key>DOB</key>
					<date>1978-01-03T00:00:00Z</date>
					<key>Height</key>
					<real>1.64</real>
					<key>IsEmployed</key>
					<true />
				</dict>
			</array>
		</array>
	</dict>
</plist>";
			Assert.Equal(expected, actual);//"Parsing DataSet did not return the expected value.");


		}
	}
}
