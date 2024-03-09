namespace Engine;

public interface IEditorCamera : ICamera
{
	public ITransform Transform { get; set; }
	public void Reset();
	public void Update(IInputController input);
}