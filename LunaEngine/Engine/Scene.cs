using Silk.NET.Assimp;

namespace Engine;

public class Scene
{
	public HashSet<GameObject> GameObjects = new();

	public Scene()
	{
	}

	public ICamera ActiveCamera { get; set; }

	private void OnGameObjectCreated(GameObject obj)
	{
		if (obj == null)
		{
			return;
		}

		GameObjects.Add(obj);
	}

	public void Update()
	{
		foreach (var go in GameObjects)
		{
			go?.Update();
		}
	}

	public void AddGameObject(GameObject go)
	{
		if (go == null)
		{
			return;
		}

		GameObjects.Add(go);
	}

	public void Clear()
	{
		GameObjects.Clear();
	}
}