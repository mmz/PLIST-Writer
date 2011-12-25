using System;
using System.Reflection.Emit;
using EmitLib.AST.Interfaces;
using EmitLib.AST.Helpers;

namespace EmitLib.AST.Nodes
{
    class AstReturn : IAstNode, IAstAddr
    {
        public Type ReturnType;
        public IAstRefOrValue ReturnValue;

        public void Compile(CompilationContext context)
        {
            ReturnValue.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, ReturnType, ReturnValue.itemType);
            context.Emit(OpCodes.Ret);
        }

        public Type itemType
        {
            get { return ReturnType; }
        }
    }
}