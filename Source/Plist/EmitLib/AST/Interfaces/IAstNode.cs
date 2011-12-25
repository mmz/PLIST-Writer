using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection.Emit;
using EmitLib.AST;

namespace EmitLib.AST.Interfaces
{
    interface IAstNode
    {
        void Compile(CompilationContext context);
    }
}