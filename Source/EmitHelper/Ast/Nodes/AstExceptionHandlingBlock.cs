using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Creates try-catch block for a specified exception type.
	/// </summary>
	public class AstExceptionHandlingBlock : IAstNode
    {
        IAstNode protectedBlock;
        IAstNode handlerBlock;
        Type exceptionType;
        LocalBuilder exceptionVariable;

        public AstExceptionHandlingBlock(
            IAstNode protectedBlock, 
            IAstNode handlerBlock, 
            Type exceptionType,
            LocalBuilder exceptionVariable)
        {
            this.protectedBlock = protectedBlock;
            this.handlerBlock = handlerBlock;
            this.exceptionType = exceptionType;
            this.exceptionVariable = exceptionVariable;
        }

        #region IAstNode Members

        public void Compile(ICompilationContext context)
        {
            var endBlock = context.BeginExceptionBlock();
            protectedBlock.Compile(context);
            context.BeginCatchBlock(exceptionType);
            context.Emit(OpCodes.Stloc, exceptionVariable);
            handlerBlock.Compile(context);
            context.EndExceptionBlock();
        }

        #endregion
    }
}