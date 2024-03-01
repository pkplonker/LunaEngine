using System.Numerics;
using Engine;
using ImGuiNET;
using Silk.NET.Maths;

namespace Editor.Controls;

public class ObjectPreviewPanel : IPanel
{
	private Scene scene;
	private GameObject materialSphere;
	private Vector2 previousSize;

	public ObjectPreviewPanel(InspectorPanel inspector)
	{
		inspector.SelectionChanged += InspectorOnSelectionChanged;
		scene = new Scene();

		materialSphere = new GameObject();
		materialSphere.AddComponent<RotateComponent>();
		materialSphere.AddComponent<MeshFilter>()
			?.AddMesh(ResourceManager.GetMesh(@"Resources/Core/models/TestSphere.obj".MakeAbsolute()));
		scene.ActiveCamera = new EditorCamera(Vector3.UnitZ * 6, 1024 / (float) 1024);
	}

	private void InspectorOnSelectionChanged(object obj)
	{
		scene.Clear();
		switch (obj)
		{
			case GameObject gameObject:
				scene.AddGameObject(gameObject);
				break;
			case Material material:
				scene.AddGameObject(materialSphere);
				materialSphere.GetComponent<MeshRenderer>().Material = material;
				break;
			default:
				// Handle default case
				break;
		}
	}

	public string PanelName { get; set; } = "Preview";

	public void Draw(Renderer renderer)
	{
		ImGui.Begin(PanelName);
		var size = ImGui.GetContentRegionAvail();
		if (size != previousSize)
		{
			renderer.SetRenderTargetSize(renderer.inspectorRenderTarget, new Vector2D<float>(size.X, size.Y));
			previousSize = size;
		}

		ImGui.Image((IntPtr) renderer.inspectorRenderTarget.texture.Handle, new Vector2(size.X, size.Y), Vector2.Zero,
			Vector2.One,
			Vector4.One,
			Vector4.Zero);
		ImGui.End();
	}
}