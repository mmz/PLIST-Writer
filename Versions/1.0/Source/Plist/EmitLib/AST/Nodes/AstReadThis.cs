using System;
using EmitLib.AST.Interfaces;
using EmitLib.AST.Helpers;

namespace EmitLib.AST.Nodes
{
	class AstReadThis : IAstRefOrAddr
	{
		public Type thisType;

		public Type itemType
		{
			get
			{
				return thisType;
			}
		}

		public virtual void Compile(CompilationContext context)
		{
			var arg = new AstReadArgument
			          	{
												  argumentIndex = 0,
												  argumentType = thisType
											  };
			arg.Compile(context);
		}
	}

	class AstReadThisRef : AstReadThis, IAstRef
	{
		override public void Compile(CompilationContext context)
		{
			CompilationHelper.CheckIsRef(itemType);
			base.Compile(context);
		}
	}

	class AstReadThisAddr : AstReadThis, IAstRef
	{
		override public void Compile(CompilationContext context)
		{
			CompilationHelper.CheckIsRef(itemType);
			base.Compile(context);
		}
	}
}