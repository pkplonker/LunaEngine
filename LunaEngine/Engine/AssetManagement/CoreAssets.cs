using Engine.Logging;

namespace Engine;

public class CoreAssets 
{
	private static readonly Dictionary<string, string> assetPaths = new()
	{
		{DEFAULT_SHADER, "/resources/coreassets/shaders/pbr.glsl".MakeAbsolute()},
		{DEFAULT_CUBE, "/resources/coreassets/models/testcube.obj".MakeAbsolute()},
		{DEFAULT_SPHERE, "/resources/coreassets/models/testsphere.obj".MakeAbsolute()},
		{DEFAULT_PLANE, "/resources/coreassets/models/plane.fbx".MakeAbsolute()},
	};

	public const string DEFAULT_SHADER = "DEFAULT_SHADER";
	public const string DEFAULT_CUBE = "DEFAULT_CUBE";
	public const string DEFAULT_SPHERE = "DEFAULT_SPHERE";
	public const string DEFAULT_PLANE = "DEFAULT_PLANE";

	private static readonly Dictionary<string, Metadata> metadatas = new();

	static CoreAssets()
	{
		LoadCoreAssetsMetadata();
	}

	private static void LoadCoreAssetsMetadata()
	{
		foreach (var kvp in assetPaths)
		{
			string key = kvp.Key;
			string path = kvp.Value;
			string metadataFilePath =
				path + Metadata
					.MetadataFileExtension;

			var metadata = Metadata.CreateMetadataFromMetadataFile(metadataFilePath);
			if (metadata != null)
			{
				metadatas[key] = metadata;
			}
			else
			{
				Logger.Warning($"Failed to create metadata for core asset {key} from {metadataFilePath}");
			}
		}
	}

	public static Metadata? GetMetadata(string key)
	{
		metadatas.TryGetValue(key, out var metadata);
		return metadata;
	}
}