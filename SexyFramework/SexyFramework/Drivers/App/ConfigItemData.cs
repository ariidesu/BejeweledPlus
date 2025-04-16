using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.Drivers.App
{
	public class ConfigItemData : ConfigItem
	{
		public byte[] value;

		public ConfigItemData()
		{
			value = null;
			type = ConfigType.Data;
		}

		public override void load(Buffer buffer)
		{
			base.load(buffer);
			int num = buffer.ReadInt32();
			value = new byte[num];
			buffer.ReadBytes(ref value, num);
		}

		public override void save(Buffer buffer)
		{
			base.save(buffer);
			buffer.WriteInt32(value.Length);
			buffer.WriteBytes(value, value.Length);
		}
	}
}
