using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
	/// <summary>
	/// Stores the specified value in the specified local variable.
	/// </summary>
	public class AstWriteLocal : IAstNode
    {
        public int localIndex;
        public Type localType;
        public IAstRefOrValue value;

        public AstWriteLocal()
        {
        }

        public AstWriteLocal(LocalBuilder loc, IAstRefOrValue value)
        {
            localIndex = loc.LocalIndex;
            localType = loc.LocalType;
            this.value = value;
        }


        public void Compile(ICompilationContext context)
        {
            value.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, localType, value.itemType);
            context.Emit(OpCodes.Stloc, localIndex);
        }
    }
}