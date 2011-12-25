using System.Reflection;
namespace Plist
{
	static class PropertyInfoExtension
	{
		public static string GetPlistName(this PropertyInfo propertyInfo)
		{
			object[] attrs = propertyInfo.GetCustomAttributes(typeof (PlistPropertyAttribute), true);
			return attrs.Length == 0 
				? propertyInfo.Name 
				: ((PlistPropertyAttribute)attrs[0]).Name;
		}
	}
}
