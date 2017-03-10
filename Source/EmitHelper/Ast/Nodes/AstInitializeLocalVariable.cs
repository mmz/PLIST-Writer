using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
    public class AstInitializeLocalVariable: IAstNode
    {
        public Type localType;
        public int localIndex;

        public AstInitializeLocalVariable()
        {
        }

        public AstInitializeLocalVariable(LocalBuilder loc)
        {
            localType = loc.LocalType;
            localIndex = loc.LocalIndex;
        }

        public void Compile(ICompilationContext context)
        {
            if(localType.IsValueType)
            {
                context.Emit(OpCodes.Ldloca, localIndex);
                context.Emit(OpCodes.Initobj, localType);
            }
        }
    }
}