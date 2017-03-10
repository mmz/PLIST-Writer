using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	public class AstReadLocal : IAstStackItem
	{
		public int localIndex;
		public Type localType;

		public Type itemType
		{
			get
			{
				return localType;
			}
		}

		public AstReadLocal()
		{
		}

		public AstReadLocal(LocalBuilder loc)
		{
			localIndex = loc.LocalIndex;
			localType = loc.LocalType;
		}

		public virtual void Compile(ICompilationContext context)
		{
			context.Emit(OpCodes.Ldloc, localIndex);
		}
	}

	public class AstReadLocalRef : AstReadLocal, IAstRef
	{
		public AstReadLocalRef() { }

		public AstReadLocalRef(LocalBuilder localBuilder):base(localBuilder){ }

		public override void Compile(ICompilationContext context)
		{
			CompilationHelper.CheckIsRef(itemType);
			base.Compile(context);
		}
	}

	public class AstReadLocalValue : AstReadLocal, IAstValue
	{
		public AstReadLocalValue(){}

		public AstReadLocalValue(LocalBuilder localBuilder):base(localBuilder){}

		public override void Compile(ICompilationContext context)
		{
			CompilationHelper.CheckIsValue(itemType);
			base.Compile(context);
		}
	}

	public class AstReadLocalAddr : AstReadLocal, IAstAddr
	{
		public AstReadLocalAddr(LocalBuilder loc)
		{
			localIndex = loc.LocalIndex;
			localType = loc.LocalType;//.MakeByRefType();
		}

		public override void Compile(ICompilationContext context)
		{
			//CompilationHelper.CheckIsValue(itemType);
			context.Emit(OpCodes.Ldloca, localIndex);
		}
	}
}