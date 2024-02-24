using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

public class MeshFilter : Component
{
	public HashSet<Mesh?> meshes = new();
	public MeshFilter(GameObject gameObject) : base(gameObject) { }

	public void AddMesh(Mesh? mesh)
	{
		meshes.Add(mesh);
	}
}