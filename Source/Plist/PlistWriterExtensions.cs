namespace Plist
{
	public static class PlistWriterExtensions
	{
		/// <summary>
		/// Creates key-value pair 
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="key"></param>
		/// <param name="value">An object to represent in the PropertyList.</param>
		public static void Write<T>(this PlistWriter writer, string key, T value)
		{
			if (value == null)
				return;
			writer.WriteKey(key);
			writer.Write(value);
		}



	}
}