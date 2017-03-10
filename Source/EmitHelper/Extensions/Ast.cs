using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EmitHelper.Ast.Interfaces;
using EmitHelper.Ast.Nodes;

namespace EmitHelper.Extensions
{
	public static class Ast
	{

		public static AstCallMethodVoid CallVoid(string methodName, IAstRefOrAddr invocationObject, params IAstStackItem[] arguments)
		{
			var methodInfo = invocationObject.itemType.GetMethod(methodName, arguments.Select(i => i.itemType).ToArray());
			return CallVoid(methodInfo, invocationObject, arguments.ToList());
		}
		public static AstCallMethodVoid CallVoid(MethodInfo methodInfo, IAstRefOrAddr invocationObject, params IAstStackItem[] arguments)
		{
			return CallVoid(methodInfo, invocationObject, arguments.ToList());
		}
		public static AstCallMethodVoid CallVoid(
			MethodInfo methodInfo,
			IAstRefOrAddr invocationObject,
			List<IAstStackItem> arguments)
		{
			return new AstCallMethodVoid(methodInfo, invocationObject, arguments);
		}

		public static AstCallMethod Call(
			MethodInfo methodInfo,
			IAstRefOrAddr invocationObject,
			List<IAstStackItem> arguments)
		{
			return new AstCallMethod(
					methodInfo, invocationObject, arguments
				);
		}


		#region Const
		public static AstConstantString Const(string value)
		{
			return new AstConstantString { str = value };
		}
		public static AstConstantInt32 Const(Int32 value)
		{
			return new AstConstantInt32 { value = value };
		}
		public static AstConstantNull Null
		{
			get { return new AstConstantNull(); }
		}
		#endregion

		#region Arg

		public static AstReadArgument Arg(int index, Type argumentType)
		{
			return new AstReadArgument { argumentIndex = index, argumentType = argumentType };
		}

		public static AstReadArgumentRef ArgRef(int index, Type argumentType)
		{
			return new AstReadArgumentRef { argumentIndex = index, argumentType = argumentType };
		}

		public static AstReadArgumentValue ArgVal(int index, Type argumentType)
		{
			return new AstReadArgumentValue { argumentIndex = index, argumentType = argumentType };
		}

		public static IAstRefOrAddr ArgRefOrAddr(int index, Type argumentType)
		{
			return argumentType.IsValueType
				? (IAstRefOrAddr) new AstReadArgumentAddr { argumentIndex = index, argumentType = argumentType }
				: new AstReadArgumentRef {argumentIndex = index, argumentType = argumentType};
		}

		#endregion

		#region This

		public static AstReadThis This { get { return new AstReadThis(); } }

		#endregion

		#region TypeOf

		public static AstTypeof TypeOf(Type type)
		{
			return new AstTypeof { type = type };
		}

		#endregion


		#region Return
		public static AstReturnVoid RetVoid
		{
			get { return new AstReturnVoid(); }
		}

		#endregion

		#region Conditional return

		public static AstIfFalseRetVoid IfFalseRetVoid(IAstStackItem node)
		{
			return new AstIfFalseRetVoid(node);
		}

		#endregion

		#region AstIf
		public static AstIf If(
			IAstValue condition,
			AstComplexNode trueBranch,
			AstComplexNode falseBranch)
		{
			return new AstIf { condition = condition, falseBranch = falseBranch, trueBranch = trueBranch };
		}
		public static AstIf If(
			IAstValue condition,
			AstComplexNode trueBranch)
		{
			return If(condition, trueBranch, null);
		}
		#endregion

		#region AstComplexNode
		public static AstComplexNode Complex(params IAstNode[] nodes)
		{
			return new AstComplexNode { nodes = nodes.ToList() };
		}

		public static AstComplexNode Complex(List<IAstNode> nodes)
		{
			return new AstComplexNode { nodes = nodes };
		}
		#endregion
	}
}
