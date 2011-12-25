﻿using System;
using System.Reflection.Emit;
using EmitLib.AST.Interfaces;

namespace EmitLib.AST.Nodes
{
	class AstConstantInt32 : IAstValue
	{
		public Int32 value;

		#region IAstReturnValueNode Members

		public Type itemType
		{
			get { return typeof(Int32); }
		}

		#endregion

		#region IAstNode Members

		public void Compile(CompilationContext context)
		{
			context.Emit(OpCodes.Ldc_I4, value);
		}

		#endregion
	}
}