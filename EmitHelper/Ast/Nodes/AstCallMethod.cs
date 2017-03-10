using System;
using System.Collections.Generic;
using System.Reflection;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Calls the method indicated by the MethodInfo descriptor, pushing the return value onto the evaluation stack.
	/// </summary>
	public class AstCallMethod : IAstRefOrValue
	{
		public MethodInfo methodInfo;
		public IAstRefOrAddr invocationObject;
		public List<IAstStackItem> arguments;

		public AstCallMethod(
			MethodInfo methodInfo,
			IAstRefOrAddr invocationObject,
			List<IAstStackItem> arguments)
		{
			if (methodInfo == null)
			{
				throw new ArgumentNullException(nameof(methodInfo));
			}
			this.methodInfo = methodInfo;
			this.invocationObject = invocationObject;
			this.arguments = arguments;
		}

		public Type itemType
		{
			get
			{
				return methodInfo.ReturnType;
			}
		}

		public virtual void Compile(ICompilationContext context)
		{
			CompilationHelper.EmitCall(context, invocationObject, methodInfo, arguments);
		}
	}
	/// <summary>
	/// Calls the method indicated by the MethodInfo descriptor, pushing the return value reference onto the evaluation stack.
	/// </summary>
	public class AstCallMethodRef : AstCallMethod, IAstRef
	{
		public AstCallMethodRef(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
			: base(methodInfo, invocationObject, arguments)
		{
		}

		public override void Compile(ICompilationContext context)
		{
			CompilationHelper.CheckIsRef(itemType);
			base.Compile(context);
		}
	}

	/// <summary>
	/// Calls the method indicated by the MethodInfo descriptor, pushing the return value onto the evaluation stack.
	/// </summary>
	public class AstCallMethodValue : AstCallMethod, IAstValue
	{
		public AstCallMethodValue(MethodInfo methodInfo, IAstRefOrAddr invocationObject, List<IAstStackItem> arguments)
			: base(methodInfo, invocationObject, arguments)
		{
		}
		public override void Compile(ICompilationContext context)
		{
			CompilationHelper.CheckIsValue(itemType);
			base.Compile(context);
		}
	}
}