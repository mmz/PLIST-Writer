using System.Reflection.Emit;
using EmitLib.AST.Interfaces;

namespace EmitLib.AST.Nodes
{
	class AstIf : IAstNode
	{
		public IAstRefOrValue condition;
		public IAstNode trueBranch;
		public IAstNode falseBranch;

		#region IAstNode Members

		public virtual void Compile(CompilationContext context)
		{
			condition.Compile(context);
			if (falseBranch == null)
				CompileIfNoElse(context);
			else if (trueBranch == null)
				CompileElseNoIf(context);
			else
				CompileIfAndElse(context);
		}

		protected void CompileIfAndElse(CompilationContext context)
		{
			Label elseLabel = context.ilGenerator.DefineLabel();
			Label endIfLabel = context.ilGenerator.DefineLabel();

			//if(
			context.Emit(OpCodes.Brfalse, elseLabel);
			//){
			trueBranch.Compile(context);
			//}
			context.Emit(OpCodes.Br, endIfLabel);
			//else
			context.ilGenerator.MarkLabel(elseLabel);
			//{
			falseBranch.Compile(context);
			//}
			context.ilGenerator.MarkLabel(endIfLabel);
		}

		protected void CompileIfNoElse(CompilationContext context)
		{
			Label endIfLabel = context.ilGenerator.DefineLabel();

			//if(
			context.Emit(OpCodes.Brfalse, endIfLabel);
			//){
			trueBranch.Compile(context);
			//}
			context.ilGenerator.MarkLabel(endIfLabel);
		}

		protected void CompileElseNoIf(CompilationContext context)
		{
			Label endIfLabel = context.ilGenerator.DefineLabel();

			//if(!
			context.Emit(OpCodes.Brtrue, endIfLabel);
			//){
			falseBranch.Compile(context);
			//}
			context.ilGenerator.MarkLabel(endIfLabel);
		}

		#endregion
	}
}