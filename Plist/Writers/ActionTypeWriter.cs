using System;

namespace Plist.Writers
{
	public class ActionTypeWriter : TypeWriterBase
	{
		private readonly Action<PlistWriter, object> _write;

		public ActionTypeWriter(Action<PlistWriter, object> write)
		{
			_write = write;
		}

		protected override void WriteImpl(PlistWriter writer, object obj)
		{
			if (obj != null)
				_write(writer, obj);
		}

		protected override void WriteImpl(PlistWriter writer, object obj, string key)
		{
			if (obj == null) return;
			writer.WriteKey(key);
			_write(writer, obj);
		}
	}
}