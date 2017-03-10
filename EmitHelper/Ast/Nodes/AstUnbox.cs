using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Converts the boxed representation of a value type to specifeid form.
	/// </summary>
	public class AstUnbox : IAstValue
    {
        public Type unboxedType;
        public IAstRef refObj;

		/// <summary>
		/// Тип распакованного объекта.
		/// </summary>
        public Type itemType
        {
            get { return unboxedType; }
        }

        public void Compile(ICompilationContext context)
        {
            refObj.Compile(context);
            context.Emit(OpCodes.Unbox_Any, unboxedType);
        }
    }
}