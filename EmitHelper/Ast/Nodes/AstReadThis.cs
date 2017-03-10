using System;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	public class AstReadThis : IAstRefOrAddr
	{
		public Type thisType;

		public Type itemType
		{
			get
			{
				return thisType;
			}
		}

		public AstReadThis()
		{
		}

		public virtual void Compile(ICompilationContext context)
		{
			AstReadArgument arg = new AstReadArgument()
			{
				argumentIndex = 0,
				argumentType = thisType
			};
			arg.Compile(context);
		}
	}

	public class AstReadThisRef : AstReadThis, IAstRef
	{
		public override void Compile(ICompilationContext context)
		{
			CompilationHelper.CheckIsRef(itemType);
			base.Compile(context);
		}
	}

	public class AstReadThisAddr : AstReadThis, IAstRef
	{
		public override void Compile(ICompilationContext context)
		{
			CompilationHelper.CheckIsRef(itemType);
			base.Compile(context);
		}
	}
}