using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

public class MeshFilter : Component
{
	[Inspectable(false)]
	[Serializable]
	public HashSet<Guid> meshes { get; set; } = new();

	public MeshFilter(GameObject gameObject) : base(gameObject) { }

	public void AddMesh(Guid guid)
	{
		meshes.Add(guid);
	}

	public MeshFilter Clone(MeshFilter newMeshFilter)
	{
		foreach (var mesh in meshes)
		{
			newMeshFilter.meshes.Clear();
			newMeshFilter.AddMesh(mesh);
		}

		return newMeshFilter;
	}
}