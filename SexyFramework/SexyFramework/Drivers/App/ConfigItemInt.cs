using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.Drivers.App
{
	public class ConfigItemInt : ConfigItem
	{
		public int value;

		public ConfigItemInt()
		{
			value = 0;
			type = ConfigType.Int;
		}

		public override void load(Buffer buffer)
		{
			base.load(buffer);
			value = buffer.ReadInt32();
		}

		public override void save(Buffer buffer)
		{
			base.save(buffer);
			buffer.WriteInt32(value);
		}
	}
}
