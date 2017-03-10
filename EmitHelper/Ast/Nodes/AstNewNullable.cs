using System;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	public class AstNewNullable: IAstValue
	{
		private Type _nullableType;
		public Type itemType
		{
			get 
			{
				return _nullableType;
			}
		}
		public void Compile(ICompilationContext context)
		{
			throw new NotImplementedException();
		}
	}
}
