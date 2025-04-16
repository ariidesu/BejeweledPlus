using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.Drivers.App
{
	public class ConfigItemBoolean : ConfigItem
	{
		public bool value;

		public ConfigItemBoolean()
		{
			value = false;
			type = ConfigType.Boolean;
		}

		public override void load(Buffer buffer)
		{
			base.load(buffer);
			value = buffer.ReadBoolean();
		}

		public override void save(Buffer buffer)
		{
			base.save(buffer);
			buffer.WriteBoolean(value);
		}
	}
}
