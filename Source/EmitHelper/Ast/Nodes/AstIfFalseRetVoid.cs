using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	public class AstIfFalseRetVoid : IAstNode
	{
		IAstStackItem _value;

		public AstIfFalseRetVoid(IAstStackItem value)
		{
			_value = value;
		}

		public void Compile(ICompilationContext context)
		{
			Label ifNotNullLabel = context.DefineLabel();
			_value.Compile(context);
			context.Emit(OpCodes.Brtrue_S, ifNotNullLabel);
			context.Emit(OpCodes.Ret);
			context.MarkLabel(ifNotNullLabel);
		}
	}
}