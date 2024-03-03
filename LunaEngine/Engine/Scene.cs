using Silk.NET.Assimp;

namespace Engine;

public class Scene : Transform
{
	public IReadOnlyList<Transform> AllTransforms => GetChildren();

	public List<GameObject?> AllGameObjects =>
		AllTransforms.Where(x => x.GameObject != null).Select(x => x.GameObject).ToList();

	public Scene() : base(null) { }

	public ICamera? ActiveCamera { get; set; }

	public void Update()
	{
		foreach (var go in AllGameObjects)
		{
			go?.Update();
		}
	}

	public void Clear()
	{
		ClearRelationshipsRecursive(this);
	}

	private void ClearRelationshipsRecursive(Transform node)
	{
		if (node == null) return;

		foreach (var child in node.GetChildren())
		{
			node.SetParent(null);
			ClearRelationshipsRecursive(child);
		}
		children.Clear();
	}

	public void AddGameObject(GameObject cameraGo)
	{
		cameraGo.Transform.SetParent(this);
	}
}