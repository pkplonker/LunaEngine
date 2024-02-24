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

	public void Render(Renderer renderer, RenderPassData data)
	{
		renderer.UseShader(Shader);
		
		this.Shader.SetUniform("uView", data.View);
		this.Shader.SetUniform("uProjection", data.Projection);
		this.Shader.SetUniform("uModel", GameObject.Transform.ModelMatrix);

		var mf = GameObject.GetComponent<MeshFilter>();
		if (mf != null)
		{
			foreach (var mesh in mf.meshes)
			{
				mesh?.Render(renderer, data);
			}
		}
	}

	public GameObject GameObject { get; set; }
	public void Update() { }
}