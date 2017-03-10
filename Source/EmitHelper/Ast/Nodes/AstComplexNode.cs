using System.Collections;
using System.Collections.Generic;
using System.Linq;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// List of AST nodes.
	/// </summary>
	public class AstComplexNode : IAstNode, IEnumerable<IAstNode>
	{
		public List<IAstNode> nodes = new List<IAstNode>();

		public void Compile(ICompilationContext context)
		{
			nodes.ForEach(n => n?.Compile(context));
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IEnumerator<IAstNode> GetEnumerator()
		{
			return nodes.Where(n => n != null).GetEnumerator();
		}
	}
}