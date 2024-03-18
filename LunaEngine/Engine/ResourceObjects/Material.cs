using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

[Inspectable]
[Serializable]
public class Material : IMaterial
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
	[ResourceGuid(typeof(Shader))]
	public Guid ShaderGUID { get; set; }


	[Inspectable]
	[ResourceGuid(typeof(Texture))]
	public Guid AlbedoGUID { get; set; }

	[Inspectable]
	[ResourceGuid(typeof(Texture))]
	public Guid NormalGUID { get; set; }

	[Inspectable]
	[ResourceGuid(typeof(Texture))]
	public Guid MetallicGUID { get; set; }

	[Inspectable]
	[ResourceGuid(typeof(Texture))]
	public Guid RoughnessGUID { get; set; }

	[Inspectable]
	[ResourceGuid(typeof(Texture))]
	public Guid AOGUID { get; set; }

	public Material(Guid shaderGuid)
	{
		this.ShaderGUID = shaderGuid;
	}

	public Material() { }

	public void SetShader(Guid shader)
	{
		this.ShaderGUID = shader;
	}

	public void Use(Renderer renderer, RenderPassData data, Matrix4x4 modelMatrix)
	{
		if (renderer == null) return;
		Shader? shader = null;
		if (!ResourceManager.Instance.TryGetResourceByGuid<Shader>(ShaderGUID, out shader))
		{
			return;
		}

		if (shader == null) return;
		renderer.UseShader(shader);
		BindTexture(AlbedoGUID, Material.TextureType.Albedo, shader);
		BindTexture(NormalGUID, Material.TextureType.Normal, shader);
		BindTexture(MetallicGUID, Material.TextureType.Metallic, shader);
		BindTexture(RoughnessGUID, Material.TextureType.Roughness, shader);
		BindTexture(AOGUID, Material.TextureType.AO, shader);

		shader.SetUniform("uView", data.View);
		shader.SetUniform("uProjection", data.Projection);
		shader.SetUniform("uModel", modelMatrix);
	}

	private void BindTexture(Guid textureGuid, Material.TextureType textureType, Shader shader)
	{
		ResourceManager.Instance.TryGetResourceByGuid<Texture>(textureGuid, out var texture);
		int textureUnit = -1;
		if (texture != null)
		{
			textureUnit = (int) textureType;
			texture?.Bind(TextureUnit.Texture0 + textureUnit);
		}

		shader.SetUniform($"u{Enum.GetName(textureType)}", textureUnit);
	}
}