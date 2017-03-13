using System;
using System.Globalization;
using System.Xml;
using Plist.Writers;

namespace Plist
{
	public class PlistWriter
	{
		private const int DEFAULT_MAX_RECURSION = 32;
		public XmlWriter XmlWriter { get; protected set; }
		public int MaxRecursion { get; protected set; }
		private int _nestLevel;
		#region Ctor
		public PlistWriter(XmlWriter xmlWriter) : this(xmlWriter, DEFAULT_MAX_RECURSION)
		{
		}

		public PlistWriter(XmlWriter xmlWriter, int maxRecursion)
		{
			_nestLevel = 0;
			MaxRecursion = maxRecursion;
			XmlWriter = xmlWriter;
		}
		#endregion
		#region Syntax helpers
		/// <summary>
		/// Writes the XML declaration with PropertyList-1.0.dtd
		/// </summary>
		public virtual void WriteStartDocument()
		{
			XmlWriter.WriteStartDocument();
			XmlWriter.WriteDocType(Plist.PlistTag, Plist.DocTypePubid, Plist.DocTypeSysid, null);
			XmlWriter.WriteStartElement(Plist.PlistTag);
			XmlWriter.WriteAttributeString("version", Plist.PlistVersion);
		}

		/// <summary>
		/// Closes any open elements or attributes and puts the underlying writer back into Start state.
		/// </summary>
		public virtual void WriteEndDocument()
		{
			XmlWriter.WriteEndDocument();
		}

		/// <summary>
		/// Writes out a "array" start tag.
		/// </summary>
		public virtual void WriteArrayStartElement()
		{
			XmlWriter.WriteStartElement(Plist.ArrayValueTag);
		}

		/// <summary>
		/// Writes out a "dictionary" start tag.
		/// </summary>
		public virtual void WriteDictionaryStartElement()
		{
			XmlWriter.WriteStartElement(Plist.DictValueTag);
		}

		/// <summary>
		/// Writes out a "integer" start tag.
		/// </summary>
		public virtual void WriteIntegerStartElement()
		{
			XmlWriter.WriteStartElement(Plist.IntValueTag);
		}

		/// <summary>
		/// Writes out a "real" start tag.
		/// </summary>
		public virtual void WriteRealStartElement()
		{
			XmlWriter.WriteStartElement(Plist.RealValueTag);
		}

		/// <summary>
		/// Writes out a "real" start tag.
		/// </summary>
		public virtual void WriteDateStartElement()
		{
			XmlWriter.WriteStartElement(Plist.DateValueTag);
		}

		/// <summary>
		/// Writes out a "string" start tag.
		/// </summary>
		public virtual void WriteStringStartElement()
		{
			XmlWriter.WriteStartElement(Plist.StringValueTag);
		}

		/// <summary>
		/// Writes out string.
		/// </summary>
		public virtual void WriteRawString(string str)
		{
			XmlWriter.WriteString(str);
		}

		/// <summary>
		/// Closes active element.
		/// </summary>
		public virtual void WriteEndElement()
		{
			XmlWriter.WriteEndElement();
		}

		/// <summary>
		/// Creates plist key element in the XmlWriter.
		/// </summary>
		/// <param name="name">Key value.</param>
		public virtual void WriteKey(string name)
		{
			XmlWriter.WriteElementString(Plist.KeyTag, name);
		}
		#endregion

		#region Dynamic
		/// <summary>
		/// Creates a valid XML fragment in the XmlWriter, based upon the object passed in.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(object value)
		{
			if (value == null || _nestLevel > MaxRecursion)
				return;
			_nestLevel++;
			var objectType = value.GetType();
			var typeWriter = TypeWriterBase.CreateTypeWriter(objectType);
			typeWriter.Write(this, value);
			_nestLevel--;
		}
		public virtual void Write(string key, object value)
		{
			if (value == null || _nestLevel > MaxRecursion)
				return;
			_nestLevel++;
			var objectType = value.GetType();
			var typeWriter = TypeWriterBase.CreateTypeWriter(objectType);
			typeWriter.Write(this, key, value);
			_nestLevel--;
		}
		#endregion

		#region write types

		#region Boolean
		/// <summary>
		/// Writes plist boolean value representation based upon the object passed in.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(Boolean value)
		{
			XmlWriter.WriteStartElement(value ? Plist.TrueValueTag : Plist.FalseValueTag);
			XmlWriter.WriteEndElement();
		}
		#endregion

		#region DateTime
		/// <summary>
		/// Writes date and time into plist date tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(DateTime value)
		{
			XmlWriter.WriteElementString(Plist.DateValueTag, value.ToString(Plist.DateFormat));
		}
		#endregion

		#region Real
		/// <summary>
		/// Writes value into plist real tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(Decimal value)
		{
			XmlWriter.WriteElementString(Plist.RealValueTag, value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes value into plist real tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(Single value)
		{
			XmlWriter.WriteElementString(Plist.RealValueTag, value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes value into plist real tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(Double value)
		{
			XmlWriter.WriteElementString(Plist.RealValueTag, value.ToString(CultureInfo.InvariantCulture));
		}

		#endregion

		#region Integer
		/// <summary>
		/// Writes value into plist integer tag
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(Int16 value)
		{
			XmlWriter.WriteElementString(Plist.IntValueTag, value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes value into plist integer tag
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(Int32 value)
		{
			XmlWriter.WriteElementString(Plist.IntValueTag, value.ToString(CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes value into plist integer tag
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(Int64 value)
		{
			XmlWriter.WriteElementString(Plist.IntValueTag, value.ToString(CultureInfo.InvariantCulture));
		}

		#endregion


		#region Enum
		/// <summary>
		/// Writes value into plist string tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(Enum value)
		{
			XmlWriter.WriteElementString(Plist.StringValueTag, value.ToString("F"));
		}

		#endregion

		/// <summary>
		/// Writes byte array into plist data tag using base64 encoding.
		/// </summary>
		/// <param name="data">An object to represent in the PropertyList.</param>
		public virtual void Write(byte[] data)
		{
			XmlWriter.WriteStartElement(Plist.DataValueTag);
			XmlWriter.WriteBase64(data, 0, data.Length);
			XmlWriter.WriteEndElement();
		}

		#region String

		/// <summary>
		/// Writes value into plist string tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(string value)
		{
			XmlWriter.WriteElementString(Plist.StringValueTag, value);
		}

		#endregion

		#endregion

		#region predifined formatting
		/// <summary>
		/// Writes value into plist integer tag
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteInteger(object value)
		{
			WriteIntegerImpl(value);
		}

		/// <summary>
		/// Writes key value pair using plist integer tag for value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteInteger(string key, object value)
		{
			WriteKey(key);
			WriteIntegerImpl(value);
		}


		/// <summary>
		/// Writes value into plist real tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteReal(object value)
		{
			WriteRealImpl(value);
		}

		/// <summary>
		/// Writes key value pair using plist real tag for value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteReal(string key, object value)
		{
			WriteKey(key);
			WriteRealImpl(value);
		}

		/// <summary>
		/// Writes value into plist string tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteString(object value)
		{
			WriteStringImpl(Convert.ToString(value, CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes key value pair using plist string tag for value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteString(string key, object value)
		{
			WriteKey(key);
			WriteStringImpl(Convert.ToString(value, CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Writes plist boolean value representation based upon the object passed in.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteBoolean(object value)
		{
			Write((bool)value);
		}

		/// <summary>
		/// Writes key value pair using plist boolean value representation for value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteBoolean(string key, object value)
		{
			WriteKey(key);
			Write((bool)value);
		}

		/// <summary>
		/// Writes byte array into plist data tag using base64 encoding.
		/// </summary>
		/// <param name="data">An object to represent in the PropertyList.</param>
		public void WriteData(byte[] data)
		{
			Write(data);
		}

		/// <summary>
		/// Writes byte array with key value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="data">An object to represent in the PropertyList.</param>
		public void WriteData(string key, byte[] data)
		{
			WriteKey(key);
			Write(data);
		}

		/// <summary>
		/// Writes date and time into plist date tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteDate(object value)
		{
			Write((DateTime)value);
		}

		/// <summary>
		/// Writes date and time with key value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteDate(string key, object value)
		{
			WriteKey(key);
			Write((DateTime)value);
		}

		#region Enum
		/// <summary>
		/// Writes enum value into plist string tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteEnum(object value)
		{
			Write((Enum)value);
		}

		/// <summary>
		/// Writes key value pair using plist string tag for enum value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public void WriteEnum(string key, object value)
		{
			WriteKey(key);
			Write((Enum)value);
		}
		#endregion
		#endregion

		#region Implementation

		protected virtual void WriteIntegerImpl(object value)
		{
			XmlWriter.WriteElementString(Plist.IntValueTag, Convert.ToString(value, CultureInfo.InvariantCulture));
		}

		protected virtual void WriteRealImpl(object value)
		{
			XmlWriter.WriteElementString(Plist.RealValueTag, Convert.ToString(value, CultureInfo.InvariantCulture));
		}

		protected virtual void WriteStringImpl(string value)
		{
			XmlWriter.WriteElementString(
				Plist.StringValueTag,
				value
			);
		}

		#endregion

	}
}