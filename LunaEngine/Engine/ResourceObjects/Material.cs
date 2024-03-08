﻿using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

[Inspectable]
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

	[Inspectable(false)]
	[Serializable(true)]
	public Guid GUID { get; private set; } = Guid.NewGuid();

	[Inspectable]
	public Guid ShaderGUID { get; set; }

	[Inspectable]
	public Guid AlbedoGUID { get; set; }
	
	[Inspectable]
	public Guid NormalGUID { get; set; }

	[Inspectable]
	public Guid MetallicGUID { get; set; }

	[Inspectable]
	public Guid RoughnessGUID { get; set; }

	[Inspectable]
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
		if (!ResourceManager.TryGetResourceByGuid<Shader>(ShaderGUID, out shader))
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
		if (!ResourceManager.TryGetResourceByGuid<Texture>(textureGuid, out var texture)) return;
		int textureUnit = -1;
		if (texture != null)
		{
			textureUnit = (int) textureType;
			texture?.Bind(TextureUnit.Texture0 + textureUnit);
		}

		shader.SetUniform($"u{Enum.GetName(textureType)}", textureUnit);
	}
}