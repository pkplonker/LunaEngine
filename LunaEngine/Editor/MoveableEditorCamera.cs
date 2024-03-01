using System.Numerics;
using Editor;
using Editor.Settings;
using Silk.NET.Input;

namespace Engine;

public class MoveableEditorCamera : EditorCamera
{

	private const string settingsCategory = "Editor Camera";
	public MoveableEditorCamera(Vector3 position, float aspectRatio) : base(position, aspectRatio) { }

	public override void Update(InputController input)
	{
		if (!input.IsMousePressed(InputController.MouseButton.Right)) return;
		float deltaTime = Time.DeltaTime;

		float speed = Settings.GetSetting("Key Speed", settingsCategory, true, 10f);
		float speedMultiplier = Settings.GetSetting("Speed Multiplier", settingsCategory, true, 5f);

		float mouseSensitivityX = Settings.GetSetting("Mouse Sensitivity X", settingsCategory, true, 0.3f);
		float mouseSensitivityY = Settings.GetSetting("Mouse Sensitivity Y", settingsCategory, true, 0.5f);

		float currentSpeed = speed;
		if (input.IsKeyPressed(InputController.Key.ShiftLeft))
		{
			currentSpeed *= speedMultiplier;
		}

		if (input.IsKeyPressed(InputController.Key.W))
		{
			Transform.Position += Transform.Forward * currentSpeed * deltaTime;
		}

		if (input.IsKeyPressed(InputController.Key.S))
		{
			Transform.Position += Transform.Back * currentSpeed * deltaTime;
		}

		if (input.IsKeyPressed(InputController.Key.A))
		{
			Transform.Position += Transform.Right * currentSpeed * deltaTime;
		}

		if (input.IsKeyPressed(InputController.Key.D))
		{
			Transform.Position += Transform.Left * currentSpeed * deltaTime;
		}

		var mouseDelta = input.GetMouseDelta();
		Transform.Rotate(-mouseDelta.X * mouseSensitivityX * Time.DeltaTime,
			mouseDelta.Y * mouseSensitivityY * Time.DeltaTime);
	}
}