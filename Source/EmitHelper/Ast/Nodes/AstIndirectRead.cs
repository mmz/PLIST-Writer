using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	public abstract class AstIndirectRead : IAstStackItem
	{
		public Type argumentType;

		public Type itemType
		{
			get
			{
				return argumentType;
			}
		}

		public abstract void Compile(ICompilationContext context);
	}

	public class AstIndirectReadRef : AstIndirectRead, IAstRef
	{
		public override void Compile(ICompilationContext context)
		{
			CompilationHelper.CheckIsRef(itemType);
			context.Emit(OpCodes.Ldind_Ref, itemType);
		}
	}

	public class AstIndirectReadValue : AstIndirectRead, IAstValue
	{
		public override void Compile(ICompilationContext context)
		{
			CompilationHelper.CheckIsValue(itemType);
			if (itemType == typeof(Int32))
			{
				context.Emit(OpCodes.Ldind_I4);
			}
			else
			{
				throw new Exception("Unsupported type");
			}
		}
	}

	public class AstIndirectReadAddr : AstIndirectRead, IAstAddr
	{
		public override void Compile(ICompilationContext context)
		{
			CompilationHelper.CheckIsValue(itemType);
		}
	}
}