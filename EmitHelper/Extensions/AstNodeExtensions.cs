using EmitHelper.Ast.Interfaces;
using EmitHelper.Ast.Nodes;
using System;
using System.Linq;
using System.Reflection;

namespace EmitHelper.Extensions
{
	public static class AstNodeExtensions
	{

		#region Call
		public static AstCallMethodVoid CallVoid(this IAstRefOrAddr invocationObject, MethodInfo methodInfo, params IAstStackItem[] arguments)
		{
			return Ast.CallVoid(methodInfo, methodInfo.IsStatic ? null : invocationObject, arguments.ToList());
		}
		public static AstCallMethodVoid CallVoid(this IAstRefOrAddr invocationObject, string methodName, params IAstStackItem[] arguments)
		{
			var methodInfo = invocationObject.itemType.GetMethod(methodName, arguments.Select(i => i.itemType).ToArray());
			return Ast.CallVoid(methodInfo, methodInfo.IsStatic ? null : invocationObject, arguments.ToList());
		}


		public static AstCallMethod Call(this IAstRefOrAddr invocationObject, MethodInfo methodInfo, params IAstStackItem[] arguments)
		{
			return Ast.Call(methodInfo, invocationObject, arguments.ToList());
		}
		public static AstCallMethod Call(this IAstRefOrAddr invocationObject, string methodName, params IAstStackItem[] arguments)
		{
			var methodInfo = invocationObject.itemType.GetMethod(methodName, arguments.Select(i => i.itemType).ToArray());
			return Ast.Call(methodInfo, invocationObject, arguments.ToList());
		}
		#endregion

		#region AstReadProperty

		public static IAstRefOrValue ReadProp(this IAstRef sourceObject, PropertyInfo property)
		{
			return ReadProp((IAstRefOrAddr)sourceObject, property);
		}

		public static IAstRefOrValue ReadProp(this IAstRefOrAddr sourceObject, PropertyInfo property)
		{
			return new AstReadProperty
			{
				sourceObject = sourceObject,
				propertyInfo = property
			};
		}

		public static AstReadPropertyRef ReadPropRef(this IAstRefOrAddr sourceObject, PropertyInfo property)
		{
			return new AstReadPropertyRef
			{
				sourceObject = sourceObject,
				propertyInfo = property
			};
		}

		public static AstReadPropertyValue ReadPropVal(this IAstRefOrAddr sourceObject, PropertyInfo property)
		{
			return new AstReadPropertyValue
			{
				sourceObject = sourceObject,
				propertyInfo = property
			};
		}

		#endregion

		#region AstReadField
		public static AstReadField Field(this IAstRefOrAddr sourceObject, FieldInfo field)
		{
			return new AstReadField { fieldInfo = field, sourceObject = sourceObject };
		}
		public static AstReadFieldRef FieldRef(this IAstRefOrAddr sourceObject, FieldInfo field)
		{
			return new AstReadFieldRef { fieldInfo = field, sourceObject = sourceObject };
		}

		#endregion

		#region AstCastclass
		public static AstCastclass Cast(this IAstRefOrValue value, Type targetType)
		{
			return new AstCastclass(value, targetType);
		}
		public static AstCastclassRef Cast(this IAstRef value, Type targetType)
		{
			return new AstCastclassRef(value, targetType);
		}
		public static AstCastclassValue Cast(this IAstValue value, Type targetType)
		{
			return new AstCastclassValue(value, targetType);
		}

		#endregion

		#region Box/Unbox
		public static AstUnbox BoxUnbox(this IAstRefOrValue value, Type unboxedType)
		{
			return Unbox(Box(value), unboxedType);
		}
		public static AstUnbox Unbox(this IAstRef refObj, Type unboxedType)
		{
			return new AstUnbox { refObj = refObj, unboxedType = unboxedType };
		}

		public static AstBox Box(this IAstRefOrValue value)
		{
			return new AstBox { value = value };
		}
		#endregion

		#region AstReadArrayItem
		public static AstReadArrayItem ReadItem(this IAstRef array, int index)
		{
			return new AstReadArrayItem { array = array, index = index };
		}
		public static AstReadArrayItemRef ReadItemRef(this IAstRef array, int index)
		{
			return new AstReadArrayItemRef { array = array, index = index };
		}

		#endregion

		#region Expressions
		#region AstExprIsNull
		public static AstExprIsNull IsNull(this IAstRefOrValue value)
		{
			return new AstExprIsNull(value);
		}
		#endregion

		#region AstExprNot
		public static AstExprNot Not(this IAstRefOrValue value)
		{
			return new AstExprNot(value);
		}
		#endregion

		#region AstExprIsNotNull
		public static AstExprNot NotNull(this IAstRefOrValue value)
		{
			return Not(IsNull(value));
		}
		#endregion

		#endregion

		#region Control transfer
		public static AstIfNotNull IfNotNull(this IAstStackItem condition, IAstNode trueBranch, IAstNode falseBranch)
		{
			return new AstIfNotNull { condition = condition, trueBranch = trueBranch, falseBranch = falseBranch };
		}
		public static AstIfNotNull IfNotNull_S(this IAstStackItem condition, IAstNode trueBranch, IAstNode falseBranch)
		{
			return new AstIfNotNull { condition = condition, trueBranch = trueBranch, falseBranch = falseBranch, useShort = true };
		}

		#endregion
	}
}
