﻿using System.Collections.Generic;
using EmitLib.AST.Helpers;
using EmitLib.AST.Interfaces;
using EmitLib.AST.Nodes;
using System.Reflection.Emit;

namespace EmitLib
{
	class BuilderUtils
	{
		/// <summary>
		/// Copies an argument to local variable
		/// </summary>
		/// <param name="loc"></param>
		/// <param name="argIndex"></param>
		/// <returns></returns>
		public static IAstNode InitializeLocal(LocalBuilder loc, int argIndex)
		{
			return new AstComplexNode()
			{
				nodes =
					new List<IAstNode>()
					{
						new AstInitializeLocalVariable(loc),
						new AstWriteLocal()
						{
							localIndex = loc.LocalIndex,
							localType = loc.LocalType,
							value = AstBuildHelper.ReadArgumentRV(argIndex, typeof(object))
						}
					}
			};
		}
	}
}
