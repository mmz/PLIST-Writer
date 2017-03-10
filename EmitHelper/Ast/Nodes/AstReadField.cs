using System;
using System.Reflection;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
    public class AstReadField: IAstStackItem
    {
        public IAstRefOrAddr sourceObject;
        public FieldInfo fieldInfo;

        public Type itemType
        {
            get
            {
                return fieldInfo.FieldType;
            }
        }

        public virtual void Compile(ICompilationContext context)
        {
            sourceObject.Compile(context);
            context.Emit(OpCodes.Ldfld, fieldInfo);
        }
    }

    public class AstReadFieldRef : AstReadField, IAstRef
    {
        public override void Compile(ICompilationContext context)
        {
            CompilationHelper.CheckIsRef(itemType);
            base.Compile(context);
        }
    }

    public class AstReadFieldValue : AstReadField, IAstValue
    {
        public override void Compile(ICompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            base.Compile(context);
        }
    }

    public class AstReadFieldAddr : AstReadField, IAstAddr
    {
        public override void Compile(ICompilationContext context)
        {
            CompilationHelper.CheckIsValue(itemType);
            sourceObject.Compile(context);
            context.Emit(OpCodes.Ldflda, fieldInfo);
        }
    }
}