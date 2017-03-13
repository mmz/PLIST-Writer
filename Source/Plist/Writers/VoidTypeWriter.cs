namespace Plist.Writers
{
	public class VoidTypeWriter:TypeWriterBase
	{
		protected override void WriteImpl(PlistWriter writer, object obj)
		{
		}

		protected override void WriteImpl(PlistWriter writer, object obj, string key)
		{
		}
	}
}