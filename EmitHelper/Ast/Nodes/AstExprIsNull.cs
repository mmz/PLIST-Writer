using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// If value is null, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
	/// </summary>
	public class AstExprIsNull : IAstValue
    {
        IAstRefOrValue value;

		public AstExprIsNull(IAstRefOrValue value)
        {
            this.value = value;
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
            if (!(value is IAstRef) && !ReflectionUtils.IsNullable(value.itemType))
            {
                context.Emit(OpCodes.Ldc_I4_1);
            }
			else if (ReflectionUtils.IsNullable(value.itemType))
			{
				AstBuildHelper.ReadPropertyRV(
					new AstValueToAddr((IAstValue)value),
					value.itemType.GetProperty("HasValue")
				).Compile(context);
				context.Emit(OpCodes.Ldc_I4_0);
				context.Emit(OpCodes.Ceq);
			}
			else
			{
				value.Compile(context);
				new AstConstantNull().Compile(context);
				context.Emit(OpCodes.Ceq);
			}
        }

        #endregion
    }
}
