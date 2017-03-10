using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	public class AstTypeof: IAstRef
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

		public void Compile(ICompilationContext context)
		{
			context.Emit(OpCodes.Ldtoken, type);
			context.EmitCall(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
		}

		#endregion
	}
}
