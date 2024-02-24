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

	private Matrix4x4 viewMatrix;

	public Matrix4x4 ViewMatrix
	{
		get
		{
			if (isDirty)
			{
				viewMatrix = Matrix4x4.Identity * Matrix4x4.CreateFromQuaternion(Rotation) *
				             Matrix4x4.CreateScale(Scale) * Matrix4x4.CreateTranslation(Position);
				isDirty = false;
			}

			return viewMatrix;
		}
	}
}