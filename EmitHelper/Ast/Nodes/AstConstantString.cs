using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Pushes a new object reference to a string literal stored in the metadata.
	/// </summary>
	public class AstConstantString: IAstRef
    {
        public string str;

        #region IAstStackItem Members

        public Type itemType
        {
            get 
            {
                return typeof(string);
            }
        }

        #endregion

        #region IAstNode Members

        public void Compile(ICompilationContext context)
        {
            context.Emit(OpCodes.Ldstr, str);
        }

        #endregion
    }
}