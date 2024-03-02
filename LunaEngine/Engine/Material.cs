using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

[Serializable]
public class Material
{
	public enum TextureType
	{
		Albedo,
		Normal,
		Metallic,
		Roughness,
		AO
	}

	public int TestProp { get; set; } = 22;
	public float TestProp2 { get; set; } = 99;
	public bool TestProp3 { get; set; } = true;

	[Serializable(false)] public Guid GUID { get; private set; } = Guid.NewGuid();

	[Serializable] public Shader? Shader { get; set; }
	[Serializable] public Texture? Albedo { get; set; }
	[Serializable] public Texture? Normal { get; set; }
	[Serializable] public Texture? Metallic { get; set; }
	[Serializable] public Texture? Roughness { get; set; }
	[Serializable] public Texture? AO { get; set; }

	public Material(Shader? shader)
	{
		this.Shader = shader;
	}

	public void Use(Renderer renderer, RenderPassData data, Matrix4x4 modelMatrix)
	{
		if (Shader == null || renderer == null) return;
		renderer.UseShader(Shader);
		BindTexture(Albedo, Material.TextureType.Albedo);
		BindTexture(Normal, Material.TextureType.Normal);
		BindTexture(Metallic, Material.TextureType.Metallic);
		BindTexture(Roughness, Material.TextureType.Roughness);
		BindTexture(AO, Material.TextureType.AO);

		Shader.SetUniform("uView", data.View);
		Shader.SetUniform("uProjection", data.Projection);
		Shader.SetUniform("uModel", modelMatrix);
	}

	private void BindTexture(Texture? texture, Material.TextureType textureType)
	{
		int textureUnit = -1;
		if (texture != null)
		{
			textureUnit = (int) textureType;
			texture?.Bind(TextureUnit.Texture0 + textureUnit);
		}

		Shader.SetUniform($"u{Enum.GetName(textureType)}", textureUnit);
	}
}