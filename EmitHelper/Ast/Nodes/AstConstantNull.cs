using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Pushes a null reference (type O) onto the evaluation stack.
	/// </summary>
	public class AstConstantNull : IAstRefOrValue
    {
        #region IAstReturnValueNode Members

        public Type itemType
        {
            get { return typeof(object); }
        }

        #endregion

        #region IAstNode Members

        public void Compile(ICompilationContext context)
        {
            context.Emit(OpCodes.Ldnull);
        }

        #endregion
    }
}