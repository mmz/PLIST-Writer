using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitLib.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitLib.AST.Nodes
{
	class AstTypeof: IAstRef
	{
		public Type type;

		#region IAstStackItem Members

		public Type itemType
		{
			get 
			{
				return typeof(Type);
			}
		}

		#endregion

		#region IAstNode Members

		public void Compile(CompilationContext context)
		{
			context.Emit(OpCodes.Ldtoken, type);
			context.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
		}

		#endregion
	}
}
