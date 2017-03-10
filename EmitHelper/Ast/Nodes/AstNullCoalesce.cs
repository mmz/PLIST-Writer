using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Generates "value ?? ifNullValue" expression.
	/// </summary>
	public class AstNullCoalesce : IAstRefOrValue
	{
		IAstRef _value;
		IAstRefOrValue _ifNullValue;

		public Type itemType
		{
			get 
			{
				return _value.itemType;
			}
		}

		public AstNullCoalesce(IAstRef value, IAstRefOrValue ifNullValue)
		{
			_value = value;
			_ifNullValue = ifNullValue;
			if (!_value.itemType.IsAssignableFrom(_ifNullValue.itemType))
			{
				throw new InvalidOperationException("Incorrect ifnull expression");
			}
		}

		public void Compile(ICompilationContext context)
		{
			Label ifNotNullLabel = context.DefineLabel();
			_value.Compile(context);
			context.Emit(OpCodes.Dup);
			context.Emit(OpCodes.Brtrue_S, ifNotNullLabel);
			context.Emit(OpCodes.Pop);
			_ifNullValue.Compile(context);
			context.MarkLabel(ifNotNullLabel);
		}
	}
}