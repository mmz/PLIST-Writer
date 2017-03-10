using System;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
    public class AstReadArrayItem : IAstStackItem
    {
        public IAstRef array;
        public int index;

        public Type itemType
        {
            get
            {
                return array.itemType.GetElementType();
            }
        }

        public virtual void Compile(ICompilationContext context)
        {
            array.Compile(context);
            context.Emit(OpCodes.Ldc_I4, index);
            context.Emit(OpCodes.Ldelem, itemType);
        }
    }

    public class AstReadArrayItemRef : AstReadArrayItem, IAstRef
    {
        public override void Compile(ICompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }

    public class AstReadArrayItemValue: AstReadArrayItem, IAstValue
    {
        public override void Compile(ICompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            base.Compile(context);
        }
    }

    public class AstReadArrayItemAddr : AstReadArrayItem, IAstAddr
    {
        public override void Compile(ICompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            array.Compile(context);
            context.Emit(OpCodes.Ldc_I4, index);
            context.Emit(OpCodes.Ldelema, itemType);
        }
    }
}