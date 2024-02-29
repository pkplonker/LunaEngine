using Newtonsoft.Json;

namespace Editor;

public static class FileExtensions
{
	private static Dictionary<string, IEnumerable<string>> extentionsDict = new();

	static FileExtensions()
	{
		LoadExtensionsFromFile("path_to_your_json_file.json");
	}

	public static IEnumerable<string> GetExtensions(Type type) => extentionsDict.TryGetValue(nameof(type), out var extensions)
		? extensions
		: Enumerable.Empty<string>();

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