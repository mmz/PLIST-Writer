using System;
using System.Collections.Generic;
using System.Text;

namespace EmitLib.AST.Interfaces
{
    interface IAstRef : IAstRefOrValue, IAstRefOrAddr
    {
    }
}