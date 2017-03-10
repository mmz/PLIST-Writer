using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
    public class AstReturnVoid:IAstNode
    {
        #region IAstNode Members

        public void Compile(ICompilationContext context)
        {
            context.Emit(OpCodes.Ret);
        }

        #endregion
    }
}