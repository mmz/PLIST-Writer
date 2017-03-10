using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
    public class AstIfTernar : IAstRefOrValue
    {
        public IAstRefOrValue condition;
        public IAstRefOrValue trueBranch;
        public IAstRefOrValue falseBranch;

        #region IAstNode Members

        public Type itemType
        {
            get 
            {
                return trueBranch.itemType;
            }
        }

        public AstIfTernar(IAstRefOrValue condition, IAstRefOrValue trueBranch, IAstRefOrValue falseBranch)
        {
            if (trueBranch.itemType != falseBranch.itemType)
            {
                throw new ArgumentException("Types mismatch");
            }

            this.condition = condition;
            this.trueBranch = trueBranch;
            this.falseBranch = falseBranch;
        }

        public void Compile(ICompilationContext context)
        {
            Label elseLabel = context.DefineLabel();
            Label endIfLabel = context.DefineLabel();

            condition.Compile(context);
            context.Emit(OpCodes.Brfalse, elseLabel);

            if (trueBranch != null)
            {
                trueBranch.Compile(context);
            }
            if (falseBranch != null)
            {
                context.Emit(OpCodes.Br, endIfLabel);
            }

            context.MarkLabel(elseLabel);
            if (falseBranch != null)
            {
                falseBranch.Compile(context);
            }
            context.MarkLabel(endIfLabel);
        }

        #endregion
    }
}
