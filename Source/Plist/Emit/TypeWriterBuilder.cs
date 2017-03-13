using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EmitHelper;
using EmitHelper.Ast;
using EmitHelper.Ast.Interfaces;
using EmitHelper.Ast.Nodes;
using EmitHelper.Extensions;
using Plist.Writers;

namespace Plist.Emit
{
	class TypeWriterBuilder
	{

		private readonly Type _objectType;
		private readonly TypeBuilder _typeBuilder;
		private readonly FieldInfo _soFld;
		private readonly FieldInfo _tFld;
		private List<object> _constructorParam;
		private readonly Type _actualType;

		public TypeWriterBuilder(Type objectType, TypeBuilder typeBuilder)
		{
			_objectType = objectType;
			_typeBuilder = typeBuilder;
			_tFld = typeof(TypeWriterBase).GetField("ObjectType", BindingFlags.NonPublic | BindingFlags.Instance);
			_soFld = typeof(TypeWriterBase).GetField("SupliedObjects", BindingFlags.NonPublic | BindingFlags.Instance);
			var isNullable = _objectType.IsNullable();
			_actualType =
						isNullable
							? Nullable.GetUnderlyingType(_objectType)
							: _objectType;

		}
		public TypeWriterBase CreateInstance()
		{

			_constructorParam = new List<object>();
			BuildWriteMethod(false);
			BuildWriteMethod(true);
			BuildConstructor();

			return
				(TypeWriterBase)
				Activator.CreateInstance(_typeBuilder.CreateType(), new object[] { _actualType, _constructorParam.ToArray() });
		}

		private void BuildWriteMethod(bool withKey)
		{
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(
				"WriteImpl",
				MethodAttributes.FamORAssem | MethodAttributes.Virtual,
				typeof(void),
				withKey ? new[] { typeof(PlistWriter), typeof(object), typeof(string) } : new[] { typeof(PlistWriter), typeof(object) }
				);

			var ilGen = methodBuilder.GetILGenerator();
			var compilationContext = new CompilationContext(ilGen);

			var writerAst = new AstComplexNode();

			var mi = GetBoxedMethodInfo(_actualType);
			writerAst.nodes.AddRange(mi != null
												? BuildWriterWithMethod(mi, withKey)
												: BuildWriteImplMethodForClass(compilationContext, withKey));
			writerAst.Compile(compilationContext);
		}

		private IEnumerable<IAstNode> BuildWriterWithMethod(MethodInfo mi, bool withKey)
		{
			var writerRef = Ast.ArgRef(1, typeof(PlistWriter));
			if (withKey)
				yield return writerRef.CallVoid("WriteKey", Ast.ArgRef(3, typeof(string)));
			yield return mi.CallVoid(writerRef, Ast.Arg(2, typeof(object)));

			yield return Ast.RetVoid;
		}

		private IEnumerable<IAstNode> BuildWriteImplMethodForClass(ICompilationContext compilationContext, bool withKey)
		{
			var writerRef = Ast.ArgRef(1, typeof(PlistWriter));
			var objArg = Ast.ArgRefOrAddr(2, _objectType);

			if (withKey)
				yield return writerRef.CallVoid("WriteKey", Ast.ArgRef(3, typeof(string)));

			//Write opening dict element
			yield return writerRef.CallVoid("WriteDictionaryStartElement");

			//
			//iterate through properties
			foreach (var property in _objectType.GetProperties().Where(p => !p.PlistIgnore()))
			{
				if (property.PropertyType == typeof(object))
				{
					writerRef.CallVoid("Write", Ast.Const(property.GetPlistKey()), objArg.ReadProp(property));
					continue;
				}
				LocalBuilder localP = null;
				var isPropNullable = property.IsNullable();

				if (!property.PropertyType.IsValueType || isPropNullable)
				{
					localP = compilationContext.DeclareLocal(property.PropertyType);
					yield return localP.Init();
					yield return localP.Write(objArg.ReadProp(property));
				}


				var tb = new List<IAstNode>
				{//Write key value
					writerRef.CallVoid("WriteKey", Ast.Const(property.GetPlistKey()))
				};


				var plw = property.GetPlistPropertyWriter();
				if (plw != null)
				#region Use custom writer from attribute //Use value writer from the property attribute
				{
					var idx = _constructorParam.Count;
					_constructorParam.Add(plw);
					tb.Add(
						Ast.This
							.FieldRef(_soFld)
							.ReadItemRef(idx)
							.Cast(typeof(PlistValueWriterAttribute))
							.CallVoid("WriteValue", writerRef, localP != null ? localP.AstLoad() : objArg.ReadProp(property))
						);
				}
				#endregion
				else if (property.PlistSerializableType())
				#region Use IPlistSerializable interface methods of the property type.
				{
					tb.Add(
						localP
							.AstLoadRef()
							.Cast(typeof(IPlistSerializable))
							.CallVoid("Write", writerRef)
						);
				}
				#endregion
				else if (isPropNullable)
				{
					var m = GetMethodInfo(property.GetUnderlyingType());
					tb.Add(
						m != null
							? m.CallVoid(
								writerRef,
								localP.AstLoadRefOrAddr().ReadPropVal(property.PropertyType.GetProperty("Value"))
							)
							: writerRef.CallVoid("Write", localP.AstLoad())
						);
				}
				else
				#region Find something suitable from base methods or use PlistWriter
				{
					var testM = GetMethodInfo(property.PropertyType);
					tb.Add(
						testM != null
							? testM.CallVoid(writerRef, localP != null ? localP.AstLoad() : objArg.ReadProp(property))
							: writerRef.CallVoid("Write", localP != null ? localP.AstLoad() : objArg.ReadProp(property))
					);
				}
				#endregion

				#region Check property value supression

				if (localP != null)
					yield return localP.AstLoadRefOrAddr().IfNotNull(Ast.Complex(tb), null);
				else
					yield return Ast.Complex(tb);

				#endregion

			}


			//Write closing element
			yield return writerRef.CallVoid("WriteEndElement");
			yield return Ast.RetVoid;

		}

		public void BuildConstructor()
		{
			var b = _typeBuilder.DefineConstructor(
					MethodAttributes.Public,
					CallingConventions.Standard,
					new[] { typeof(Type), typeof(object[]) }
				);
			var ilGen = b.GetILGenerator();
			var compilationContext = new CompilationContext(ilGen);
			compilationContext.Emit(OpCodes.Ldarg_0);
			compilationContext.Emit(OpCodes.Ldarg_1);
			compilationContext.Emit(OpCodes.Stfld, _tFld);
			compilationContext.Emit(OpCodes.Ldarg_0);
			compilationContext.Emit(OpCodes.Ldarg_2);
			compilationContext.Emit(OpCodes.Stfld, _soFld);
			compilationContext.Emit(OpCodes.Ret);
		}


		#region Base writer methods

		public static MethodInfo GetBoxedMethodInfo(Type objectType)
		{

			if (objectType.IsValueType)
				return CreateValueTypeWriteMethod(objectType);
			if (objectType.IsEnum)
				return typeof(PlistWriter).GetMethod("WriteEnum", new[] { typeof(object) });
			if (objectType == typeof(string))
				return typeof(PlistWriter).GetMethod("WriteString", new[] { typeof(object) });
			if (objectType.IsArray && objectType.HasElementType && objectType.GetElementType() == typeof(byte))
				return typeof(PlistWriter).GetMethod("WriteData", new[] { typeof(byte[]) });
			return null;
		}
		private static MethodInfo CreateValueTypeWriteMethod(Type objectType)
		{
			if (objectType.IsPrimitive)
			// Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, Char, Double, Single. 
			{
				if (typeof(Boolean) == objectType)
					return typeof(PlistWriter).GetMethod("WriteBoolean", new[] { typeof(object) });
				if (typeof(Single) == objectType || typeof(Double) == objectType)
					return typeof(PlistWriter).GetMethod("WriteReal", new[] { typeof(object) });
				if (typeof(Char) == objectType)
					return typeof(PlistWriter).GetMethod("WriteString", new[] { typeof(object) });

				//if (typeof(Int16) == actualType|| typeof(Int32) == actualType|| typeof(Int64) == actualType|| typeof(UInt16) == actualType
				//|| typeof(UInt32) == actualType|| typeof(UInt64) == actualType|| typeof(Byte) == actualType|| typeof(SByte) == actualType)
				return typeof(PlistWriter).GetMethod("WriteInteger", new[] { typeof(object) });
			}

			if (typeof(Decimal) == objectType)
				return typeof(PlistWriter).GetMethod("WriteReal", new[] { typeof(object) });
			if (typeof(DateTime) == objectType)
				return typeof(PlistWriter).GetMethod("WriteDate", new[] { typeof(object) });

			return typeof(PlistWriter).GetMethod("WriteString", new[] { typeof(object) });
		}

		public static MethodInfo GetMethodInfo(Type objectType)
		{
			if (typeof(object) != objectType)
			{
				var method = typeof(PlistWriter).GetMethod("Write", new[] { objectType });
				if (method != null)
					return method;
			}
			if (objectType.IsValueType)
				return typeof(PlistWriter).GetMethod("WriteString", new[] { typeof(object) });
			return null;
		}

		#endregion

	}
}
