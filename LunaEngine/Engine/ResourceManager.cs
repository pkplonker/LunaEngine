using Silk.NET.OpenGL;

namespace Engine;

public static class ResourceManager
{
	private const string DEFAULT_FRAG = @"/resources/shaders/unlitfragment.glsl";

	private const string DEFAULT_VERT = @"/resources/shaders/unlitvertex.glsl";

	private static Dictionary<string, Mesh?> meshes = new();
	private static Dictionary<string, Shader> shaders = new();
	private static Dictionary<string, Texture> textures = new();
	//private static Dictionary<string, Material> materials = new();

	private static GL gl;

	public static void Init(GL gl)
	{
		ResourceManager.gl = gl;
	}

	public static Texture? GetTexture(string path)
	{
		
		if (textures.ContainsKey(path))
		{
			return textures[path];
		}

		Texture texture;

		try
		{
			texture = new Texture(gl, path);
		}
		catch (Exception e)
		{
			Console.WriteLine($"Failed to generate texture{e}");
			return null;
		}

		if (texture != null)
		{
			textures.TryAdd(path, texture);
		}

		return texture;
	}

	public static Mesh? GetMesh(string path)
	{
		if (!meshes.ContainsKey(path))
		{
			var mesh = ModelLoader.LoadModel(gl, path);
			if (mesh != null)
			{
				meshes.TryAdd(path, mesh);
			}
		}

		return meshes.ContainsKey(path) ? meshes[path] : null;
	}

	public static Shader? GetShader(string vertPath = DEFAULT_VERT, string fragPath = DEFAULT_FRAG)
	{
		var key = vertPath + fragPath;
		
		if (shaders.ContainsKey(key))
		{
			return shaders[key];
		}

		Shader shader;

		try
		{
			shader = new Shader(gl, vertPath, fragPath);
		}
		catch (Exception e)
		{
			Console.WriteLine($"Failed to generate shader{e}");
			return null;
		}

		if (shader != null)
		{
			shaders.TryAdd(key, shader);
		}

		return shader;
	}
}