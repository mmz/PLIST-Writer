using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Attempts to cast an object passed by reference to the specified class.
	/// </summary>
	public class AstCastclass : IAstRefOrValue
	{
        protected IAstRefOrValue _value;
        protected Type _targetType;

        public AstCastclass(IAstRefOrValue value, Type targetType)
		{
			_value = value;
			_targetType = targetType;
		}

		#region IAstStackItem Members

		public Type itemType
		{
			get { return _targetType; }
		}

		#endregion

		#region IAstNode Members

		public virtual void Compile(ICompilationContext context)
		{

            if (_value.itemType != _targetType)
            {
                if (!_value.itemType.IsValueType && !_targetType.IsValueType)
                {
                    _value.Compile(context);
                    context.Emit(OpCodes.Castclass, _targetType);
                    return;
                }
                else if (_targetType.IsValueType && !_value.itemType.IsValueType)
                {
                    new AstUnbox() { refObj = (IAstRef)_value, unboxedType = _targetType }.Compile(context);
                    return;
                }
				//ToDo?: Think about exception type.
                throw new NotSupportedException();
            }
            else
            {
                _value.Compile(context);
            }
		}

		#endregion
	}

	/// <summary>
	/// Attempts to cast an object passed by reference to the specified reference type.
	/// </summary>
	public class AstCastclassRef : AstCastclass, IAstRef
    {
        public AstCastclassRef(IAstRefOrValue value, Type targetType): base(value, targetType)
		{
		}

        public override void Compile(ICompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
	}

	/// <summary>
	/// Attempts to cast an object passed by reference to the specified value type.
	/// </summary>
	public class AstCastclassValue : AstCastclass, IAstValue
    {
        public AstCastclassValue(IAstRefOrValue value, Type targetType): base(value, targetType)
		{
		}

        public override void Compile(ICompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            base.Compile(context);
        }
    }
}
