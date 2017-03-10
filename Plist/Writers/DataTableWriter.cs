using System.Data;
using System.Linq;

namespace Plist.Writers
{
	public class DataTableWriter : DataRowWriter
	{
		protected override void WriteImpl(PlistWriter writer, object obj)
		{
			WriteDataTable(writer, obj as DataTable, null);
		}

		protected override void WriteImpl(PlistWriter writer, object obj, string key)
		{
			WriteDataTable(writer, obj as DataTable, key);
		}

		protected void WriteDataTable(PlistWriter writer, DataTable value, string key)
		{
			if (value == null)
				return;
			if (key != null)
				writer.WriteKey(key);

			writer.WriteArrayStartElement();
			var ms = GenerateRowSerializer(value.Columns).ToArray();

			foreach (DataRow row in value.Rows)
			{
				writer.WriteDictionaryStartElement();
				foreach (var action in ms)
					action(writer, row);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}
	}
}