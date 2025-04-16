using System.Collections.Generic;
using SexyFramework.Misc;
using Buffer = SexyFramework.Misc.Buffer;

namespace SexyFramework.Drivers.App
{
	public class ConfigItemKey : ConfigItem
	{
		public List<ConfigItem> keys;

		public ConfigItem this[string name]
		{
			get
			{
				ConfigItem result = null;
				foreach (ConfigItem key in keys)
				{
					if (key.name == name)
					{
						result = key;
						break;
					}
				}
				return result;
			}
		}

		public ConfigItemKey()
		{
			keys = new List<ConfigItem>();
			type = ConfigType.Key;
		}

		public override void load(Buffer buffer)
		{
			base.load(buffer);
			int num = buffer.ReadInt32();
			bool flag = false;
			for (int i = 0; i < num; i++)
			{
				if (flag)
				{
					break;
				}
				ConfigItem configItem = null;
				switch (buffer.ReadInt32())
				{
				case 0:
					configItem = new ConfigItemKey();
					break;
				case 1:
					configItem = new ConfigItemString();
					break;
				case 2:
					configItem = new ConfigItemInt();
					break;
				case 3:
					configItem = new ConfigItemBoolean();
					break;
				case 4:
					configItem = new ConfigItemData();
					break;
				default:
					flag = true;
					break;
				}
				if (!flag)
				{
					configItem.load(buffer);
					keys.Add(configItem);
				}
			}
		}

		public override void save(Buffer buffer)
		{
			base.save(buffer);
			buffer.WriteInt32(keys.Count);
			foreach (ConfigItem key in keys)
			{
				key.save(buffer);
			}
		}

		public bool create(string name, ConfigType type)
		{
			bool result = false;
			ConfigItem configItem = null;
			configItem = this[name];
			if (configItem != null)
			{
				if (configItem.type == type)
				{
					result = true;
				}
			}
			else
			{
				switch (type)
				{
				case ConfigType.Key:
					configItem = new ConfigItemKey();
					break;
				case ConfigType.String:
					configItem = new ConfigItemString();
					break;
				case ConfigType.Int:
					configItem = new ConfigItemInt();
					break;
				case ConfigType.Boolean:
					configItem = new ConfigItemBoolean();
					break;
				case ConfigType.Data:
					configItem = new ConfigItemData();
					break;
				}
				if (configItem != null)
				{
					configItem.name = name;
					keys.Add(configItem);
					result = true;
				}
			}
			return result;
		}
	}
}
