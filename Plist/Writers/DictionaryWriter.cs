using System;
using System.Collections;
using System.Globalization;

namespace Plist.Writers
{
	public class DictionaryWriter : TypeWriterBase
	{
		private readonly Lazy<TypeWriterBase> _valueWriter;

		public DictionaryWriter(Func<TypeWriterBase> createWriter)
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
			writer.WriteDictionaryStartElement();

			var dictionaryEnumerator = ((IDictionary)obj).GetEnumerator();
			while (dictionaryEnumerator.MoveNext())
			{
				if (dictionaryEnumerator.Value == null)
					continue;
				_valueWriter.Value.Write(writer, dictionaryEnumerator.Value, Convert.ToString(dictionaryEnumerator.Key, CultureInfo.InvariantCulture));
			}
			writer.WriteEndElement();
		}
	}
}