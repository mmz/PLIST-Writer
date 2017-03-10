using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Calls the void method indicated by the MethodInfo descriptor.
	/// </summary>
	public class AstCallMethodVoid : IAstNode
	{
		protected MethodInfo methodInfo;
		protected IAstRefOrAddr invocationObject;
		protected List<IAstStackItem> arguments;

		public AstCallMethodVoid(
			MethodInfo methodInfo,
			IAstRefOrAddr invocationObject,
			List<IAstStackItem> arguments)
		{
			if (methodInfo == null)
				throw new ArgumentNullException(nameof(methodInfo));
			this.methodInfo = methodInfo;
			this.invocationObject = invocationObject;
			this.arguments = arguments;
		}

		public void Compile(ICompilationContext context)
		{
			new AstCallMethod(methodInfo, invocationObject, arguments).Compile(context);

			if (methodInfo.ReturnType != typeof(void))
			{
				context.Emit(OpCodes.Pop);
			}
		}
	}
}