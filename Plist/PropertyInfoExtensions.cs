using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Plist
{
	public static class TypeExtensions
	{
		public static bool IsPlistSerializable(this Type type)
		{
			return typeof(IPlistSerializable).IsAssignableFrom(type);
		}
		public static PlistValueWriterAttribute GetPlistValueWriter(this Type type)
		{
			return
				type.GetCustomAttributes(typeof(PlistValueWriterAttribute), true).Cast<PlistValueWriterAttribute>()
					.FirstOrDefault();
		}

		public static bool PlistIgnore(this Type type)
		{
			if (type.IsSerializable)
				return type.GetCustomAttributes(typeof(PlistIgnoreAttribute), true).Length > 0;
			return type.GetCustomAttributes(typeof(PlistSerializableAttribute), true).Length == 0;
		}
	}

	public static class PropertyInfoExtensions
	{
		public static string GetPlistKey(this PropertyInfo propertyInfo)
		{
			object[] attrs = propertyInfo.GetCustomAttributes(typeof(PlistKeyAttribute), true);
			return attrs.Length == 0
				? propertyInfo.Name
				: ((PlistKeyAttribute)attrs[0]).Name;
		}
		public static bool PlistIgnore(this PropertyInfo propertyInfo)
		{
			return 
				propertyInfo.GetCustomAttributes(typeof(PlistIgnoreAttribute), false).Any()
				|| 
				propertyInfo.PropertyType.GetCustomAttributes(typeof(PlistIgnoreAttribute), true).Any();
		}
		public static bool PlistSerializableType(this PropertyInfo propertyInfo)
		{
			return typeof(IPlistSerializable).IsAssignableFrom(propertyInfo.PropertyType);
		}
		public static PlistValueWriterAttribute GetPlistPropertyWriter(this PropertyInfo propertyInfo)
		{
			return
				propertyInfo.GetCustomAttributes(typeof(PlistValueWriterAttribute), true)
					.Cast<PlistValueWriterAttribute>()
					.FirstOrDefault();
		}

		public static Type GetUnderlyingType(this PropertyInfo propertyInfo)
		{
			return Nullable.GetUnderlyingType(propertyInfo.PropertyType);
		}

		//public static PropertyInfo GetHasValueProp(this PropertyInfo propertyInfo)
		//{
		//	return propertyInfo.PropertyType.GetProperty("HasValue");
		//}

	}
}
