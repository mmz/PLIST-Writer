using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using EmitLib.AST.Helpers;
using EmitLib.AST.Interfaces;
using Plist.EmitLib.AST.Nodes;

namespace EmitLib.AST.Nodes
{
	static class Ast
	{
		#region LocalBuilder

		public static IAstValue Val(this LocalBuilder loc)
		{
			return new AstReadLocalValue
			{
				localType = loc.LocalType,
				localIndex = loc.LocalIndex
			};
		}

		public static IAstRef Ref(this LocalBuilder loc)
		{
			return new AstReadLocalRef
			{
				localType = loc.LocalType,
				localIndex = loc.LocalIndex
			};
		}

		public static IAstRefOrAddr RoA(this LocalBuilder loc)
		{
			return loc.LocalType.IsValueType
				? (IAstRefOrAddr) new AstReadLocalAddr(loc)
				: new AstReadLocalRef
				{
					localType = loc.LocalType,
					localIndex = loc.LocalIndex
				};
		}

		public static IAstRefOrValue RoV(this LocalBuilder loc)
		{
			return loc.LocalType.IsValueType
				? (IAstRefOrValue) new AstReadLocalValue
				{
					localType = loc.LocalType,
					localIndex = loc.LocalIndex
				}
				: new AstReadLocalRef
				{
					localType = loc.LocalType,
					localIndex = loc.LocalIndex
				};
		}

		#endregion

		#region Call metod

		#region AstCallMethodVoid

		public static AstCallMethodVoid CallVoidM(
			MethodInfo methodInfo,
			IAstRefOrAddr invocationObject,
			List<IAstStackItem> arguments)
		{
			return new AstCallMethodVoid(
				methodInfo, invocationObject, arguments
			);
		}

		public static AstCallMethodVoid CallVoid(this MethodInfo methodInfo, IAstRefOrAddr invocationObject,
			params IAstStackItem[] arguments)
		{
			return CallVoidM(methodInfo, invocationObject, arguments.ToList());
		}

		public static AstCallMethodVoid CallVoid(this MethodInfo methodInfo, LocalBuilder local,
			params IAstStackItem[] arguments)
		{
			return CallVoidM(methodInfo, local.RoA(), arguments.ToList());
		}

		public static AstCallMethodVoid CallVoid(string methodName, LocalBuilder local, params IAstStackItem[] arguments)
		{
			var methodInfo = local.LocalType.GetMethod(methodName, arguments.Select(i => i.itemType).ToArray());
			return CallVoidM(methodInfo, local.RoA(), arguments.ToList());
		}




		public static AstCallMethodVoid CallVoid(this IAstRefOrAddr invocationObject, MethodInfo methodInfo,
			params IAstStackItem[] arguments)
		{
			return CallVoidM(methodInfo, methodInfo.IsStatic ? null : invocationObject, arguments.ToList());
		}

		public static AstCallMethodVoid CallVoid(this IAstRefOrAddr invocationObject, string methodName,
			params IAstStackItem[] arguments)
		{
			var methodInfo = invocationObject.itemType.GetMethod(methodName, arguments.Select(i => i.itemType).ToArray());
			return CallVoidM(methodInfo, methodInfo.IsStatic ? null : invocationObject, arguments.ToList());
		}

		#endregion

		#region Call Method Val

		public static AstCallMethod Call(
			MethodInfo methodInfo,
			IAstRefOrAddr invocationObject,
			List<IAstStackItem> arguments)
		{
			return new AstCallMethod(
				methodInfo, invocationObject, arguments
			);
		}

		public static AstCallMethod Call(this IAstRefOrAddr invocationObject, MethodInfo methodInfo,
			params IAstStackItem[] arguments)
		{
			return Call(methodInfo, invocationObject, arguments.ToList());
		}

		public static AstCallMethod Call(this IAstRefOrAddr invocationObject, string methodName,
			params IAstStackItem[] arguments)
		{
			var methodInfo = invocationObject.itemType.GetMethod(methodName, arguments.Select(i => i.itemType).ToArray());
			return Call(methodInfo, invocationObject, arguments.ToList());
		}

		#endregion

		#endregion

		#region Const

		public static AstConstantString Const(string value)
		{
			return new AstConstantString {str = value};
		}

		public static AstConstantInt32 Const(Int32 value)
		{
			return new AstConstantInt32 {value = value};
		}

		public static AstConstantNull Null
		{
			get { return new AstConstantNull(); }
		}

		#endregion

		#region AstReadProperty

		public static AstReadProperty ReadProp(this IAstRefOrAddr sourceObject, PropertyInfo property)
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
			return new AstReadField {fieldInfo = field, sourceObject = sourceObject};
		}

		public static AstReadFieldRef FieldRef(this IAstRefOrAddr sourceObject, FieldInfo field)
		{
			return new AstReadFieldRef {fieldInfo = field, sourceObject = sourceObject};
		}

		#endregion

		#region Arg


		public static AstReadArgument Arg(int index, Type argumentType)
		{
			return new AstReadArgument {argumentIndex = index, argumentType = argumentType};
		}

		public static AstReadArgumentRef ArgR(int index, Type argumentType)
		{
			return new AstReadArgumentRef { argumentIndex = index, argumentType = argumentType };

		}

		public static AstReadArgumentValue ArgV(int index, Type argumentType)
		{
			return new AstReadArgumentValue { argumentIndex = index, argumentType = argumentType };

		}

		public static IAstRefOrAddr ArgRoA(int index, Type argumentType)
		{
			return argumentType.IsValueType
				? (IAstRefOrAddr) new AstReadArgumentAddr {argumentIndex = index, argumentType = argumentType}
				: new AstReadArgumentRef {argumentIndex = index, argumentType = argumentType};
		}

		#endregion

		#region This

		public static AstReadThis This
		{
			get { return new AstReadThis(); }
		}

		#endregion

		#region TypeOf

		public static AstTypeof TypeOf(Type type)
		{
			return new AstTypeof {type = type};
		}

		#endregion

		#region AstReadArrayItem

		public static AstReadArrayItem ReadItem(this IAstRef array, int index)
		{
			return new AstReadArrayItem {array = array, index = index};
		}

		public static AstReadArrayItemRef ReadItemRef(this IAstRef array, int index)
		{
			return new AstReadArrayItemRef {array = array, index = index};
		}

		#endregion

		#region Return

		public static AstReturnVoid RetVoid
		{
			get { return new AstReturnVoid(); }
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

		#region AstIf

		public static AstIf If(
			IAstValue condition,
			AstComplexNode trueBranch,
			AstComplexNode falseBranch)
		{
			return new AstIf {condition = condition, falseBranch = falseBranch, trueBranch = trueBranch};
		}

		public static AstIf If(
			IAstValue condition,
			AstComplexNode trueBranch)
		{
			return If(condition, trueBranch, null);
		}

		public static AstIfNotNull IfNotNull(this IAstRefOrValue condition, IAstNode trueBranch, IAstNode falseBranch)
		{
			return new AstIfNotNull {condition = condition, trueBranch = trueBranch, falseBranch = falseBranch};
		}


		public static AstIfFalseRetVoid IfFalseRetVoid(IAstStackItem node)
		{
			return new AstIfFalseRetVoid(node);
		}

		#endregion

		#region AstComplexNode

		public static AstComplexNode Complex(params IAstNode[] nodes)
		{
			return new AstComplexNode {nodes = nodes.ToList()};
		}

		internal static AstComplexNode Complex(List<IAstNode> nodes)
		{
			return new AstComplexNode {nodes = nodes};
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

		#region Box/Unbox/Cast

		public static AstUnbox BoxUnbox(this IAstRefOrValue value, Type unboxedType)
		{
			return Unbox(Box(value), unboxedType);
		}

		public static AstUnbox Unbox(this IAstRef refObj, Type unboxedType)
		{
			return new AstUnbox {refObj = refObj, unboxedType = unboxedType};
		}

		public static AstBox Box(this IAstRefOrValue value)
		{
			return new AstBox {value = value};
		}

		public static IAstValue ToValue(this IAstRefOrValue value)
		{
			return ToValue(value, value.itemType);
		}

		public static IAstValue ToValue(this IAstRefOrValue value, Type toType)
		{
			return new AstCastclassValue(value, toType);
		}
		#endregion
	}
}
