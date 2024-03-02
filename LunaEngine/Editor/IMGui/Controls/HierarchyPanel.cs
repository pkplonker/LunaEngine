using System.Numerics;
using Engine;
using ImGuiNET;

namespace Editor.Controls;

public class HierarchyPanel : IPanel
{
	private readonly EditorImGuiController controller;

	public HierarchyPanel(EditorImGuiController controller)
	{
		this.controller = controller;
	}

	public string PanelName { get; set; } = "Hierarchy";

	public void Draw(Renderer renderer)
	{
		ImGui.Begin(PanelName);

		var scene = SceneController.ActiveScene;
		if (scene != null)
		{
			foreach (var go in scene.GameObjects)
			{
				
				DrawGameObjectNode(go);
			}
		}

		ImGui.End();
	}

	private void DrawGameObjectNode(GameObject go)
	{
		if (ImGui.TreeNode($"{go.Name}##{go.Guid}"))
		{
			if (ImGui.IsItemClicked())
			{
				HandleGameObjectSelection(go);
			}
			// foreach (var child in go.Children)
			// {
			// 	DrawGameObjectNode(child);
			// }

			ImGui.TreePop();
		}
	}

	private void HandleGameObjectSelection(GameObject go)
	{
		controller.SelectedGameObject = go;
	}
}