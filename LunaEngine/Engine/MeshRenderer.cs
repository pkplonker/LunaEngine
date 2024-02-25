using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

public class MeshRenderer : IRenderableComponent
{
	public Material Material { get; set; }

	public MeshRenderer(GameObject gameObject)
	{
		this.GameObject = gameObject;
	}

	public void Render(Renderer renderer, RenderPassData data)
	{
		if (Material == null)
		{
			//Console.WriteLine("Trying to render without valid shader");
			return;
		}

		renderer.UseMaterial(Material, data, GameObject.Transform.ModelMatrix);

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