using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Conversion of a value type to an object reference (type O).
	/// </summary>
	public class AstBox : IAstRef
    {
        public IAstRefOrValue value;

        #region IAstReturnValueNode Members

		/// <summary>
		/// Type of the source object.
		/// </summary>
        public Type itemType
        {
            get 
            {
                return value.itemType;  
            }
        }

        #endregion

        #region IAstNode Members

        public void Compile(ICompilationContext context)
        {
            value.Compile(context);

            if (value.itemType.IsValueType)
            {
                context.Emit(OpCodes.Box, itemType);
            }
        }

        #endregion
    }
}