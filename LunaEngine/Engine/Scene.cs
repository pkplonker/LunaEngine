using Silk.NET.Assimp;

namespace Engine;

public class Scene
{
	public HashSet<GameObject> GameObjects = new();

	public Scene()
	{
		GameObject.GameObjectCreated += OnGameObjectCreated;
	}

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
}