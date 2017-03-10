using System;
using System.Reflection.Emit;
using System.Reflection;
using Plist.Writers;

namespace Plist.Emit
{

	internal class DynamicAssemblyHelper
	{
		#region Non-public members

		private const string ASSEMBLY_NAME = "PlistDynamicAssembly";
		private const string MODULE_NAME = "PlistDynamicMod";
		private static readonly AssemblyName AssemblyName;
		private static readonly AssemblyBuilder AssemblyBuilder;
		private static readonly ModuleBuilder ModuleBuilder;

		static DynamicAssemblyHelper()
		{
			AssemblyName = new AssemblyName(ASSEMBLY_NAME);
			AssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(
				AssemblyName,
				AssemblyBuilderAccess.Run
				);

			ModuleBuilder = AssemblyBuilder.DefineDynamicModule(
				MODULE_NAME
				//,AssemblyName.Name + ".dll",true
				);
		}

		private static string CheckTypeName(string typeName)
		{
			return (typeName.Length >= 1042)
			       	? "type_" + typeName.Substring(0, 900) + Guid.NewGuid().ToString().Replace("-", "")
			       	: typeName;
		}

		internal static TypeBuilder DefineWriterType(string typeName)
		{
			return DefineType(typeName + Guid.NewGuid().ToString().Replace("-", ""), typeof(TypeWriterBase));
		}

		internal static TypeBuilder DefineType(string typeName, Type parent)
		{

			return ModuleBuilder.DefineType(
				CheckTypeName(typeName),
				TypeAttributes.Public,
				parent,
				null
			);
		}
		#endregion
	}
}