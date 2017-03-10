using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Pushes a supplied value of type int32 onto the evaluation stack as an int32.
	/// </summary>
	public class AstConstantInt32 : IAstValue
    {
        public Int32 value;

        #region IAstReturnValueNode Members

        public Type itemType
        {
            get { return typeof(Int32); }
        }

        #endregion

        #region IAstNode Members

        public void Compile(ICompilationContext context)
        {
            context.Emit(OpCodes.Ldc_I4, value);
        }

        #endregion
    }
}