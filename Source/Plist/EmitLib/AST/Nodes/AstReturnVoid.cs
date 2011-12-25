using EmitLib.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitLib.AST.Nodes
{
	class AstReturnVoid : IAstNode
	{
		#region IAstNode Members

		public void Compile(CompilationContext context)
		{
			context.Emit(OpCodes.Ret);
		}

		#endregion
	}
}