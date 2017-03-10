using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Plist.Writers
{
	public class DataRowWriter : TypeWriterBase
	{
		protected override void WriteImpl(PlistWriter writer, object obj, string key)
		{
			WriteDataRow(writer, obj as DataRow, key);
		}

		protected override void WriteImpl(PlistWriter writer, object obj)
		{
			WriteDataRow(writer, obj as DataRow, null);
		}

		protected void WriteDataRow(PlistWriter writer, DataRow value, string key)
		{

			if (value == null)
				return;
			if (key != null)
				writer.WriteKey(key);

			var ms = GenerateRowSerializer(value.Table.Columns);
			writer.WriteDictionaryStartElement();
			foreach (var action in ms)
				action(writer, value);
			writer.WriteEndElement();

		}

		protected IEnumerable<Action<PlistWriter, DataRow>> GenerateRowSerializer(DataColumnCollection columns)
		{
			return
				from DataColumn column in columns
				let tWriter = CreateTypeWriter(column.DataType)
				let o = column.Ordinal
				let n = column.ColumnName
				select (Action<PlistWriter, DataRow>)
				((writer, row) =>
				{
					var value = row[o];
					if (value.Equals(DBNull.Value))
						return;
					tWriter.Write(writer, value, column.ColumnName);
				});
		}
	}
}