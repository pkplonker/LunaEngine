using Engine.Logging;

namespace Engine;

public enum PrimitiveType
{
	Empty,
	EmptyMesh,
	Cube,
	Sphere,
	Plane
}

public static class GameObjectFactory
{
	private const string DEFAULT_SHADER_KEY = "DefaultShader";
	private const string DEFAULT_MESH_KEY = "CubeMesh";
	private const string DEFAULT_SPHERE_KEY = "SphereMesh";
	private const string DEFAULT_PLANE_KEY = "PlaneMesh";
	private static readonly char[] separator = new[] {'(', ')'};
	private const string SPHERE_NAME = "New Sphere";
	private const string CUBE_NAME = "New Cube";
	private const string PLANE_NAME = "New Plane";

	public static GameObject CreateCamera(IScene activeScene, bool isPerspective = true)
	{
		var go = CreatePrimitive(activeScene);
		if (isPerspective)
			go.AddComponent<PerspectiveCamera>();
		else go.AddComponent<OrthographicCamera>();
		go.Transform.SetParent(activeScene);
		return go;
	}

	public static GameObject? CreatePrimitive(IScene activeScene, PrimitiveType type = PrimitiveType.Empty)
	{
		var go = new GameObject();

		try
		{
			if (type != PrimitiveType.Empty)
			{
				switch (type)
				{
					case PrimitiveType.Cube:
						CreateMesh(go, DEFAULT_SHADER_KEY, DEFAULT_MESH_KEY);
						go.Name = CUBE_NAME;
						break;
					case PrimitiveType.Sphere:
						CreateMesh(go, DEFAULT_SHADER_KEY, DEFAULT_SPHERE_KEY);
						go.Name = SPHERE_NAME;
						break;
					case PrimitiveType.Plane:
						CreateMesh(go, DEFAULT_SHADER_KEY, DEFAULT_PLANE_KEY);
						go.Name = PLANE_NAME;
						break;
					default:
						throw new ArgumentOutOfRangeException(nameof(type), type, null);
				}
			}
		}
		catch (Exception e)
		{
			Logger.Error($"Failed to create primitive mesh {e}");
			return null;
		}

		go.Transform.SetParent(activeScene);

		SetName(activeScene, go);
		return go;
	}

	private static void SetName(IScene activeScene, GameObject go)
	{
		var defaultName = go.Name;

		var maxNumber = 0;
		foreach (var parts in activeScene.ChildrenAsGameObjectsRecursive
			         .Where(x => x.Name.StartsWith(defaultName))
			         .Select(x => x.Name)
			         .Select(name => name.Split(separator, StringSplitOptions.RemoveEmptyEntries)))
		{
			if (parts.Length == 2 && int.TryParse(parts[1], out var number))
			{
				maxNumber = Math.Max(maxNumber, number);
			}
		}

		go.Name = $"{defaultName} ({maxNumber + 1})";
	}

	private static void CreateMesh(GameObject go, string shaderKey, string meshKey)
	{
		var mf = go.AddComponent<MeshFilter>();
		var mr = go.AddComponent<MeshRenderer>();
		Shader? shader = null;
		Mesh? mesh = null;
		if (ResourceManager.TryGetResourceByGuid(CoreAssets.GetMetadata(shaderKey).GUID,
			    out shader))
		{
			if (shader != null)
			{
				mr.MaterialGuid = shader.GUID;
			}
		}
		else
		{
			throw new ArgumentException(shaderKey);
		}

		if (ResourceManager.TryGetResourceByGuid(CoreAssets.GetMetadata(meshKey).GUID,
			    out mesh))
		{
			if (mesh != null)
			{
				mf.AddMesh(mesh.GUID);
			}
			else
			{
				throw new ArgumentException(meshKey);
			}
		}
	}

	public static GameObject CreateMesh(IScene activeScene) =>
		CreatePrimitive(activeScene, PrimitiveType.EmptyMesh);
}