using System;

namespace EmitLib.AST.Interfaces
{
    interface IAstStackItem: IAstNode
    {
        Type itemType { get; }
    }
}