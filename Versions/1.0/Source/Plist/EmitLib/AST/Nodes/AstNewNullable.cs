using System;
using EmitLib.AST.Interfaces;

namespace EmitLib.AST.Nodes
{
	class AstNewNullable: IAstValue
	{
		private Type _nullableType;
		public Type itemType
		{
			get 
			{
				return _nullableType;
			}
		}
		public void Compile(CompilationContext context)
		{
			throw new NotImplementedException();
		}
	}
}
