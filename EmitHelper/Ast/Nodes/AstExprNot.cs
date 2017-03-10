using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Compares value with 0. If they are equal, the integer value 1 (int32) is pushed onto the evaluation stack; otherwise 0 (int32) is pushed onto the evaluation stack.
	/// </summary>
	public class AstExprNot : IAstValue
	{
		IAstRefOrValue _value;

		public Type itemType
		{
			get { return typeof(Int32); }
		}

        public AstExprNot(IAstRefOrValue value)
		{
			_value = value;
		}

		public void Compile(ICompilationContext context)
		{
			context.Emit(OpCodes.Ldc_I4_0);
			_value.Compile(context);
			context.Emit(OpCodes.Ceq);
		}
	}
}
