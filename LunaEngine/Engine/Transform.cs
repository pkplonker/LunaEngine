using System.Numerics;

namespace Engine;

public class Transform
{
	public GameObject? GameObject { get; private set; }
	private bool isDirty = true;
	private Vector3 position = new(0f);
	protected HashSet<Transform> children = new();
	private Transform? parent = null;

	public Transform(GameObject? go)
	{
		this.GameObject = go;
	}

	public void SetParent(Transform? newParent)
	{
		parent?.children.Remove(this);

		parent = newParent;

		if (newParent != null)
		{
			newParent.children.Add(this);
		}
	}

	public IReadOnlyList<Transform> GetChildren()
	{
		return children.SelectMany(child => child.GetChildren()).Concat(children).ToList();
	}

	public IEnumerable<GameObject> GetChildrenAsGameObjectsRecurisve()
	{
		return children.SelectMany(child => child.GetChildrenAsGameObjectsRecurisve())
			.Concat(children.Select(child => child.GameObject)).WhereNotNull().Distinct()
			.ToList() ?? Enumerable.Empty<GameObject>();
	}
	
	public IEnumerable<GameObject> GetChildrenAsGameObjects()
	{
		return children.Select(x => x.GameObject).WhereNotNull();
	}

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
	public Vector3 Back => Vector3.Transform(Vector3.UnitZ, Rotation);

	public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation);
	public Vector3 Down => Vector3.Transform(-Vector3.UnitY, Rotation);

	public Vector3 Left => Vector3.Transform(Vector3.UnitX, Rotation);
	public Vector3 Right => Vector3.Transform(-Vector3.UnitX, Rotation);
	public bool HasChildren => children.Any();

	public void Rotate(float xOffset, float yOffset)
	{
		Quaternion yaw = Quaternion.CreateFromAxisAngle(Vector3.UnitY, xOffset);
		Quaternion pitch = Quaternion.CreateFromAxisAngle(Vector3.UnitX, yOffset);

		Rotation = Quaternion.Normalize(yaw * Rotation * pitch);
	}

	public void Translate(Vector3 translation)
	{
		position += translation;
	}
}