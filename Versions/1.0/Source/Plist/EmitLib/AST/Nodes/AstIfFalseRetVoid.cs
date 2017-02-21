using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using EmitLib.AST;
using EmitLib.AST.Interfaces;

namespace Plist.EmitLib.AST.Nodes
{
	class AstIfFalseRetVoid : IAstNode
	{
		IAstStackItem _value;

		public AstIfFalseRetVoid(IAstStackItem value)
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