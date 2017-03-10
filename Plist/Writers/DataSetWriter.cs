using System.Data;

namespace Plist.Writers
{
	public class DataSetWriter : DataTableWriter
	{
		protected override void WriteImpl(PlistWriter writer, object obj, string key)
		{
			WriteDataSet(writer, obj as DataSet, key);
		}

		protected override void WriteImpl(PlistWriter writer, object obj)
		{
			WriteDataSet(writer, obj as DataSet, null);
		}
		protected void WriteDataSet(PlistWriter writer, DataSet value, string key)
		{
			if (value == null)
				return;
			if (key != null)
				writer.WriteKey(key);

			writer.WriteArrayStartElement();
			foreach (DataTable table in value.Tables)
			{
				WriteDataTable(writer, table, null);
			}

			writer.WriteEndElement();
		}
	}
}