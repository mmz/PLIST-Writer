using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
    public class AstThrow: IAstNode
    {
        public IAstRef exception;

        public void Compile(ICompilationContext context)
        {
            exception.Compile(context);
            context.Emit(OpCodes.Throw);
        }
    }
}