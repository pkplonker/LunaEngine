using Newtonsoft.Json;

namespace Editor.Settings;

public static class Settings
{
	private static Dictionary<string, Setting> settings = new();
	private const string SettingsFilePath = "settings.json";

	public static void SaveSettings()
	{
		var settings = new JsonSerializerSettings
		{
			TypeNameHandling = TypeNameHandling.Auto,
			Formatting = Formatting.Indented
		};
		var serializedSettings = JsonConvert.SerializeObject(Settings.settings, settings);
		File.WriteAllText(SettingsFilePath, serializedSettings);
	}

	public static void LoadSettings()
	{
		if (File.Exists(SettingsFilePath))
		{
			var settingsJson = File.ReadAllText(SettingsFilePath);
			var settings = new JsonSerializerSettings
			{
				Converters = new List<JsonConverter> {new SettingJsonConverter()},
				TypeNameHandling = TypeNameHandling.Auto
			};
			var deserializedSettings =
				JsonConvert.DeserializeObject<Dictionary<string, Setting>>(settingsJson, settings);

			if (deserializedSettings != null)
			{
				Settings.settings = deserializedSettings;
			}
		}
	}

	public static float GetSetting(string name, string settingsCategory, bool exposed, float value)
	{
		Setting setting;
		if (!settings.TryGetValue(name, out setting))
		{
			setting = new Setting(name, settingsCategory, typeof(float), exposed);
			setting.Prop = value;
			settings.Add(name, setting);
		}

		return (float) setting.Prop;
	}

	public static int GetSetting(string name, string settingsCategory, bool exposed, int value)
	{
		Setting setting;
		if (!settings.TryGetValue(name, out setting))
		{
			setting = new Setting(name, settingsCategory, typeof(int), exposed);
			setting.Prop = value;
			settings.Add(name, setting);
		}

		return (int) setting.Prop;
	}

	public static string GetSetting(string name, string settingsCategory, bool exposed, string value)
	{
		Setting setting;
		if (!settings.TryGetValue(name, out setting))
		{
			setting = new Setting(name, settingsCategory, typeof(string), exposed);
			setting.Prop = value;
			settings.Add(name, setting);
		}

		return (string) setting.Prop;
	}

	public static bool GetSetting(string name, string settingsCategory, bool exposed, bool value)
	{
		Setting setting;
		if (!settings.TryGetValue(name, out setting))
		{
			setting = new Setting(name, settingsCategory, typeof(bool), exposed);
			setting.Prop = value;
			settings.Add(name, setting);
		}

		return (bool) setting.Prop;
	}
}