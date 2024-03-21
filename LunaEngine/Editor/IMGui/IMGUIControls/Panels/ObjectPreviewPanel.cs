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
	private readonly IInputController inputController;
	private readonly EditorViewport editorViewport;
	private Vector2 currentSize;

	public ObjectPreviewPanel(PropertiesPanel properties, IInputController inputController, IRenderer renderer)
	{
		this.inputController = inputController;
		properties.SelectionChanged += InspectorOnSelectionChanged;
		scene = new Scene("Objet Preview Scene");
		editorViewport = new EditorViewport();
		materialSphere = new GameObject();
		materialSphere.AddComponent<MeshRenderer>();
	
		scene.ActiveCamera = new MoveableEditorCamera(Vector3.UnitZ * 6, 1024 / (float) 1024);
		renderer.AddScene(scene, new Vector2D<uint>((uint) 0, (uint) 0), out var rt, true);

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
				materialSphere.GetComponent<MeshRenderer>().MaterialGuid = material.GUID;
				
				break;
			default:
				// Handle default case
				break;
		}
	}

	public string PanelName { get; set; } = "Preview";

	public void Draw(IRenderer renderer)
	{
		editorViewport.Update(PanelName,scene.ActiveCamera as IEditorCamera, scene, inputController,renderer,ref currentSize);
	}

}