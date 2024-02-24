namespace Engine;

public abstract class Component
{
	private readonly GameObject gameObject;

	public Component(GameObject gameObject)
	{
		this.gameObject = gameObject;
	}
}