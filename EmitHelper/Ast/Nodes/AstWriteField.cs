using System.Reflection;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{

	/// <summary>
	/// Replaces the value of a specified field with a specified value.
	/// </summary>
	public class AstWriteField: IAstNode
    {
        public IAstRefOrAddr targetObject;
        public IAstRefOrValue value;
        public FieldInfo fieldInfo;

        public void Compile(ICompilationContext context)
        {
            targetObject.Compile(context);
            value.Compile(context);
            CompilationHelper.PrepareValueOnStack(context, fieldInfo.FieldType, value.itemType);
            context.Emit(OpCodes.Stfld, fieldInfo);
        }
    }
}