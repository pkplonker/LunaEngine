using Engine.Logging;
using Newtonsoft.Json;

namespace Engine;

public static class ObjectSerializer
{
	public static void Serialize(object obj, string absolutePath)
	{
		try
		{
			var res = SceneSerializer.SerializeComponent(obj);
			string jsonString = res.ToString(Formatting.Indented);

			File.WriteAllText(absolutePath, jsonString);
		}
		catch (Exception e)
		{
			Logger.Warning($"Failed to serialize {obj} to {absolutePath}. {e}");
		}
	}
}