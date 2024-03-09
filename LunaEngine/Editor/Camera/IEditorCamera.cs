namespace Engine;

public interface IEditorCamera : ICamera
{
	public Transform Transform { get; set; }
	public void Reset();
	public void Update(IInputController input);
}