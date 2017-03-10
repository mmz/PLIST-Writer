﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using EmitHelper.Ast.Interfaces;

namespace EmitHelper.Ast.Helpers
{
    class CompilationHelper
    {
        public static void EmitCall(
            ICompilationContext context,
            IAstRefOrAddr invocationObject,
            MethodInfo methodInfo,
            List<IAstStackItem> arguments)
        {
            if (arguments == null)
            {
                arguments = new List<IAstStackItem>();
            }

	        invocationObject?.Compile(context);

	        ParameterInfo[] args = methodInfo.GetParameters();
            if (args.Length != arguments.Count)
            {
                throw new Exception("Invalid method parameters count");
            }

            for (int i = 0; i < args.Length; ++i)
            {
                arguments[i].Compile(context);
                PrepareValueOnStack(context, args[i].ParameterType, arguments[i].itemType);
            }
            if (methodInfo.IsVirtual)
            {
                context.EmitCall(OpCodes.Callvirt, methodInfo);
            }
            else
            {
                context.EmitCall(OpCodes.Call, methodInfo);
            }
        }

        public static void PrepareValueOnStack(ICompilationContext context, Type desiredType, Type typeOnStack)
        {
            if (typeOnStack.IsValueType && !desiredType.IsValueType)
            {
                context.Emit(OpCodes.Box, typeOnStack);
            }
            else if (!typeOnStack.IsValueType && desiredType.IsValueType)
            {
                context.Emit(OpCodes.Unbox_Any, desiredType);
            }
            else if (desiredType != typeOnStack)
            {
                context.Emit(OpCodes.Castclass, desiredType);
            }
        }

        public static void CheckIsRef(Type type)
        {
            if (type.IsValueType)
            {
                throw new ILCompilationException("A reference type was expected, but it was: " + type);
            }
        }

        public static void CheckIsValue(Type type)
        {
            if (!type.IsValueType)
            {
                throw new ILCompilationException("A value type was expected, but it was: " + type);
            }
        }

    }
}