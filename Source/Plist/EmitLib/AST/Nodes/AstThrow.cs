using EmitLib.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitLib.AST.Nodes
{
    class AstThrow: IAstNode
    {
        public IAstRef Exception;

        public void Compile(CompilationContext context)
        {
            Exception.Compile(context);
            context.Emit(OpCodes.Throw);
        }
    }
}