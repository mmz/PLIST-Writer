using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Calls the TrueBranch if the Condition is true, not null, or non-zero, otherwise calls the FalseBranch.
	/// </summary>
	public class AstIfNotNull : AstIf
	{
		public override void Compile(ICompilationContext context)
		{

			if (!(condition is IAstRef) && !ReflectionUtils.IsNullable(condition.itemType))
			#region Non-nullable value-type
			{
				trueBranch?.Compile(context);
				return;
			}
			#endregion
			else if (ReflectionUtils.IsNullable(condition.itemType))
			#region Nullable value-type;

			{
				var roa = condition as IAstAddr;
				if (roa != null)
				{
					AstBuildHelper.ReadPropertyRV(
						roa,
						roa.itemType.GetProperty("HasValue")
					).Compile(context);
				}
				else
				{
					AstBuildHelper.ReadPropertyRV(
						new AstValueToAddr((IAstValue)condition),
						condition.itemType.GetProperty("HasValue")
					).Compile(context);
				}
			}
			#endregion
			else
			#region Reference-type
			{
				condition.Compile(context);
			}
			#endregion


			if (falseBranch == null)
				CompileIfNoElse(context);
			else if (trueBranch == null)
				CompileElseNoIf(context);
			else
				CompileIfAndElse(context);
		}
	}
}
