using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
    public class AstValueToAddr: IAstAddr
    {
        public IAstValue value;
        public Type itemType
        {
            get 
            {
                return value.itemType; 
            }
        }

        public AstValueToAddr(IAstValue value)
        {
            this.value = value;
        }

        public void Compile(ICompilationContext context)
        {
            LocalBuilder loc = context.DeclareLocal(itemType);
            new AstInitializeLocalVariable(loc).Compile(context);
            new AstWriteLocal()
                {
                    localIndex = loc.LocalIndex,
                    localType = loc.LocalType,
                    value = value
                }.Compile(context);
            new AstReadLocalAddr(loc).Compile(context);
        }
    }
}