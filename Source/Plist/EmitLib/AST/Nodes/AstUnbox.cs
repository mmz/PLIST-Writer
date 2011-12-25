using System;
using System.Reflection.Emit;
using EmitLib.AST.Interfaces;

namespace EmitLib.AST.Nodes
{
	class AstUnbox : IAstValue
	{
		public Type unboxedType;
		public IAstRef refObj;

		public Type itemType
		{
			get { return unboxedType; }
		}

		public void Compile(CompilationContext context)
		{
			refObj.Compile(context);
			context.Emit(OpCodes.Unbox_Any, unboxedType);
		}
	}
}