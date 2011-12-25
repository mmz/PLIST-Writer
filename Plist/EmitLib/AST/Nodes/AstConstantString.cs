using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EmitLib.AST.Interfaces;
using System.Reflection.Emit;

namespace EmitLib.AST.Nodes
{
	class AstConstantString : IAstRef
	{
		public string str;

		#region IAstStackItem Members

		public Type itemType
		{
			get
			{
				return typeof(string);
			}
		}

		#endregion

		#region IAstNode Members

		public void Compile(CompilationContext context)
		{
			context.Emit(OpCodes.Ldstr, str);
		}

		#endregion
	}
}