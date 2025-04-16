using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.Drivers.App
{
	public class ConfigItemString : ConfigItem
	{
		public string value;

		public ConfigItemString()
		{
			value = string.Empty;
			type = ConfigType.String;
		}

		public override void load(Buffer buffer)
		{
			base.load(buffer);
			value = buffer.ReadString();
		}

		public override void save(Buffer buffer)
		{
			base.save(buffer);
			buffer.WriteString(value);
		}
	}
}
