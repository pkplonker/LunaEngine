using System.Collections;
using System.Collections.Concurrent;
using Editor;
using Engine.Logging;
using Silk.NET.Assimp;
using Silk.NET.OpenGL;

namespace Engine;

public static class ResourceManager
{
	private readonly static string DEFAULT_FRAG = @"/resources/shaders/unlitfragment.glsl";

	private readonly static string DEFAULT_VERT = @"/resources/shaders/unlitvertex.glsl";

	private static ConcurrentDictionary<Guid, Mesh?> meshes = new();
	private static ConcurrentDictionary<Guid, Shader?> shaders = new();
	private static ConcurrentDictionary<Guid, Texture?> textures = new();
	private static ConcurrentDictionary<Guid, Material?> materials = new();
	private static ConcurrentDictionary<Guid, Metadata> metadatas = new();

	private static GL gl;

	static ResourceManager()
	{
		DEFAULT_FRAG = DEFAULT_FRAG.MakeAbsolute();
		DEFAULT_VERT = DEFAULT_VERT.MakeAbsolute();

		LoadMetadata();
	}

	public static void LoadMetadata()
	{
		var root = ProjectManager.ActiveProject?.Directory;
		if (string.IsNullOrEmpty(root))
		{
			Logger.Warning("No active project to import metadata for");
			return;
		}

		var files = GetFilesFromFolder(root, new List<string>() {Metadata.MetadataFileExtension});
		int result = 0;
		foreach (var file in files)
		{
			var metaData = Metadata.CreateMetadataFromMetadataFile(file);
			if (metaData != null)
			{
				if (AddMetaData(metaData))
				{
					result++;
				}
			}
			else
			{
				Logger.Warning($"Failed to create metadata from {file}");
			}
		}

		Logger.Info($"Imported {result}/{files.Count()} metadata files");
	}

	public static IEnumerable<string> GetFilesFromFolder(string path, IEnumerable<string> ext = null)
	{
		IEnumerable<string> paths = Directory.GetFiles(path);

		if (ext != null && ext.Any())
		{
			paths = paths.Where(p => ext.Contains(Path.GetExtension(p).TrimStart('.').ToLowerInvariant()));
		}

		foreach (var dir in Directory.GetDirectories(path))
		{
			paths = paths.Concat(GetFilesFromFolder(dir, ext));
		}

		return paths;
	}

	public static void Init(GL gl)
	{
		ResourceManager.gl = gl;

		//test
		// try
		// {
		// 	var shader = new Shader(gl, @"assets/Shaders\unlit.glsl".MakeProjectAbsolute());
		// 	var pbr = new Shader(gl, @"assets/Shaders\unlit.glsl".MakeProjectAbsolute());
		// }
		// catch (Exception e)
		// {
		// 	Console.WriteLine(e);
		// 	throw;
		// }
	}

	public static bool TryGetResourceByGuid(Guid guid, out object? result)
	{
		result = null;
		if (!metadatas.TryGetValue(guid, out var metadata))
		{
			return false;
		}

		switch (metadata.MetadataType)
		{
			case MetadataType.Texture:
				if (!textures.TryGetValue(metadata.GUID, out var texture))
				{
					texture = LoadTexture(metadata);
					if (texture != null)
					{
						textures[metadata.GUID] = texture;
					}
				}

				result = texture;
				break;

			case MetadataType.Material:
				if (!materials.TryGetValue(metadata.GUID, out var material))
				{
					material = LoadMaterial(metadata);
					if (material != null)
					{
						materials[metadata.GUID] = material;
					}
				}

				result = material;
				break;

			case MetadataType.Shader:
				if (!shaders.TryGetValue(metadata.GUID, out var shader))
				{
					shader = LoadShader(metadata);
					if (shader != null)
					{
						shaders[metadata.GUID] = shader;
					}
				}

				result = shader;
				break;

			case MetadataType.Mesh:
				if (!meshes.TryGetValue(metadata.GUID, out var mesh))
				{
					mesh = LoadMesh(metadata);
					if (mesh != null)
					{
						meshes[metadata.GUID] = mesh;
					}
				}

				result = mesh;
				break;

			default:
				result = null;
				break;
		}

		return result != null;
	}

	private static Texture? LoadTexture(Metadata metadata)
	{
		try
		{
			return new Texture(gl, metadata.Path.MakeProjectAbsolute());
		}
		catch (Exception e)
		{
			Logger.Warning($"Failed to load texture {e}");
			return null;
		}
	}

	private static Material? LoadMaterial(Metadata metadata)
	{
		try
		{
			return null;
		}
		catch (Exception e)
		{
			Logger.Warning($"Failed to load material {e}");
			return null;
		}
	}

	private static Shader? LoadShader(Metadata metadata)
	{
		try
		{
			return new Shader(gl, metadata.Path.MakeProjectAbsolute());
		}
		catch (Exception e)
		{
			Logger.Warning($"Failed to generate shader {e}");
			return null;
		}
	}

	private static Mesh? LoadMesh(Metadata metadata)
	{
		try
		{
			return ModelLoader.LoadModel(gl, metadata.Path.MakeProjectAbsolute());
		}
		catch (Exception e)
		{
			Logger.Warning($"Failed to load mesh {e}");
			return null;
		}
	}

	public static bool AddMetaData(Metadata metadata)
	{
		if (metadatas.TryAdd(metadata.GUID, metadata))
		{
			return true;
		}

		Logger.Warning($"Duplicate metadata detected {metadata.GUID}");
		return false;
	}

	public static IEnumerable<Metadata> GetMetadata(MetadataType? filterType = null) =>
		metadatas.Values.Where(metadata => filterType == null || metadata.MetadataType == filterType);

	public static bool MetadataExistsWithPath(string path) =>
		metadatas.Values.Any(m => m.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
}