﻿using System;

namespace EmitHelper.Ast
{
    public class ILCompilationException:Exception
    {
        public ILCompilationException(string message)
            : base(message)
        {
        }

        public ILCompilationException(string message, params object[] p)
            : base(string.Format(message, p))
        {
        }

    }
}