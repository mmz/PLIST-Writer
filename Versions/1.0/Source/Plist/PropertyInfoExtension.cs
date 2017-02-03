using System.Reflection;
namespace Plist
{
	static class PropertyInfoExtension
	{
		public static string GetPlistKey(this PropertyInfo propertyInfo)
		{
			object[] attrs = propertyInfo.GetCustomAttributes(typeof (PlistKeyAttribute), true);
			return attrs.Length == 0 
				? propertyInfo.Name 
				: ((PlistKeyAttribute)attrs[0]).Name;
		}
	}
}
