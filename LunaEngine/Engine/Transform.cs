using System.Numerics;

namespace Engine;

public class Transform
{
	private bool isDirty = true;

	private Vector3 position = new Vector3(0f);

	[Serializable]
	public Vector3 Position
	{
		get => position;
		set
		{
			position = value;
			isDirty = true;
		}
	}

	private Vector3 scale = new Vector3(1f);

	[Serializable]
	public Vector3 Scale
	{
		get => scale;
		set
		{
			scale = value;
			isDirty = true;
		}
	}

	private Quaternion rotation = Quaternion.Identity;

	[Serializable]
	public Quaternion Rotation
	{
		get => rotation;
		set
		{
			rotation = value;
			isDirty = true;
		}
	}

	public void RotateByEuler(Vector3 euler)
	{
		Quaternion deltaRotation = Quaternion.CreateFromYawPitchRoll(euler.Y, euler.X, euler.Z);

		Rotation = Quaternion.Normalize(Rotation * deltaRotation);
	}

	private Matrix4x4 modelMatrix;

	public Matrix4x4 ModelMatrix
	{
		get
		{
			if (isDirty)
			{
				modelMatrix = Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) *
				              Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);
				isDirty = false;
			}

			return modelMatrix;
		}
	}

	public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);
	public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);

	public void Rotate(float xOffset, float yOffset)
	{
		Quaternion yaw = Quaternion.CreateFromAxisAngle(Vector3.UnitY, xOffset);
		Quaternion pitch = Quaternion.CreateFromAxisAngle(Vector3.UnitX, yOffset);

		Rotation = Quaternion.Normalize(yaw * Rotation * pitch);

	}
}