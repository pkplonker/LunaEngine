using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

[Inspectable]
[Serializable]
[ResourceIdentifier]
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

	[Inspectable(false)]
	[Serializable(true)]
	public Guid GUID { get; private set; } = Guid.NewGuid();

	[Inspectable]
	public Shader? Shader { get; set; }

	[Inspectable]
	public Texture? Albedo { get; set; }

	[Inspectable]
	public Texture? Normal { get; set; }

	[Inspectable]
	public Texture? Metallic { get; set; }

	[Inspectable]
	public Texture? Roughness { get; set; }

	[Inspectable]
	public Texture? AO { get; set; }

	public Material(Shader? shader)
	{
		this.Shader = shader;
	}

	public Material() { }

	public void SetShader(Shader? shader)
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