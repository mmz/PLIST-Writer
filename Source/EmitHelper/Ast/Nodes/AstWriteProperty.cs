using System;
using System.Collections.Generic;
using System.Reflection;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Sets the specified property to the specified value for the given object.
	/// </summary>
	public class AstWriteProperty : IAstNode
	{
		private IAstRefOrAddr _targetObject;
		private IAstRefOrValue _value;
		private PropertyInfo _propertyInfo;
		private MethodInfo _setMethod;

		public AstWriteProperty(IAstRefOrAddr targetObject, IAstRefOrValue value, PropertyInfo propertyInfo)
		{
			_targetObject = targetObject;
			_value = value;
			_propertyInfo = propertyInfo;
			_setMethod = propertyInfo.GetSetMethod();
			if (_setMethod == null)
			{
				throw new ArgumentException("Property " + propertyInfo.Name + " doesn't have set accessor");
			}
			if (_setMethod.GetParameters().Length != 1)
			{
				throw new ArgumentException("Property " + propertyInfo.Name + " has invalid arguments");
			}
		}

		public void Compile(ICompilationContext context)
		{
			AstBuildHelper.CallMethod(
				_setMethod,
				_targetObject,
				new List<IAstStackItem>() { _value }
			).Compile(context);
		}
	}
}