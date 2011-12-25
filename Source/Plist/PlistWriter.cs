using System;
using System.Xml;

namespace Plist
{
	public class PlistWriter
	{
		private const int DEFAULT_MAX_RECURSION = 32;
		public XmlWriter XmlWriter { get; protected set; }
		public int MaxRecursion { get; protected set; }
		private int _nestLevel;

		public PlistWriter(XmlWriter xmlWriter) : this(xmlWriter, DEFAULT_MAX_RECURSION)
		{
		}

		public PlistWriter(XmlWriter xmlWriter, int maxRecursion)
		{
			_nestLevel = 0;
			MaxRecursion = maxRecursion;
			XmlWriter = xmlWriter;
		}

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

		/// <summary>
		/// Creates key-value pair 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void Write(string key, object value)
		{

			if (value == null)
				return;
			var objectType = value.GetType();
			var typeWriter = TypeWriterBase.CreateTypeWriter(objectType);
			typeWriter.Write(this, value, key);
		}

		#endregion

		#region predifined formatting
		/// <summary>
		/// Writes value into plist integer tag
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void WriteInteger(object value)
		{
			WriteIntegerImpl(null, value);
		}

		/// <summary>
		/// Writes key value pair using plist integer tag for value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void WriteInteger(string key, object value)
		{
			WriteIntegerImpl(key, value);
		}

		/// <summary>
		/// Writes value into plist real tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void WriteReal(object value)
		{
			WriteRealImpl(null, value);
		}

		/// <summary>
		/// Writes key value pair using plist real tag for value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void WriteReal(string key, object value)
		{
			WriteRealImpl(key, value);
		}

		/// <summary>
		/// Writes value into plist string tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void WriteString(object value)
		{
			WriteStringImpl(null, value);
		}

		/// <summary>
		/// Writes key value pair using plist string tag for value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void WriteString(string key, object value)
		{
			WriteStringImpl(key, value);
		}

		/// <summary>
		/// Writes plist boolean value representation based upon the object passed in.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void WriteBoolean(object value)
		{
			WriteBooleanImpl(null, value);
		}

		/// <summary>
		/// Writes key value pair using plist boolean value representation for value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void WriteBoolean(string key, object value)
		{
			WriteBooleanImpl(key, value);
		}

		/// <summary>
		/// Writes byte array into plist data tag using base64 encoding.
		/// </summary>
		/// <param name="data">An object to represent in the PropertyList.</param>
		public virtual void WriteData(byte[] data)
		{
			WriteDataImpl(null, data);
		}

		/// <summary>
		/// Writes byte array with key value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="data">An object to represent in the PropertyList.</param>
		public virtual void WriteData(string key, byte[] data)
		{
			WriteDataImpl(key, data);
		}

		/// <summary>
		/// Writes date and time into plist date tag.
		/// </summary>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void WriteDate(object value)
		{
			WriteDateImpl(null, value);
		}

		/// <summary>
		/// Writes date and time with key value.
		/// </summary>
		/// <param name="key">Key value.</param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public virtual void WriteDate(string key, object value)
		{
			WriteDateImpl(key, value);
		}
		#endregion

		#region Implementation

		void WriteIntegerImpl(string key, object value)
		{
			if (value == null)
				return;
			if (!string.IsNullOrEmpty(key))
				WriteKey(key);
			TypeWriterBase.WriteInt(this, value);
		}

		void WriteRealImpl(string key, object value)
		{
			if (value == null)
				return;
			if (!string.IsNullOrEmpty(key))
				WriteKey(key);
			TypeWriterBase.WriteReal(this, value);
		}

		void WriteStringImpl(string key, object value)
		{
			if (value == null)
				return;
			if (!string.IsNullOrEmpty(key))
				WriteKey(key);
			TypeWriterBase.WriteString(this, value);
		}

		void WriteBooleanImpl(string key, object value)
		{
			if (value == null)
				return;
			if (!string.IsNullOrEmpty(key))
				WriteKey(key);
			TypeWriterBase.WriteBool(this, value);
		}

		void WriteDataImpl(string key, byte[] data)
		{
			if (data == null || data.Length == 0)
				return;
			if (!string.IsNullOrEmpty(key))
				WriteKey(key);
			TypeWriterBase.WriteData(this, data);

		}

		void WriteDateImpl(string key, object value)
		{
			if (value == null)
				return;
			if (!string.IsNullOrEmpty(key))
				WriteKey(key);

			if (value is DateTime || value is DateTime?)
				TypeWriterBase.WriteDate(this, value);
			else
				XmlWriter.WriteElementString(Plist.DateValueTag, value.ToString());
		}

		#endregion

	}
}