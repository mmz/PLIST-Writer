﻿using System;

namespace Plist
{
	/// <summary>
	/// Marks
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
	public class PlistIgnoreAttribute : Attribute
	{ }

	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class PlistSerializableAttribute : Attribute
	{ }

	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public class PlistKeyAttribute : Attribute
	{
		public string Name { get; private set; }
		public PlistKeyAttribute(string name)
		{
			if (string.IsNullOrEmpty(name))
				throw new ArgumentException("name");
			Name = name;
		}
	}
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
	public abstract class PlistValueWriterAttribute : Attribute
	{
		public abstract void WriteValue(PlistWriter writer, object value);
	}
}
