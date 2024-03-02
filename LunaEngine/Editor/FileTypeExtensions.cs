using Engine;
using Newtonsoft.Json;

namespace Editor;

public static class FileTypeExtensions
{
	private static Dictionary<string, IEnumerable<string>> extentionsDict = new();

	static FileTypeExtensions()
	{
		LoadExtensionsFromFile(FileExtensions.MakeAbsolute("resources/extensions.json"));
	}

	public static IEnumerable<string> GetExtensions(Type? type)
	{
		if (type == null)
		{
			return Enumerable.Empty<string>();
		}

		var name = type.Name.Split('.')[^1];
		if (extentionsDict.TryGetValue(name, out var extensions))
		{
			return extensions;
		}

		return Enumerable.Empty<string>();
	}

	private static void LoadExtensionsFromFile(string filePath)
	{
		if (!File.Exists(filePath))
			throw new FileNotFoundException($"The file {filePath} was not found.");

		string json = File.ReadAllText(filePath);
		var data = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(json);

		if (data != null)
		{
			foreach (var kvp in data)
			{
				extentionsDict[kvp.Key] = kvp.Value.AsEnumerable();
			}
		}
	}
}