using System;

namespace EmitHelper.Ast.Interfaces
{

	/// <summary>
	/// AST Node representing stack item.
	/// </summary>
	public interface IAstStackItem : IAstNode
    {
		/// <summary>
		/// Тип данных.
		/// </summary>
        Type itemType { get; }
    }
}