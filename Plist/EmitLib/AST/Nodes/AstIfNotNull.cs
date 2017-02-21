using EmitLib.AST;
using EmitLib.AST.Helpers;
using EmitLib.AST.Interfaces;
using EmitLib.AST.Nodes;
using EmitLib.Utils;

namespace Plist.EmitLib.AST.Nodes
{
	/// <summary>
	/// Calls the TrueBranch if the Condition is true, not null, or non-zero, otherwise calls the FalseBranch.
	/// </summary>
	class AstIfNotNull : AstIf
	{
		public override void Compile(CompilationContext context)
		{
			if (!(condition is IAstRef) && !ReflectionUtils.IsNullable(condition.itemType))
			#region Non-nullable value-type
			{
				trueBranch.Compile(context);
				return;
			}
			#endregion
			else if (ReflectionUtils.IsNullable(condition.itemType))
			#region Nullable value-type;
			{

				AstBuildHelper.ReadPropertyRV(
					new AstValueToAddr(condition.ToValue(condition.itemType)),
					condition.itemType.GetProperty("HasValue")
				).Compile(context);
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
