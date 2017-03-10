using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
    public class AstReadArgument : IAstStackItem
    {
        public int argumentIndex;
        public Type argumentType;

        public Type itemType
        {
            get
            {
                return argumentType;
            }
        }

        public virtual void Compile(ICompilationContext context)
        {
            switch (argumentIndex)
            {
                case 0:
                    context.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    context.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    context.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    context.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    context.Emit(OpCodes.Ldarg, argumentIndex);
                    break;
            }
        }
    }

    public class AstReadArgumentRef : AstReadArgument, IAstRef
    {
        public override void Compile(ICompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }

    public class AstReadArgumentValue : AstReadArgument, IAstValue
    {
        public override void Compile(ICompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            base.Compile(context);
        }
    }

    public class AstReadArgumentAddr : AstReadArgument, IAstAddr
    {
        public override void Compile(ICompilationContext context)
        {
			CompilationHelper.CheckIsValue(itemType);
			context.Emit(
				(argumentIndex <= 255)
					? OpCodes.Ldarga_S
					: OpCodes.Ldarga
			, argumentIndex);
		}
    }
}