using System;
using System.Collections.Generic;
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
			var locWriter = ilGen.DeclareLocal(typeof(PlistWriter));

			var writerAst = new AstComplexNode();

			writerAst.nodes.Add(BuilderUtils.InitializeLocal(locWriter, 1));

			var mi = TypeWriterBase.GetMethodInfo(_actualType);
			writerAst.nodes.AddRange(mi != null
												? BuildWriterWithMethod(ilGen, mi, locWriter, withKey)
												: BuildWriteImplMethodForClass(ilGen, locWriter, withKey));


			var compilationContext = new CompilationContext(ilGen);
			writerAst.Compile(compilationContext);
		}

		private IEnumerable<IAstNode> BuildWriteImplMethodForClass(ILGenerator ilGen, LocalBuilder locWriter, bool withKey)
		{
			var locObj = ilGen.DeclareLocal(_objectType);
			yield return BuilderUtils.InitializeLocal(locObj, 2);
			LocalBuilder locKey = null;
			if (withKey)
			{
				locKey = ilGen.DeclareLocal(typeof(string));
				yield return BuilderUtils.InitializeLocal(locKey, 3);
			}

			//If value is null then return
			if (!_objectType.IsValueType)
				yield return Ast.If(locObj.RoV().IsNull(), Ast.Complex(Ast.RetVoid));


			if (withKey)
				yield return locWriter.Ref().CallVoid("WriteKey", locKey.Ref());

			//Write opening dict element
			yield return Ast.CallVoid("WriteDictionaryStartElement", locWriter);

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
				         		locWriter.RoA().CallVoid("WriteKey", Ast.Const(property.GetPlistName()))
				         	};
				

				var plw = property.GetCustomAttributes(typeof(PlistValueWriterAttribute), true);
				if (plw.Length > 0)
				{
					#region Use custom writer from attribute
					var idx = _constructorParam.Count;
					_constructorParam.Add(plw[0]);
					tb.Add(
						Ast.This
							.FieldRef(_soFld)
							.ReadItemRef(idx)
							.Cast(typeof(PlistValueWriterAttribute))
							.CallVoid("WriteValue", locWriter.RoA(), locObj.RoA().ReadProp(property))
						);
					#endregion
				}
				else if (typeof(IPlistWritable).IsAssignableFrom(property.PropertyType))
				{
					tb.Add(
						locObj.RoA()
							.ReadPropRef(property)
							.Cast(typeof(IPlistWritable))
							.CallVoid("Write", locWriter.RoA())
						);
				}
				else
				{
					var testM = TypeWriterBase.GetMethodInfo(
						isPropNullable
						? Nullable.GetUnderlyingType(property.PropertyType)
						: property.PropertyType
					);

					tb.Add(
						testM != null
							? Ast.This.CallVoid(testM, locWriter.RoA(), locObj.RoA().ReadProp(property))
							: locWriter.Ref().CallVoid("Write", locObj.RoA().ReadProp(property))
						);
				}

				#region Check property value supression

				if (!property.PropertyType.IsValueType)
					yield return
						Ast.If(
							locObj.RoA().ReadPropRef(property).NotNull(), Ast.Complex(tb)
						);
				else if (isPropNullable)
					yield return
						Ast.If(
							locObj.RoA().ReadPropVal(property).NotNull(), Ast.Complex(tb)
						);
				else
					yield return Ast.Complex(tb);

				#endregion

			}
			yield return Ast.CallVoid("WriteEndElement", locWriter);
		}

		private IEnumerable<IAstNode> BuildWriterWithMethod(ILGenerator ilGen, MethodInfo mi, LocalBuilder locWriter, bool withKey)
		{
			var locObj = ilGen.DeclareLocal(typeof(object));
			yield return BuilderUtils.InitializeLocal(locObj, 2);

			var mc = Ast.Complex();
			if (withKey)
			{
				var locKey = ilGen.DeclareLocal(typeof(string));
				yield return BuilderUtils.InitializeLocal(locKey, 3);
				mc.nodes.Add(locWriter.RoA().CallVoid("WriteKey", locKey.Ref()));
			}
			mc.nodes.Add(
				Ast.This.CallVoid(mi, locWriter.RoA(), locObj.Ref())
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
