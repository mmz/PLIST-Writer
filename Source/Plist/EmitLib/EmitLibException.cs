using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EmitLib
{
	public class EmitLibException : ApplicationException
	{
		public EmitLibException()
		{
		}

		public EmitLibException(string message)
			: base(message)
		{
		}

		public EmitLibException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

	}
}
