using System.Numerics;

namespace Engine;

public interface IInputController
{
	event Action<float, float> MouseScroll;
	event Action<float, float> MouseMove;
	event Action<InputController.Key> KeyPress;
	event Action<InputController.Key> KeyReleased;
	void Update();
	Vector2 GetMouseDelta();
	bool IsKeyPressed(InputController.Key key);
	bool IsMousePressed(InputController.MouseButton mouseButton);
	Vector2 GetMousePosition();
	bool IsKeyPressedThisFrame(InputController.Key key);
}