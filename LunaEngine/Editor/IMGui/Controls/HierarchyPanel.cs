using System.Numerics;
using Engine;
using ImGuiNET;

namespace Editor.Controls;

public class HierarchyPanel : IPanel
{
	private readonly EditorImGuiController controller;
	private readonly InputController inputController;
	private readonly RenamingHelper renamingHelper;

	public HierarchyPanel(EditorImGuiController controller, InputController inputController)
	{
		this.controller = controller;
		this.inputController = inputController;
		renamingHelper = new RenamingHelper();
	}

	public string PanelName { get; set; } = "Hierarchy";

	public void Draw(Renderer renderer)
	{
		if (inputController.IsKeyPressed(InputController.Key.F2))
		{
			renamingHelper.RequestRename(controller.SelectedGameObject);
		}

		ImGui.Begin(PanelName);

		var scene = SceneController.ActiveScene;
		if (scene != null)
		{
			DrawChildren(scene);

			renamingHelper.DrawRenamePopup();
		}

		ImGui.End();
	}

	private void DrawChildren(Transform transform)
	{
		foreach (var gameobject in transform.GetChildrenAsGameObjects())
		{
			DrawGameObjectNode(gameobject);
		}
	}

	private void DrawGameObjectNode(GameObject go)
	{
		ImGuiTreeNodeFlags flags = ImGuiTreeNodeFlags.OpenOnArrow |
		                           ImGuiTreeNodeFlags.SpanAvailWidth;
		var selected = go == controller.SelectedGameObject;
		if (selected)
		{
			flags |= ImGuiTreeNodeFlags.Selected;
			ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.1764705926179886f, 0.3490196168422699f,
				0.5764706134796143f,
				0.8619999885559082f));
		}

		if (!go.Transform.HasChildren)
		{
			flags |= ImGuiTreeNodeFlags.Leaf;
		}

		bool nodeOpen = ImGui.TreeNodeEx($"{go.Name}##{go.Guid}", flags);

		if (selected)
		{
			ImGui.PopStyleColor();
		}

		if (ImGui.IsItemClicked() && !ImGui.IsItemToggledOpen())
		{
			HandleGameObjectSelection(go);
		}

		if (nodeOpen)
		{
			DrawChildren(go.Transform);
			ImGui.TreePop();
		}
	}

	private void HandleGameObjectSelection(GameObject go)
	{
		controller.SelectedGameObject = go;
	}
}