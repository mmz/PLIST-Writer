using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Compares two values. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
	/// </summary>
	public class AstExprEquals : IAstValue
    {
        IAstValue leftValue;
        IAstValue rightValue;

        public AstExprEquals(IAstValue leftValue, IAstValue rightValue)
        {
            this.leftValue = leftValue;
            this.rightValue = rightValue;
        }

        #region IAstReturnValueNode Members

        public Type itemType
        {
            get { return typeof(Int32); }
        }

        #endregion

        #region IAstNode Members

        public void Compile(ICompilationContext context)
        {
            leftValue.Compile(context);
            rightValue.Compile(context);
            context.Emit(OpCodes.Ceq);
        }

        #endregion
    }
}