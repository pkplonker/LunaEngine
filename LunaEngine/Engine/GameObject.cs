namespace Engine;

public class GameObject
{
	private HashSet<Component> components = new ();
	public Transform transform { get; private set; }

	public GameObject()
	{
		transform = new Transform();
	}

	public Component? GetComponent<T>() where T : Component
	{
		return components.First(x => x is T) as T ?? null;
	}
	
	public bool TryGetComponent<T>(out Component? component) where T : Component
	{
		component = GetComponent<T>();
		return component != null;
	}

	public void AddComponent<T>() where T : Component
	{
		T component = null;

		var constructor = typeof(T).GetConstructor(new[] {typeof(GameObject)});
		if (constructor != null)
		{
			component = (T) constructor.Invoke(new object[] {this});
		}

		if (component != null)
		{
			components.Add(component);
		}
	}
}