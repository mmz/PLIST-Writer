using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using Plist.Emit;

namespace Plist.Writers
{
	public abstract class TypeWriterBase
	{
#pragma warning disable 649
		// ReSharper disable once UnassignedReadonlyField
		protected readonly object[] SupliedObjects;
		// ReSharper disable once UnassignedReadonlyField
		protected readonly Type ObjectType;
#pragma warning restore 649
		/// <summary>
		/// Calls <paramref name="obj"/> type-specific writing strategy using <paramref name="writer"/>
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="obj"></param>
		public void Write(PlistWriter writer, object obj)
		{
			WriteImpl(writer, obj);
		}
		/// <summary>
		/// Calls <paramref name="obj"/> type-specific writing strategy using <paramref name="writer"/> with <paramref name="key"/>
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="obj"></param>
		/// <param name="key"></param>
		public void Write(PlistWriter writer, object obj, string key)
		{
			WriteImpl(writer, obj, key);
		}

		protected abstract void WriteImpl(PlistWriter writer, object obj);
		protected abstract void WriteImpl(PlistWriter writer, object obj, string key);


		#region Static
		public static TypeWriterBase CreateTypeWriter(Type objectType)
		{
			if (typeof(object) == objectType)
				return new ActionTypeWriter(
					(writer, obj) =>
					{
						var typeWriter = CreateTypeWriterImpl(obj.GetType());
						typeWriter.Write(writer, obj);
					});
			return CreateTypeWriterImpl(objectType);
		}

		private static TypeWriterBase CreateTypeWriterImpl(Type objectType)
		{
			return WriterStore.GetWriter(objectType, () => CreateWriterInstance(objectType));
		}
		private static TypeWriterBase CreateWriterInstance(Type objectType)
		{

			if (objectType.IsPlistSerializable())
				return new ActionTypeWriter((writer, obj) =>
				{
					((IPlistSerializable) obj).Write(writer);
				});

			var plw = objectType.GetPlistValueWriter();
			if (plw != null)
			{
				return new ActionTypeWriter((writer, obj) => { plw.WriteValue(writer, obj); });
			}

			if (objectType.PlistIgnore())
				return new ActionTypeWriter((writer, obj) => { });

			if (objectType.IsValueType || typeof(string) == objectType)
				return ConstructWriter(objectType);

			if (objectType.IsArray)
			{
				Type eType = typeof(object);
				return !objectType.HasElementType || (eType = objectType.GetElementType()) != typeof(byte)
							? CreateTypedEnumerableWriter(eType)
							: ConstructWriter(objectType);
			}
			if (objectType.IsGenericType && typeof(IDictionary<,>) == objectType.GetGenericTypeDefinition())
				return CreateTypedDictionaryWriter(objectType.GetGenericArguments()[1]);
			if (objectType.IsGenericType && typeof(IEnumerable<>) == objectType.GetGenericTypeDefinition())
				return CreateTypedEnumerableWriter(objectType.GetGenericArguments()[0]);


			if (typeof(IDictionary).IsAssignableFrom(objectType))
				return CreateTypedDictionaryWriter(typeof(object));
			if (typeof(IEnumerable).IsAssignableFrom(objectType))
				return CreateTypedEnumerableWriter(typeof(object));

			if (objectType == typeof(DataSet))
				return new DataSetWriter();
			if (objectType == typeof(DataTable))
				return new DataTableWriter();
			if (objectType == typeof(DataRow))
				return new DataRowWriter();

			return ConstructWriter(objectType);
		}

		private static TypeWriterBase ConstructWriter(Type objectType)
		{
			TypeBuilder typeBuilder = DynamicAssemblyHelper.DefineWriterType(objectType.FullName);

			var buider = new TypeWriterBuilder(
				objectType,
				typeBuilder
			);

			return buider.CreateInstance();
		}
		private static TypeWriterBase CreateTypedDictionaryWriter(Type valueType)
		{
			return new DictionaryWriter(() => CreateTypeWriter(valueType));
		}
		private static TypeWriterBase CreateTypedEnumerableWriter(Type elementType)
		{
			return new EnumerableWriter(() => CreateTypeWriter(elementType));
		}
		
		#endregion

		#region Data objects
		//public static void WriteDataSet(PlistWriter writer, DataSet value)
		//{
		//	writer.WriteArrayStartElement();
		//	foreach (DataTable table in value.Tables)
		//	{
		//		WriteDataTable(writer, table);
		//	}

		//	writer.WriteEndElement();
		//}
		//public void WriteDataRow(PlistWriter writer, DataRow value)
		//{

		//	if (value == null)
		//		return;
		//	var ms = GenerateRowSerializer(value.Table.Columns);
		//	writer.WriteDictionaryStartElement();
		//	foreach (var action in ms)
		//		action(writer, value);
		//	writer.WriteEndElement();

		//}
		//private static IEnumerable<Action<PlistWriter, DataRow>> GenerateRowSerializer(DataColumnCollection columns)
		//{
		//	return
		//		from DataColumn column in columns
		//		let tWriter = CreateTypeWriter(column.DataType)
		//		let o = column.Ordinal
		//		let n = column.ColumnName
		//		select (Action<PlistWriter, DataRow>)
		//		((writer, row) =>
		//			{
		//				var value = row[o];
		//				if (value.Equals(DBNull.Value))
		//					return;
		//				tWriter.Write(writer, value, column.ColumnName);
		//			});
		//}
		#endregion

	}
}