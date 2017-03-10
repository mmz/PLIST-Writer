using System;
using System.Collections;

namespace Plist.Writers
{
	public class EnumerableWriter : TypeWriterBase
	{
		private readonly Lazy<TypeWriterBase> _valueWriter;

		public EnumerableWriter(Func<TypeWriterBase> createWriter)
		{
			_valueWriter = new Lazy<TypeWriterBase>(createWriter);
		}

		protected override void WriteImpl(PlistWriter writer, object obj)
		{
			if (obj == null)
				return;
			WriteValue(writer, obj);
		}

		protected override void WriteImpl(PlistWriter writer, object obj, string key)
		{
			if (obj == null)
				return;
			writer.WriteKey(key);
			WriteValue(writer, obj);
		}

		private void WriteValue(PlistWriter writer, object obj)
		{
			writer.WriteArrayStartElement();
			foreach (var value in (IEnumerable)obj)
			{
				if (value == null)
					continue;
				_valueWriter.Value.Write(writer, value);
			}
			writer.WriteEndElement();
		}
	}
}