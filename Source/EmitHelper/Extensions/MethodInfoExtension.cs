using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;
using EmitHelper.Ast.Nodes;

namespace EmitHelper.Extensions
{
	public static class MethodInfoExtension
	{
		public static AstCallMethodVoid CallVoid(this MethodInfo methodInfo, IAstRefOrAddr invocationObject, params IAstStackItem[] arguments)
		{
			return Ast.CallVoid(methodInfo, invocationObject, arguments.ToList());
		}
		public static AstCallMethodVoid CallVoid(this MethodInfo methodInfo, LocalBuilder localBuilder, params IAstStackItem[] arguments)
		{
			return Ast.CallVoid(methodInfo, localBuilder.AstLoadRefOrAddr(), arguments.ToList());
		}
		public static AstCallMethodVoid CallVoid(string methodName, LocalBuilder localBuilder, params IAstStackItem[] arguments)
		{
			var methodInfo = localBuilder.LocalType.GetMethod(methodName, arguments.Select(i => i.itemType).ToArray());
			if (methodInfo == null)
				throw new ArgumentException(String.Format("Method {0}.{1}({2})", localBuilder.LocalType.AssemblyQualifiedName,
					arguments.Select(a => a.itemType.Name).Aggregate((r, i) => r + ", " + i).TrimStart(',').TrimStart(' ')));
			return Ast.CallVoid(methodInfo, localBuilder.AstLoadRefOrAddr(), arguments.ToList());
		}



		public static AstCallMethod Call(this MethodInfo methodInfo, IAstRefOrAddr invocationObject, params IAstStackItem[] arguments)
		{
			return Ast.Call(methodInfo, invocationObject, arguments.ToList());
		}
		public static AstCallMethod Call(this IAstRefOrAddr invocationObject, string methodName, params IAstStackItem[] arguments)
		{
			var methodInfo = invocationObject.itemType.GetMethod(methodName, arguments.Select(i => i.itemType).ToArray());
			if (methodInfo == null)
				throw new ArgumentException(String.Format("Method {0}.{1}({2})", invocationObject.itemType.AssemblyQualifiedName,
					arguments.Select(a => a.itemType.Name).Aggregate((r, i) => r + ", " + i).TrimStart(',').TrimStart(' ')));
			return Ast.Call(methodInfo, invocationObject, arguments.ToList());
		}

	}
}
