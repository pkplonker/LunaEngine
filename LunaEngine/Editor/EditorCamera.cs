using System.Numerics;
using Editor;
using Editor.Settings;
using Silk.NET.Input;

namespace Engine;

public class EditorCamera : ICamera
{
	public Transform Transform;
	public float AspectRatio { get; set; }

	private float zoom = 45f;
	private readonly Vector3 startPosition;
	private readonly float startAspectRatio;
	private const string settingsCategory = "Editor Camera";
	public EditorCamera(Vector3 position, float aspectRatio)
	{
		startPosition = position;
		startAspectRatio = aspectRatio;
		Reset();
	}

	public void Reset()
	{
		Transform = new Transform
		{
			Position = startPosition,
			Rotation = Quaternion.Identity
		};
		AspectRatio = startAspectRatio;
	}

	public void ModifyZoom(float zoomAmount)
	{
		zoom = Math.Clamp(zoom - zoomAmount, 1.0f, 45f);
	}

	public void ModifyDirection(float xOffset, float yOffset)
	{
		Transform.Rotate(xOffset, yOffset);
	}

	public Matrix4x4 GetView()
	{
		Vector3 position = Transform.Position;
		Vector3 front = Transform.Forward;
		Vector3 up = Transform.Up;
		return Matrix4x4.CreateLookAt(position, position + front, up);
	}

	public Matrix4x4 GetProjection() =>
		Matrix4x4.CreatePerspectiveFieldOfView(MathExtensions.DegreesToRadians(zoom), AspectRatio, 0.1f, 100.0f);

	public void Update(InputController input)
	{
		if (!input.IsMousePressed(InputController.MouseButton.Right)) return;
		float deltaTime = Time.DeltaTime;

		float speed = Settings.GetSetting("Key Speed", settingsCategory,true, 10f);
		float mouseSensitivityX = Settings.GetSetting("Mouse Sensitivity X", settingsCategory,true, 0.3f);
		float mouseSensitivityY = Settings.GetSetting("Mouse Sensitivity Y", settingsCategory,true, 0.5f);

		if (input.IsKeyPressed(InputController.Key.W))
		{
			Transform.Position += Transform.Forward * speed * deltaTime;
		}

		if (input.IsKeyPressed(InputController.Key.S))
		{
			Transform.Position += Transform.Back * speed * deltaTime;
		}

		if (input.IsKeyPressed(InputController.Key.A))
		{
			Transform.Position += Transform.Right * speed * deltaTime;
		}

		if (input.IsKeyPressed(InputController.Key.D))
		{
			Transform.Position += Transform.Left * speed * deltaTime;
		}

		var mouseDelta = input.GetMouseDelta();
		Transform.Rotate(-mouseDelta.X * mouseSensitivityX * Time.DeltaTime,
			mouseDelta.Y * mouseSensitivityY * Time.DeltaTime);
	}
}