using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using EmitHelper.Ast.Helpers;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Nodes
{
    public class AstNewObject: IAstRef
    {
        public Type objectType;
        public IAstStackItem[] ConstructorParams;

        public AstNewObject()
        { 
        }

        public AstNewObject(Type objectType, IAstStackItem[] constructorParams)
        {
            this.objectType = objectType;
            this.ConstructorParams = constructorParams;
        }

		
        #region IAstStackItem Members

        public Type itemType
        {
            get 
            {
                return objectType; 
            }
        }

        #endregion

        #region IAstNode Members

        public void Compile(ICompilationContext context)
        {
			if (ReflectionUtils.IsNullable(objectType))
			{
				IAstRefOrValue underlyingValue;
				var underlyingType = Nullable.GetUnderlyingType(objectType);
                if (ConstructorParams == null || ConstructorParams.Length == 0)
				{
					LocalBuilder temp = context.DeclareLocal(underlyingType);
					new AstInitializeLocalVariable(temp).Compile(context);
					underlyingValue = AstBuildHelper.ReadLocalRV(temp);
				}
				else
				{
					underlyingValue = (IAstValue)ConstructorParams[0];
				}

				ConstructorInfo constructor = objectType.GetConstructor(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance, 
					null, 
					new[] { underlyingType }, 
					null);

				underlyingValue.Compile(context);
				context.EmitNewObject(constructor);
			}
			else
			{
				Type[] types;
				if (ConstructorParams == null || ConstructorParams.Length == 0)
				{
					types = new Type[0];
				}
				else
				{
					types = ConstructorParams.Select(c => c.itemType).ToArray();
					foreach (var p in ConstructorParams)
					{
						p.Compile(context);
					}
				}

				ConstructorInfo ci = objectType.GetConstructor(types);
				if (ci != null)
				{
					context.EmitNewObject(ci);
				}
                else if (objectType.IsValueType)
                {
                    LocalBuilder temp = context.DeclareLocal(objectType);
                    new AstInitializeLocalVariable(temp).Compile(context);
                    AstBuildHelper.ReadLocalRV(temp).Compile(context);
                }
                else
                {
                    throw new Exception(
                        String.Format("Constructor for types [{0}] not found in {1}", types.ToCSV(","), objectType.FullName)
                    );
                }
			}
        }

        #endregion
    }
}