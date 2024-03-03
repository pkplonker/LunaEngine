namespace Engine;

public interface IComponent
{
	[Serializable(false)]
	public GameObject GameObject { get; protected set; }

	public void Update();
}