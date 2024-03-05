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
	private readonly InputController inputController;

	public ObjectPreviewPanel(InspectorPanel inspector, InputController inputController)
	{
		this.inputController = inputController;
		inspector.SelectionChanged += InspectorOnSelectionChanged;
		scene = new Scene();

		materialSphere = new GameObject();
		materialSphere.AddComponent<RotateComponent>();
		// materialSphere.AddComponent<MeshFilter>()
		// 	?.AddMesh(ResourceManager.GetMesh(@"Resources/models/TestSphere.obj".MakeAbsolute()));
		scene.ActiveCamera = new MoveableEditorCamera(Vector3.UnitZ * 6, 1024 / (float) 1024);
	}

	private void InspectorOnSelectionChanged(object obj)
	{
		scene.Clear();
		switch (obj)
		{
			case GameObject gameObject:
				var dummyGo = new GameObject();
				dummyGo.Transform.SetParent(scene);
				var mf = gameObject.GetComponent<MeshFilter>();
				var mr = gameObject.GetComponent<MeshRenderer>();
				var dmf = dummyGo.AddComponent<MeshFilter>();
				var dmr = dummyGo.AddComponent<MeshRenderer>();
				if (mf != null)
				{
					mf.Clone(dmf);
				}

				if (mr != null)
				{
					mr.Clone(dmr);
				}

				break;
			case Material material:
				materialSphere.Transform.SetParent(scene);
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
		if (ImGui.IsWindowFocused() || (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right)))
		{
			ImGui.SetWindowFocus();
			((MoveableEditorCamera) scene.ActiveCamera)?.Update(inputController);
		}

		var size = ImGui.GetContentRegionAvail();
		renderer.AddScene(scene, new Vector2D<uint>((uint) size.X, (uint) size.Y), out var rt, true);
		if (size != previousSize)
		{
			renderer.SetRenderTargetSize(scene, new Vector2D<float>(size.X, size.Y));
			previousSize = size;
		}

		if (rt != null && rt is FrameBufferRenderTarget fbrtt)
		{
			ImGui.Image(fbrtt.GetTextureHandlePtr(), new Vector2(size.X, size.Y), Vector2.Zero,
				Vector2.One,
				Vector4.One,
				Vector4.Zero);
		}

		ImGui.End();
	}
}