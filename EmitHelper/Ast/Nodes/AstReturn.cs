using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
    public class AstReturn : IAstNode, IAstAddr
    {
        public Type returnType;
        public IAstRefOrValue returnValue;

        public void Compile(ICompilationContext context)
        {
            returnValue.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, returnType, returnValue.itemType);
            context.Emit(OpCodes.Ret);
        }

        public Type itemType
        {
            get { return returnType; }
        }
    }
}