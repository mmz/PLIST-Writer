using System;
using System.Reflection;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	public class AstReadProperty : IAstRefOrValue
	{
		public IAstRefOrAddr sourceObject;
		public PropertyInfo propertyInfo;

		public Type itemType
		{
			get
			{
				return propertyInfo.PropertyType;
			}
		}

		public virtual void Compile(ICompilationContext context)
		{
			MethodInfo mi = propertyInfo.GetGetMethod();

			if (mi == null)
			{
				throw new Exception("Property " + propertyInfo.Name + " doesn't have get accessor");
			}

			AstBuildHelper.CallMethod(mi, sourceObject, null).Compile(context);
		}
	}

	public class AstReadPropertyRef : AstReadProperty, IAstRef
	{
		public override void Compile(ICompilationContext context)
		{
			CompilationHelper.CheckIsRef(itemType);
			base.Compile(context);
		}
	}

	public class AstReadPropertyValue : AstReadProperty, IAstValue
	{
		public override void Compile(ICompilationContext context)
		{
			CompilationHelper.CheckIsValue(itemType);
			base.Compile(context);
		}
	}
}