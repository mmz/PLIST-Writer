using System;
using System.Collections.Generic;
using System.Linq;
using EmitLib.AST.Interfaces;
using System.Reflection;
using EmitLib.AST.Nodes;
using System.Reflection.Emit;

namespace EmitLib.AST.Helpers
{
	class AstBuildHelper
	{
		public static IAstRefOrValue CallMethod(
			 MethodInfo methodInfo,
			 IAstRefOrAddr invocationObject,
			 List<IAstStackItem> arguments)
		{
			return methodInfo.ReturnType.IsValueType
						? (IAstRefOrValue)new AstCallMethodValue(methodInfo, invocationObject, arguments)
						: new AstCallMethodRef(methodInfo, invocationObject, arguments);
		}

		public static IAstRefOrValue ReadArgumentRV(int argumentIndex, Type argumentType)
		{
			return argumentType.IsValueType
						? (IAstRefOrValue)new AstReadArgumentValue
													{
														argumentIndex = argumentIndex,
														argumentType = argumentType
													}
						: new AstReadArgumentRef
							{
								argumentIndex = argumentIndex,
								argumentType = argumentType
							};
		}

		public static IAstRefOrAddr ReadArgumentRA(int argumentIndex, Type argumentType)
		{
			return argumentType.IsValueType
						? (IAstRefOrAddr)new AstReadArgumentAddr
													{
														argumentIndex = argumentIndex,
														argumentType = argumentType
													}
						: new AstReadArgumentRef
							{
								argumentIndex = argumentIndex,
								argumentType = argumentType
							};
		}

		public static IAstRefOrValue ReadArrayItemRV(IAstRef array, int index)
		{
			return array.itemType.IsValueType
						? (IAstRefOrValue)new AstReadArrayItemValue
													{
														array = array,
														index = index
													}
						: new AstReadArrayItemRef
							{
								array = array,
								index = index
							};
		}

		public static IAstRefOrAddr ReadArrayItemRA(IAstRef array, int index)
		{
			return array.itemType.IsValueType
						? (IAstRefOrAddr)new AstReadArrayItemAddr
													{
														array = array,
														index = index
													}
						: new AstReadArrayItemRef
							{
								array = array,
								index = index
							};
		}

		public static IAstRefOrValue ReadFieldRV(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
		{
			return fieldInfo.FieldType.IsValueType
						? (IAstRefOrValue)new AstReadFieldValue
													{
														fieldInfo = fieldInfo,
														sourceObject = sourceObject
													}
						: new AstReadFieldRef
							{
								fieldInfo = fieldInfo,
								sourceObject = sourceObject
							};
		}

		public static IAstRefOrValue CastClass(IAstRefOrValue value, Type targetType)
		{
			return targetType.IsValueType
						? (IAstRefOrValue)new AstCastclassValue(value, targetType)
						: new AstCastclassRef(value, targetType);
		}

		public static IAstRefOrAddr ReadFieldRA(IAstRefOrAddr sourceObject, FieldInfo fieldInfo)
		{
			return fieldInfo.FieldType.IsValueType
						? (IAstRefOrAddr)new AstReadFieldAddr
													{
														fieldInfo = fieldInfo,
														sourceObject = sourceObject
													}
						: new AstReadFieldRef
							{
								fieldInfo = fieldInfo,
								sourceObject = sourceObject
							};
		}

		[Obsolete("Use RoV extension method.")]
		public static IAstRefOrValue ReadLocalRV(LocalBuilder loc)
		{
			return loc.LocalType.IsValueType
						? (IAstRefOrValue)new AstReadLocalValue
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
		[Obsolete("Use RoA extension method.")]
		public static IAstRefOrAddr ReadLocalRA(LocalBuilder loc)
		{
			return loc.LocalType.IsValueType
						? (IAstRefOrAddr)new AstReadLocalAddr(loc)
						: new AstReadLocalRef
							{
								localType = loc.LocalType,
								localIndex = loc.LocalIndex
							};
		}

		public static IAstRefOrAddr ReadPropertyRA(IAstRefOrAddr sourceObject, PropertyInfo propertyInfo)
		{
			return propertyInfo.PropertyType.IsValueType
						? (IAstRefOrAddr)new
													AstValueToAddr(
													new AstReadPropertyValue
														{
															sourceObject = sourceObject,
															propertyInfo = propertyInfo
														}
													)
						: new AstReadPropertyRef
							{
								sourceObject = sourceObject,
								propertyInfo = propertyInfo
							};
		}

		public static IAstRefOrValue ReadPropertyRV(IAstRefOrAddr sourceObject, PropertyInfo propertyInfo)
		{
			return propertyInfo.PropertyType.IsValueType
						? (IAstRefOrValue)new AstReadPropertyValue
													{
														sourceObject = sourceObject,
														propertyInfo = propertyInfo
													}
						: new AstReadPropertyRef
							{
								sourceObject = sourceObject,
								propertyInfo = propertyInfo
							};
		}

		public static IAstRefOrAddr ReadThis(Type thisType)
		{
			return thisType.IsValueType
						? (IAstRefOrAddr)new AstReadThisAddr
													{
														thisType = thisType
													}
						: new AstReadThisRef
							{
								thisType = thisType
							};
		}

		//public static IAstRefOrValue ReadMembersChain(MappingOperationsProcessor processor, IAstRefOrAddr sourceObject, MemberDescriptor descriptor)
		//{
		//   return ReadMembersChain(sourceObject, descriptor.MembersChain, descriptor.GetArguments(processor));
		//}
		public static IAstRefOrValue ReadMembersChain(IAstRefOrAddr sourceObject, MemberInfo[] membersChain)
		{
			return ReadMembersChain(sourceObject, membersChain, null);
		}

		public static IAstRefOrValue ReadMembersChain(IAstRefOrAddr sourceObject, MemberInfo[] membersChain, List<IAstStackItem> arguments)
		{
			IAstRefOrAddr src = sourceObject;
			IAstRefOrValue result;

			for (int i = 0; i < membersChain.Length - 1; ++i)
			{
				src = ReadMemberRA(src, membersChain[i], null);
			}
			result = ReadMemberRV(src, membersChain[membersChain.Length - 1], arguments);
			return result;
		}

		public static IAstStackItem ReadMember(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
		{
			return ReadMember(sourceObject, memberInfo, null);
		}
		public static IAstStackItem ReadMember(IAstRefOrAddr sourceObject, MemberInfo memberInfo, List<IAstStackItem> arguments)
		{
			switch (memberInfo.MemberType)
			{
				case MemberTypes.Method:
					{
						MethodInfo methodInfo = GetMethodInfo(memberInfo.DeclaringType, memberInfo.Name, arguments);
						if (methodInfo == null || methodInfo.ReturnType == null || methodInfo.ReturnType == typeof(void))
						{
							throw new EmitLibException("Invalid member:" + memberInfo.Name);
						}
						return CallMethod(methodInfo, sourceObject, arguments);
						//MethodInfo methodInfo = memberInfo.DeclaringType.GetMethod(memberInfo.Name);
						//if (methodInfo.ReturnType == null)
						//{
						//   throw new EmitMapperException("Invalid member:" + memberInfo.Name);
						//}
						//if (methodInfo.GetParameters().Length > 0)
						//{
						//   throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");
						//}
						//return (IAstRef)CallMethod(methodInfo, sourceObject, null);
					}
				case MemberTypes.Field:
					return AstBuildHelper.ReadFieldRA(sourceObject, (FieldInfo)memberInfo);
				default:
					return (IAstRef)AstBuildHelper.ReadPropertyRV(sourceObject, (PropertyInfo)memberInfo);
			}
		}
		public static IAstRefOrAddr ReadMemberRA(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
		{
			return ReadMemberRA(sourceObject, memberInfo, null);
		}
		public static IAstRefOrAddr ReadMemberRA(IAstRefOrAddr sourceObject, MemberInfo memberInfo, List<IAstStackItem> arguments)
		{
			switch (memberInfo.MemberType)
			{
				case MemberTypes.Method:
					{
						MethodInfo methodInfo = GetMethodInfo(memberInfo.DeclaringType, memberInfo.Name, arguments);
						if (methodInfo == null || methodInfo.ReturnType == null || methodInfo.ReturnType == typeof(void))
						{
							throw new EmitLibException("Invalid member:" + memberInfo.Name);
						}
						if (methodInfo.ReturnType.IsValueType)
						{
							throw new EmitLibException("Method " + memberInfo.Name + " should return a reference");
						}
						return (IAstRef)CallMethod(methodInfo, sourceObject, arguments);
						//MethodInfo methodInfo = memberInfo.DeclaringType.GetMethod(memberInfo.Name);
						//if (methodInfo.ReturnType == null)
						//{
						//   throw new EmitMapperException("Invalid member:" + memberInfo.Name);
						//}
						//if (methodInfo.GetParameters().Length > 0)
						//{
						//   throw new EmitMapperException("Method " + memberInfo.Name + " should not have parameters");
						//}
						//if (methodInfo.ReturnType == null || methodInfo.ReturnType.IsValueType)
						//{
						//   throw new EmitMapperException("Method " + memberInfo.Name + " should return a reference");
						//}
						//return (IAstRef)CallMethod(methodInfo, sourceObject, null);
					}
				case MemberTypes.Field:
					return AstBuildHelper.ReadFieldRA(sourceObject, (FieldInfo)memberInfo);
				default:
					{
						var pi = (PropertyInfo)memberInfo;
						if (pi.PropertyType.IsValueType)
						{
							return AstBuildHelper.ReadPropertyRA(sourceObject, (PropertyInfo)memberInfo);
						}
						return (IAstRef)AstBuildHelper.ReadPropertyRV(sourceObject, (PropertyInfo)memberInfo);
					}
			}
		}

		public static IAstRefOrValue ReadMemberRV(IAstRefOrAddr sourceObject, MemberInfo memberInfo)
		{
			return ReadMemberRV(sourceObject, memberInfo, null);
		}

		public static IAstRefOrValue ReadMemberRV(IAstRefOrAddr sourceObject, MemberInfo memberInfo, List<IAstStackItem> arguments)
		{
			switch (memberInfo.MemberType)
			{
				case MemberTypes.Method:
					{
						MethodInfo methodInfo = GetMethodInfo(memberInfo.DeclaringType, memberInfo.Name, arguments);
						if (methodInfo == null || methodInfo.ReturnType == null || methodInfo.ReturnType == typeof(void))
						{
							throw new EmitLibException("Invalid member:" + memberInfo.Name);
						}
						return CallMethod(methodInfo, sourceObject, arguments);
					}
				case MemberTypes.Field:
					return ReadFieldRV(sourceObject, (FieldInfo)memberInfo);
				default:
					return ReadPropertyRV(sourceObject, (PropertyInfo)memberInfo);
			}
		}

		//public static IAstNode WriteMembersChain(MappingOperationsProcessor processor, MemberDescriptor descriptor, IAstRefOrAddr targetObject, IAstRefOrValue value)
		//{
		//   return WriteMembersChain(descriptor.MembersChain, targetObject, value, descriptor.GetArguments(processor, value));
		//}
		public static IAstNode WriteMembersChain(MemberInfo[] membersChain, IAstRefOrAddr targetObject, IAstRefOrValue value)
		{
			return WriteMembersChain(membersChain, targetObject, value, null);
		}

		public static IAstNode WriteMembersChain(MemberInfo[] membersChain, IAstRefOrAddr targetObject, IAstRefOrValue value, List<IAstStackItem> arguments)
		{
			if (membersChain.Length == 1)
			{
				return WriteMember(membersChain[0], targetObject, value, arguments);
			}

			IAstRefOrAddr readTarget = targetObject;

			for (int i = 0; i < membersChain.Length - 1; ++i)
			{
				readTarget = ReadMemberRA(readTarget, membersChain[i], null);
			}
			return WriteMember(membersChain[membersChain.Length - 1], readTarget, value, arguments);
		}

		public static IAstNode WriteMember(MemberInfo memberInfo, IAstRefOrAddr targetObject, IAstRefOrValue value, List<IAstStackItem> arguments)
		{
			switch (memberInfo.MemberType)
			{
				case MemberTypes.Method:
					{
						MethodInfo methodInfo = GetMethodInfo(memberInfo.DeclaringType, memberInfo.Name, arguments);
						if (methodInfo == null)
						{
							throw new EmitLibException("Invalid member:" + memberInfo.Name);
						}
						return CallMethod(methodInfo, targetObject, arguments);
					}
				case MemberTypes.Field:
					return new AstWriteField
					       	{
					       		fieldInfo = (FieldInfo)memberInfo,
					       		targetObject = targetObject,
					       		value = value
					       	};
				default:
					break;
			}
			return new AstWriteProperty(targetObject, value, (PropertyInfo)memberInfo);
		}

		public static MethodInfo GetMethodInfo(Type type, string name, List<IAstStackItem> arguments)
		{
			return type.GetMethod(name, arguments != null ? arguments.Select(a => a.itemType).ToArray() : new Type[] { });
		}
	}
}