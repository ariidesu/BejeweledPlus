using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.Drivers.App
{
	public class ConfigItem
	{
		public ConfigType type;

		public string name;

		public ConfigItem()
		{
			name = string.Empty;
			type = ConfigType.String;
		}

		public virtual void load(Buffer buffer)
		{
			name = buffer.ReadString();
		}

		public virtual void save(Buffer buffer)
		{
			buffer.WriteInt32((int)type);
			buffer.WriteString(name);
		}
	}
}
