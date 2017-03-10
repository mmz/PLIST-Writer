using System.Collections.Generic;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;
using EmitHelper.Ast.Nodes;

namespace EmitHelper.Extensions
{
	public static class LocalBuilderExtensions
	{

		#region LocalBuilder

		public static IAstStackItem AstLoad(this LocalBuilder localBuilder)
		{
			return new AstReadLocal(localBuilder);
		}

		public static IAstValue AstLoadValue(this LocalBuilder localBuilder)
		{
			return new AstReadLocalValue(localBuilder);
		}
		public static IAstRef AstLoadRef(this LocalBuilder localBuilder)
		{
			return new AstReadLocalRef(localBuilder);
		}
		public static IAstRefOrAddr AstLoadRefOrAddr(this LocalBuilder localBuilder)
		{
			return localBuilder.LocalType.IsValueType
				? (IAstRefOrAddr)new AstReadLocalAddr(localBuilder)
				: new AstReadLocalRef(localBuilder);
		}
		public static IAstRefOrValue AstLoadRefOrValue(this LocalBuilder localBuilder)
		{
			return localBuilder.LocalType.IsValueType
				? (IAstRefOrValue)new AstReadLocalValue(localBuilder)
				: new AstReadLocalRef(localBuilder);
		}

		public static AstWriteLocal Write(this LocalBuilder local, IAstRefOrValue value)
		{
			return new AstWriteLocal() { localIndex = local.LocalIndex, localType = local.LocalType, value = value };
		}

		public static IAstNode Init(this LocalBuilder localBuilder)
		{
			return new AstInitializeLocalVariable(localBuilder);
		}
		public static IAstNode InitFromArgument(this LocalBuilder localBuilder, int argumentIndex)
		{

			return new AstComplexNode()
			{
				nodes =
					new List<IAstNode>()
					{
						new AstInitializeLocalVariable(localBuilder),
						new AstWriteLocal()
						{
							localIndex = localBuilder.LocalIndex,
							localType = localBuilder.LocalType,
							value = AstBuildHelper.ReadArgumentRV(argumentIndex, typeof(object))
						}
					}
			};
		}

		#endregion

	}
}
