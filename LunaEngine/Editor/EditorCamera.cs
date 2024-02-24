using System.Numerics;
using Silk.NET.Input;

namespace Engine;

public class EditorCamera : ICamera
{
	public Transform Transform;
	public float AspectRatio { get; set; }

	private float zoom = 45f;

	public EditorCamera(Vector3 position, Vector3 front, Vector3 up, float aspectRatio)
	{
		Transform = new Transform
		{
			Position = position,
			Rotation = Quaternion.Identity
		};
		AspectRatio = aspectRatio;
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
}