using System.Numerics;
using Silk.NET.Input;

namespace Engine;

public class Camera
{
	private readonly IInputContext context;
	private Vector3 CameraPosition = new Vector3(0.0f, 0.0f, 3.0f);
	private Vector3 CameraFront = new Vector3(0.0f, 0.0f, -1.0f);
	private Vector3 CameraUp = Vector3.UnitY;
	private Vector3 CameraDirection = Vector3.Zero;
	private float CameraYaw = -90f;
	private float CameraPitch = 0f;
	private float CameraZoom = 45f;
	private static Vector2 LastMousePosition;
	private readonly IKeyboard primaryKeyboard;

	public Camera(IInputContext context)
	{
		this.context = context;
		primaryKeyboard = context.Keyboards?.FirstOrDefault();
	}

	public void OnUpdate(double deltaTime)
	{
		var moveSpeed = 2.5f * (float) deltaTime;

		if (primaryKeyboard.IsKeyPressed(Key.W))
		{
			//Move forwards
			CameraPosition += moveSpeed * CameraFront;
		}

		if (primaryKeyboard.IsKeyPressed(Key.S))
		{
			//Move backwards
			CameraPosition -= moveSpeed * CameraFront;
		}

		if (primaryKeyboard.IsKeyPressed(Key.A))
		{
			//Move left
			CameraPosition -= Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * moveSpeed;
		}

		if (primaryKeyboard.IsKeyPressed(Key.D))
		{
			//Move right
			CameraPosition += Vector3.Normalize(Vector3.Cross(CameraFront, CameraUp)) * moveSpeed;
		}
	}

	private void OnMouseMove(IMouse mouse, Vector2 position)
	{
		var lookSensitivity = 0.1f;
		if (LastMousePosition == default)
		{
			LastMousePosition = position;
		}
		else
		{
			var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
			var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
			LastMousePosition = position;

			CameraYaw += xOffset;
			CameraPitch -= yOffset;

			CameraPitch = Math.Clamp(CameraPitch, -89.0f, 89.0f);

			CameraDirection.X = MathF.Cos(MathExtensions.DegreesToRadians(CameraYaw)) *
			                    MathF.Cos(MathExtensions.DegreesToRadians(CameraPitch));
			CameraDirection.Y = MathF.Sin(MathExtensions.DegreesToRadians(CameraPitch));
			CameraDirection.Z = MathF.Sin(MathExtensions.DegreesToRadians(CameraYaw)) *
			                    MathF.Cos(MathExtensions.DegreesToRadians(CameraPitch));
			CameraFront = Vector3.Normalize(CameraDirection);
		}
	}

	private void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
	{
		//We don't want to be able to zoom in too close or too far away so clamp to these values
		CameraZoom = Math.Clamp(CameraZoom - scrollWheel.Y, 1.0f, 45f);
	}

	public Matrix4x4 GetView()
	{
		return Matrix4x4.CreateLookAt(CameraPosition, CameraPosition + CameraFront, CameraUp);
	}

	public Matrix4x4 GetProjection()
	{
		return Matrix4x4.CreatePerspectiveFieldOfView(MathExtensions.DegreesToRadians(CameraZoom),
			Application.WINDOW_SIZE_X / Application.WINDOW_SIZE_Y, 0.1f, 100.0f);
	}
}