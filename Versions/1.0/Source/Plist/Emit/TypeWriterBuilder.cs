using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EmitLib;
using EmitLib.AST;
using EmitLib.AST.Interfaces;
using EmitLib.AST.Nodes;
using EmitLib.Utils;

namespace Plist.Emit
{
	class TypeWriterBuilder
	{

		private readonly Type _objectType;
		private readonly TypeBuilder _typeBuilder;
		private readonly FieldInfo _soFld;
		private readonly FieldInfo _tFld;
		private List<object> _constructorParam;
		private readonly bool _isNullable;
		private readonly Type _actualType;

		public TypeWriterBuilder(Type objectType, TypeBuilder typeBuilder)
		{
			_objectType = objectType;
			_typeBuilder = typeBuilder;
			_tFld = typeof(TypeWriterBase).GetField("ObjectType", BindingFlags.NonPublic | BindingFlags.Instance);
			_soFld = typeof(TypeWriterBase).GetField("SupliedObjects", BindingFlags.NonPublic | BindingFlags.Instance);
			_isNullable = ReflectionUtils.IsNullable(_objectType);
			_actualType =
						_isNullable
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

			var writerAst = new AstComplexNode();

			var mi = TypeWriterBase.GetMethodInfo(_actualType);
			writerAst.nodes.AddRange(mi != null
												? BuildWriterWithMethod(ilGen, mi, withKey)
												: BuildWriteImplMethodForClass(withKey));


			var compilationContext = new CompilationContext(ilGen);
			writerAst.Compile(compilationContext);
		}

		private IEnumerable<IAstNode> BuildWriteImplMethodForClass(bool withKey)
		{


			var writerRef = Ast.ArgR(1, typeof(PlistWriter));
			var objArg = Ast.ArgRoA(2, _objectType);

			//If value is null then return
			yield return Ast.IfFalseRetVoid(writerRef);

			//Write key if necessary
			if (withKey)
				yield return writerRef.CallVoid("WriteKey", Ast.ArgR(3, typeof(string)));

			//Write opening dict element
			yield return writerRef.CallVoid("WriteDictionaryStartElement");

			//iterate through properties
			foreach (var property in _objectType.GetProperties())
			{
				//Ignored properties
				if (property.GetCustomAttributes(typeof(PlistIgnoreAttribute), false).Length > 0)
					continue;

				var isPropNullable = ReflectionUtils.IsNullable(property.PropertyType);

				var tb = new List<IAstNode>
				{
					//Write key value
					writerRef.CallVoid("WriteKey", Ast.Const(property.GetPlistKey()))
				};


				var plw = property.GetCustomAttributes(typeof(PlistValueWriterAttribute), true).FirstOrDefault();
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
							.CallVoid("WriteValue", writerRef, objArg.ReadProp(property))
						);

				}
				#endregion
				else if (typeof(IPlistSerializable).IsAssignableFrom(property.PropertyType))
				#region Use IPlistSerializable interface methods of the property type.
				{
					tb.Add(
						objArg
							.ReadPropRef(property)
							.Cast(typeof(IPlistSerializable))
							.CallVoid("Write", writerRef)
						);
				}
				#endregion
				else
				#region Find something suitable from base methods or use PlistWriter
				{
					var testM = TypeWriterBase.GetMethodInfo(
						isPropNullable
						? Nullable.GetUnderlyingType(property.PropertyType)
						: property.PropertyType
					);
					tb.Add(
						testM != null
							? Ast.This.CallVoid(testM, writerRef, objArg.ReadProp(property))
							: writerRef.CallVoid("Write", objArg.ReadProp(property))
						);
				}
				#endregion


				#region Check property value supression

				if (!property.PropertyType.IsValueType)
					yield return
						Ast.If(
							objArg.ReadPropRef(property).NotNull(), Ast.Complex(tb)
						);
				else if (isPropNullable)
					yield return
						Ast.If(
							objArg.ReadPropVal(property).NotNull(), Ast.Complex(tb)
						);
				else
					yield return Ast.Complex(tb);

				#endregion

			}
			yield return writerRef.CallVoid("WriteEndElement");
			yield return Ast.RetVoid;
		}

		private IEnumerable<IAstNode> BuildWriterWithMethod(ILGenerator ilGen, MethodInfo mi, bool withKey)
		{
			var writerRef = Ast.ArgR(1, typeof(PlistWriter));
			var locObj = ilGen.DeclareLocal(typeof(object));
			yield return BuilderUtils.InitializeLocal(locObj, 2);

			var mc = Ast.Complex();

			//Write key if necessary
			if (withKey)
				mc.nodes.Add(writerRef.CallVoid("WriteKey", Ast.ArgR(3, typeof(string))));

			mc.nodes.Add(
				Ast.This.CallVoid(mi, writerRef, locObj.Ref())
				);

			#region Check value supression
			if (!_actualType.IsValueType)
			{
				yield return Ast.If(
					locObj.Ref().NotNull(), mc
					);
			}
			else if (_isNullable)
				yield return Ast.If(
						locObj.Val().NotNull(), mc
					);
			else
				yield return mc;
			#endregion

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
	}
}
