using System.Numerics;

namespace Engine;

[Serializable]
public class Material
{
	[Serializable]
	public Shader? Shader { get; set; }

	public Material(Shader? shader)
	{
		this.Shader = shader;
	}

	public void Use(Renderer renderer, RenderPassData data, Matrix4x4 modelMatrix)
	{
		if (Shader == null || renderer == null) return;
		renderer.UseShader(Shader);
		Shader.SetUniform("uView", data.View);
		Shader.SetUniform("uProjection", data.Projection);
		Shader.SetUniform("uModel", modelMatrix);
	}
}