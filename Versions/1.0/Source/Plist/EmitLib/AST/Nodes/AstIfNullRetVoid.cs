using System.Reflection.Emit;
using EmitLib.AST.Interfaces;

namespace EmitLib.AST.Nodes
{
	class AstIfNullRetVoid : IAstNode
	{
		IAstRef _value;

		public AstIfNullRetVoid(IAstRef value)
		{
			_value = value;
		}

		public void Compile(CompilationContext context)
		{
			Label ifNotNullLabel = context.ilGenerator.DefineLabel();
			_value.Compile(context);
			context.Emit(OpCodes.Brtrue_S, ifNotNullLabel);
			context.Emit(OpCodes.Ret);
			context.ilGenerator.MarkLabel(ifNotNullLabel);
		}
	}
}