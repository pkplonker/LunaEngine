using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

public class MeshRenderer : IRenderableComponent
{
	public Shader Shader { get; set; }

	public MeshRenderer(GameObject gameObject)
	{
		this.GameObject = gameObject;
	}

	public void Render(GL gl, RenderPassData data)
	{
		this.Shader?.Use();
		this.Shader.SetUniform("uView", data.View);
		this.Shader.SetUniform("uProjection", data.Projection);
		this.Shader.SetUniform("uProjection", GameObject.Transform.ModelMatrix);

		var mf = GameObject.GetComponent<MeshFilter>();
		if (mf != null)
		{
			foreach (var mesh in mf.meshes)
			{
				mesh?.Render(gl, data);
			}
		}
	}
	public GameObject GameObject { get; set; }
	public Shader GetShader() => Shader;
}