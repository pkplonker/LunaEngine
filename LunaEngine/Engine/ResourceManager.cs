﻿using Engine.Logging;
using Silk.NET.OpenGL;

namespace Engine;

public static class ResourceManager
{
	private readonly static string DEFAULT_FRAG = @"/resources/shaders/unlitfragment.glsl";

	private readonly static string DEFAULT_VERT = @"/resources/shaders/unlitvertex.glsl";

	private static Dictionary<string, Mesh?> meshes = new();
	private static Dictionary<string, Shader> shaders = new();
	private static Dictionary<string, Texture> textures = new();
	private static Dictionary<string, Material> materials = new();

	private static Dictionary<Guid, Texture> guidToTextures = new Dictionary<Guid, Texture>();
	private static Dictionary<Guid, Mesh> guidToMeshes = new Dictionary<Guid, Mesh>();
	private static Dictionary<Guid, Shader> guidToShaders = new Dictionary<Guid, Shader>();
	private static Dictionary<Guid, Material> guidToMaterialss = new Dictionary<Guid, Material>();
	private static Dictionary<Guid, string> guidToPath = new Dictionary<Guid, string>();

	private static GL gl;

	static ResourceManager()
	{
		DEFAULT_FRAG = DEFAULT_FRAG.MakeAbsolute();
		DEFAULT_VERT = DEFAULT_VERT.MakeAbsolute();
	}

	public static void Init(GL gl)
	{
		ResourceManager.gl = gl;
	}

	public static Texture? GetTexture(string path)
	{
		if (!textures.ContainsKey(path))
		{
			try
			{
				var texture = new Texture(gl, path);
				if (texture != null)
				{
					textures.TryAdd(path, texture);
					guidToTextures.TryAdd(texture.GUID, texture);
					guidToPath[texture.GUID] = path;
				}
			}
			catch (Exception e)
			{
				Logger.Warning($"Failed to generate texture: {e}");
				return null;
			}
		}

		return textures[path];
	}

	public static Mesh? GetMesh(string path)
	{
		if (!meshes.ContainsKey(path))
		{
			var mesh = ModelLoader.LoadModel(gl, path);
			if (mesh != null)
			{
				meshes.TryAdd(path, mesh);
				guidToMeshes.TryAdd(mesh.GUID, mesh);
				guidToPath[mesh.GUID] = path;
			}
		}

		return meshes[path];
	}

	public static Shader? GetShader(string vertPath = "", string fragPath = "")
	{
		var split = vertPath.Split('|');
		if (split.Length == 2)
		{
			vertPath = split[0];
			fragPath = split[1];
		}

		var key = vertPath + "|" + fragPath;
		if (shaders.ContainsKey(key))
		{
			return shaders[key];
		}

		if (string.IsNullOrEmpty(vertPath))
		{
			vertPath = DEFAULT_VERT;
		}

		if (string.IsNullOrEmpty(fragPath))
		{
			fragPath = DEFAULT_FRAG;
		}

		Shader shader;

		try
		{
			shader = new Shader(gl, vertPath, fragPath);
		}
		catch (Exception e)
		{
			Logger.Warning($"Failed to generate shader{e}");
			return null;
		}

		if (shader != null)
		{
			shaders.TryAdd(key, shader);
			guidToShaders.TryAdd(shader.GUID, shader);
			guidToPath[shader.GUID] = key;
		}

		return shader;
	}

	private static Dictionary<Guid, string> guidToResourcePath = new Dictionary<Guid, string>();

	public static object? GetResourceByGuid(Type resourceType, Guid guid)
	{
		if (resourceType == typeof(Texture) && guidToTextures.TryGetValue(guid, out var texture))
		{
			return texture;
		}

		if (resourceType == typeof(Mesh) && guidToMeshes.TryGetValue(guid, out var mesh))
		{
			return mesh;
		}

		if (resourceType == typeof(Shader) && guidToMeshes.TryGetValue(guid, out var shader))
		{
			return shader;
		}

		if (resourceType == typeof(Material) && guidToMeshes.TryGetValue(guid, out var material))
		{
			return material;
		}

		if (!guidToResourcePath.TryGetValue(guid, out var resourcePath))
		{
			Logger.Warning($"No resource found for GUID {guid}");
			return null;
		}

		try
		{
			if (resourceType == typeof(Texture))
			{
				return GetTexture(resourcePath);
			}

			if (resourceType == typeof(Mesh))
			{
				return GetMesh(resourcePath);
			}

			if (resourceType == typeof(Shader))
			{
				return GetShader(resourcePath);
			}

			if (resourceType == typeof(Material))
			{
				return GetMaterial(resourcePath);
			}
		}
		catch (Exception e)
		{
			Logger.Warning($"Failed to retrieve resource for GUID {guid}: {e}");
		}

		return null;
	}

	private static Material? GetMaterial(string resourcePath)
	{
		return null;
	}
}