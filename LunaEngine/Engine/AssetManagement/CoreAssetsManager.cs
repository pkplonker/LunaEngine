using Editor;
using Silk.NET.OpenGL;

namespace Engine;

public class CoreAssetsManager : BaseAssetManager
{
	private static readonly string DIRECTORY = "resources/coreassets/".MakeAbsolute();
	public CoreAssetsManager(GL gl) : base(gl, DIRECTORY) { }

	public override void LoadMetadata()
	{
		var paths = ResourceManager.Instance.GetFilesFromFolder(DIRECTORY, FileTypeExtensions.GetAllExtensions());
		paths.Foreach(p => FileImporter.Import(p));
		base.LoadMetadata();
	}
	
	public bool TryGetResourceByPath<T>(string path, out T? result) where T : class
	{
		result = null;

		// Check if the metadata exists for the provided GUID
		if (!metadatas.TryGetValue(guid, out var metadata))
		{
			return false;
		}

		// Depending on the metadata type, try to get or load the resource
		switch (metadata.MetadataType)
		{
			case MetadataType.Texture:
				Texture? texture = null;
				if (typeof(T) == typeof(Texture) && !textures.TryGetValue(guid, out texture))
				{
					texture = LoadTexture(metadata) as Texture;
					if (texture != null)
					{
						textures[guid] = texture;
					}
				}

				result = texture as T;
				break;

			case MetadataType.Material:
				Material? material = null;
				if (typeof(T) == typeof(Material) && !materials.TryGetValue(guid, out material))
				{
					material = LoadMaterial(metadata) as Material;
					if (material != null)
					{
						materials[guid] = material;
					}
				}

				result = material as T;
				break;

			case MetadataType.Shader:
				Shader? shader = null;
				if (typeof(T) == typeof(Shader) && !shaders.TryGetValue(guid, out shader))
				{
					shader = LoadShader(metadata) as Shader;
					if (shader != null)
					{
						shaders[guid] = shader;
					}
				}

				result = shader as T;
				break;

			case MetadataType.Mesh:
				Mesh? mesh = null;
				if (typeof(T) == typeof(Mesh) && !meshes.TryGetValue(guid, out mesh))
				{
					mesh = LoadMesh(metadata) as Mesh;
					if (mesh != null)
					{
						meshes[guid] = mesh;
					}
				}

				result = mesh as T;
				break;

			default:
				Logger.Warning($"Trying to request invalid resource type from guid");
				return false;
		}

		return result != null;
	}
}