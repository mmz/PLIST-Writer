namespace Plist.Writers
{
	public class GeneralTypeWriter : TypeWriterBase
	{
		protected override void WriteImpl(PlistWriter writer, object obj)
		{
			var typeWriter = GetTypeWriter(obj.GetType());
			typeWriter.Write(writer, obj);
		}

		protected override void WriteImpl(PlistWriter writer, object obj, string key)
		{
			var typeWriter = GetTypeWriter(obj.GetType());
			typeWriter.Write(writer, key, obj);
		}
	}
}