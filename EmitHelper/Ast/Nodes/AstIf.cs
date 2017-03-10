using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	public class AstIf : IAstNode
	{
		public IAstStackItem condition;
		public IAstNode trueBranch;
		public IAstNode falseBranch;
		public bool useShort = false;

		#region IAstNode Members

		public virtual void Compile(ICompilationContext context)
		{
			condition.Compile(context);
			if (falseBranch == null)
				CompileIfNoElse(context);
			else if (trueBranch == null)
				CompileElseNoIf(context);
			else
				CompileIfAndElse(context);
		}

		protected void CompileIfAndElse(ICompilationContext context)
		{
			Label elseLabel = context.DefineLabel();
			Label endIfLabel = context.DefineLabel();

			//if(
			context.Emit(useShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, elseLabel);
			//){
				trueBranch.Compile(context);
			//}
				context.Emit(useShort ? OpCodes.Br_S : OpCodes.Br, endIfLabel);
			//else
			context.MarkLabel(elseLabel);
			//{
				falseBranch.Compile(context);
			//}
			context.MarkLabel(endIfLabel);
		}

		protected void CompileIfNoElse(ICompilationContext context)
		{
			Label endIfLabel = context.DefineLabel();

			//if(
			context.Emit(useShort ? OpCodes.Brfalse_S : OpCodes.Brfalse, endIfLabel);
			//){
				trueBranch.Compile(context);
			//}
			context.MarkLabel(endIfLabel);
		}

		protected void CompileElseNoIf(ICompilationContext context)
		{
			Label endIfLabel = context.DefineLabel();

			//if(!
			context.Emit(useShort ? OpCodes.Brtrue_S : OpCodes.Brtrue, endIfLabel);
			//){
				falseBranch.Compile(context);
			//}
			context.MarkLabel(endIfLabel);
		}

		#endregion
	}
}