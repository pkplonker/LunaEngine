namespace Engine;

public interface IComponent
{
	public GameObject GameObject { get; protected set; }

	public void Update();
}