﻿using System.Collections;
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
	}

	public static bool TryGetResourceByGuid<T>(Guid guid, out T? result) where T : class
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

	private static Texture? LoadTexture(Metadata metadata)
	{
		try
		{
			return new Texture(gl, metadata.Path.MakeProjectAbsolute(), metadata.GUID);
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
			return (Material) ObjectSerializer.Deserialize(metadata.Path.MakeProjectAbsolute());
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
			return new Shader(gl, metadata.Path.MakeProjectAbsolute(), metadata.GUID);
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
			return ModelLoader.LoadModel(gl, metadata.Path.MakeProjectAbsolute(), metadata.GUID);
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

	public static void ClearMetadatas()
	{
		metadatas.Clear();
	}
}