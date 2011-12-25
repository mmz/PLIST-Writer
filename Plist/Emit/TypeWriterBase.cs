using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Plist.Emit;

namespace Plist
{
	public abstract class TypeWriterBase
	{
#pragma warning disable 649
		protected readonly object[] SupliedObjects;
		protected readonly Type ObjectType;
#pragma warning restore 649

		public void Write(PlistWriter writer, object obj)
		{
			WriteImpl(writer, obj);
		}
		public void Write(PlistWriter writer, object obj, string key)
		{
			WriteImpl(writer, obj, key);
		}

		protected abstract void WriteImpl(PlistWriter writer, object obj);
		protected abstract void WriteImpl(PlistWriter writer, object obj, string key);

		#region Base writer methods
		public static void WriteString(PlistWriter writer, object obj)
		{
			writer.XmlWriter.WriteElementString(
				Plist.StringValueTag,
				Convert.ToString(obj, CultureInfo.InvariantCulture)
			);
		}
		public static void WriteEnum(PlistWriter writer, object obj)
		{
			writer.XmlWriter.WriteElementString(
				Plist.StringValueTag,
				Enum.Format(obj.GetType(), obj, "G")
			);
		}
		public static void WriteInt(PlistWriter writer, object obj)
		{
			writer.XmlWriter.WriteElementString(Plist.IntValueTag, Convert.ToString(obj, CultureInfo.InvariantCulture));
		}
		public static void WriteReal(PlistWriter writer, object obj)
		{
			writer.XmlWriter.WriteElementString(Plist.RealValueTag, Convert.ToString(obj, CultureInfo.InvariantCulture));
		}

		public static void WriteBool(PlistWriter writer, object obj)
		{
			writer.XmlWriter.WriteElementString(((obj as bool?) == true ? Plist.TrueValueTag : Plist.FalseValueTag), null);
		}
		public static void WriteDate(PlistWriter writer, object obj)
		{
			writer.XmlWriter.WriteElementString(Plist.DateValueTag, ((DateTime)obj).ToString(Plist.DateFormat));
		}

		public static void WriteData(PlistWriter writer, object obj)
		{
			var data = (byte[])obj;
			writer.XmlWriter.WriteStartElement(Plist.DataValueTag);
			writer.XmlWriter.WriteBase64(data, 0, data.Length);
			writer.XmlWriter.WriteEndElement();
		}
		public static MethodInfo GetMethodInfo(Type objectType)
		{
			if (objectType.IsValueType)
				return CreateValueTypeWriteMethod(objectType);
			if (objectType.IsEnum)
				return typeof(TypeWriterBase).GetMethod("WriteEnum", BindingFlags.Public | BindingFlags.Static);
			if (objectType == typeof(string))
				return typeof(TypeWriterBase).GetMethod("WriteString", BindingFlags.Public | BindingFlags.Static);
			if (objectType.IsArray && objectType.HasElementType && objectType.GetElementType() == typeof(byte))
				return typeof(TypeWriterBase).GetMethod("WriteData", BindingFlags.Public | BindingFlags.Static);
			if (objectType == typeof(DataSet))
				return typeof(TypeWriterBase).GetMethod("WriteDataSet", BindingFlags.Public | BindingFlags.Static);
			if (objectType == typeof(DataTable))
				return typeof(TypeWriterBase).GetMethod("WriteDataTable", BindingFlags.Public | BindingFlags.Static);
			if (objectType == typeof(DataRow))
				return typeof(TypeWriterBase).GetMethod("WriteDataRow", BindingFlags.Public | BindingFlags.Static);
			return null;
		}
		private static MethodInfo CreateValueTypeWriteMethod(Type objectType)
		{
			if (objectType.IsPrimitive)
			{
				if (typeof(Boolean) == objectType)
					return typeof(TypeWriterBase).GetMethod("WriteBool", BindingFlags.Public | BindingFlags.Static);
				if (typeof(Single) == objectType || typeof(Double) == objectType)
					return typeof(TypeWriterBase).GetMethod("WriteReal", BindingFlags.Public | BindingFlags.Static);
				if (typeof(Char) == objectType)
					return typeof(TypeWriterBase).GetMethod("WriteString", BindingFlags.Public | BindingFlags.Static);

				//if (typeof(Int16) == actualType|| typeof(Int32) == actualType|| typeof(Int64) == actualType|| typeof(UInt16) == actualType
				//|| typeof(UInt32) == actualType|| typeof(UInt64) == actualType|| typeof(Byte) == actualType|| typeof(SByte) == actualType)
				return typeof(TypeWriterBase).GetMethod("WriteInt", BindingFlags.Public | BindingFlags.Static);
			}

			if (typeof(Decimal) == objectType)
				return typeof(TypeWriterBase).GetMethod("WriteReal", BindingFlags.Public | BindingFlags.Static);
			if (typeof(DateTime) == objectType)
				return typeof(TypeWriterBase).GetMethod("WriteDate", BindingFlags.Public | BindingFlags.Static);

			return typeof(TypeWriterBase).GetMethod("WriteString", BindingFlags.Public | BindingFlags.Static);
		}
		#endregion
		#region Static
		public static TypeWriterBase CreateTypeWriter(Type objectType)
		{
			if (typeof(object) == objectType)
				return new ActionTypeWriter(
					(writer, obj) =>
					{
						if (obj == null)
							return;
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

			if (typeof(IPlistWritable).IsAssignableFrom(objectType))
				return new ActionTypeWriter((writer, obj) => { if (obj != null)((IPlistWritable)obj).Write(writer); });

			var plw = objectType.GetCustomAttributes(typeof(PlistValueWriterAttribute), true);
			if (plw.Length > 0)
			{
				return new ActionTypeWriter((writer, obj) => { if (obj != null)((PlistValueWriterAttribute)plw[0]).WriteValue(writer, obj); });
			}

			if (!objectType.IsSerializable && objectType.GetCustomAttributes(typeof(PlistSerializableAttribute), true).Length == 0)
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
				return CreateTypedDictionaryWriter(GetDictionaryItemType(objectType));
			if (typeof(IEnumerable).IsAssignableFrom(objectType))
				return CreateTypedEnumerableWriter(typeof(object));

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

		private static Type GetDictionaryItemType(Type objectType)
		{
			//if (typeof(IDictionary<,>).IsAssignableFrom(objectType))
			//   return null;
			//ToDo: find type of a dictionary value if possible
			return typeof(object);

		}
		#endregion
		#region Data objects
		public static void WriteDataSet(PlistWriter writer, DataSet value)
		{
			writer.WriteArrayStartElement();
			foreach (DataTable table in value.Tables)
			{
				WriteDataTable(writer, table);
			}

			writer.WriteEndElement();
		}
		public static void WriteDataTable(PlistWriter writer, DataTable value)
		{
			if (value == null)
				return;
			writer.WriteArrayStartElement();
			var ms = GenerateRowSerializer(value.Columns);

			foreach (DataRow row in value.Rows)
			{
				writer.WriteDictionaryStartElement();
				foreach (var action in ms)
					action(writer, row);
				writer.WriteEndElement();
			}

			writer.WriteEndElement();
		}
		public void WriteDataRow(PlistWriter writer, DataRow value)
		{

			if (value == null)
				return;
			var ms = GenerateRowSerializer(value.Table.Columns);
			writer.WriteDictionaryStartElement();
			foreach (var action in ms)
				action(writer, value);
			writer.WriteEndElement();

		}
		private static IEnumerable<Action<PlistWriter,DataRow>> GenerateRowSerializer(DataColumnCollection columns)
		{
			return
				from DataColumn column in columns
				let tWriter = CreateTypeWriter(column.DataType)
				let o = column.Ordinal
				let n = column.ColumnName
				select (Action<PlistWriter,DataRow>)
				((writer, row) =>
					{
						var value = row[o];
						if (value.Equals(DBNull.Value))
							return;
						tWriter.Write(writer, value, column.ColumnName);
					});
		}
		#endregion
	}


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
				_valueWriter.Value.Write(writer, dictionaryEnumerator.Value, Convert.ToString(dictionaryEnumerator.Key, CultureInfo.InvariantCulture));
				//if (dictionaryEnumerator.Value == null)
				//   continue;
				//writer.WriteKey(Convert.ToString(dictionaryEnumerator.Key, CultureInfo.InvariantCulture));
				//_valueWriter.Value.Write(writer, dictionaryEnumerator.Value);
			}
			writer.WriteEndElement();
		}
	}
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
				_valueWriter.Value.Write(writer, value);
			}
			writer.WriteEndElement();
		}
	}
}