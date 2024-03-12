namespace Engine;

public class CoreAssets
{
	private static readonly Dictionary<string, Metadata> metadatas = new();

	public static Metadata? GetMetadata(string key)
	{
		metadatas.TryGetValue(key, out var metadata);
		return metadata;
	}
}