using System.Numerics;
using Silk.NET.Input;

namespace Engine;

public class Camera : ICamera
{
	public Vector3 Position { get; set; }
	public Vector3 Front { get; set; }

	public Vector3 Up { get; private set; }
	public float AspectRatio { get; set; }

	public float Yaw { get; set; } = -90f;
	public float Pitch { get; set; }

	private float Zoom = 45f;

	public Camera(Vector3 position, Vector3 front, Vector3 up, float aspectRatio)
	{
		Position = position;
		AspectRatio = aspectRatio;
		Front = front;
		Up = up;
	}

	public void ModifyZoom(float zoomAmount)
	{
		Zoom = Math.Clamp(Zoom - zoomAmount, 1.0f, 45f);
	}

	public void ModifyDirection(float xOffset, float yOffset)
	{
		Yaw += xOffset;
		Pitch -= yOffset;

		Pitch = Math.Clamp(Pitch, -89f, 89f);

		var cameraDirection = Vector3.Zero;
		cameraDirection.X = MathF.Cos(MathExtensions.DegreesToRadians(Yaw)) *
		                    MathF.Cos(MathExtensions.DegreesToRadians(Pitch));
		cameraDirection.Y = MathF.Sin(MathExtensions.DegreesToRadians(Pitch));
		cameraDirection.Z = MathF.Sin(MathExtensions.DegreesToRadians(Yaw)) *
		                    MathF.Cos(MathExtensions.DegreesToRadians(Pitch));

		Front = Vector3.Normalize(cameraDirection);
	}

	public Matrix4x4 GetView() => Matrix4x4.CreateLookAt(Position, Position + Front, Up);

	public Matrix4x4 GetProjection() =>
		Matrix4x4.CreatePerspectiveFieldOfView(MathExtensions.DegreesToRadians(Zoom), AspectRatio, 0.1f, 100.0f);
}