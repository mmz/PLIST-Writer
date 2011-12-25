using System;
using System.Reflection.Emit;
using EmitLib.AST.Interfaces;
using EmitLib.AST.Helpers;

namespace EmitLib.AST.Nodes
{
    abstract class AstIndirectRead : IAstStackItem
    {
        public Type ArgumentType;

        public Type itemType
        {
            get
            {
                return ArgumentType;
            }
        }

        public abstract void Compile(CompilationContext context);
    }

    class AstIndirectReadRef : AstIndirectRead, IAstRef
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            context.Emit(OpCodes.Ldind_Ref, itemType);
        }
    }

    class AstIndirectReadValue : AstIndirectRead, IAstValue
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            if (itemType == typeof(Int32))
            {
                context.Emit(OpCodes.Ldind_I4);
            }
            else
            {
                throw new Exception("Unsupported type");
            }
        }
    }

    class AstIndirectReadAddr : AstIndirectRead, IAstAddr
    {
        override public void Compile(CompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
        }
    }
}