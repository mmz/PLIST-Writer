using System;

namespace EmitLib.AST
{
// ReSharper disable InconsistentNaming
    class ILCompilationException:Exception
// ReSharper restore InconsistentNaming
    {
        public ILCompilationException(string message)
            : base(message)
        {
        }

        public ILCompilationException(string message, params object[] p)
            : base(String.Format(message, p))
        {
        }

    }
}