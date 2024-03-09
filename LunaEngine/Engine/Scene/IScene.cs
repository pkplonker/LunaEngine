namespace Engine;

public interface IScene : ITransform
{
	string Name { get; set; }
	ICamera? ActiveCamera { get; set; }
	void Update();
	void Clear();
	void AddGameObject(GameObject cameraGo);
}