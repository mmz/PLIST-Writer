namespace EmitHelper.Ast.Interfaces
{

	/// <summary>
	/// Any AST node.
	/// </summary>
	public interface IAstNode
	{
		void Compile(ICompilationContext context);
	}
}