using System.Numerics;
using Engine;
using ImGuiNET;

namespace Editor.Controls;

public class InspectorPanel : IPanel
{
	private readonly EditorImGuiController controller;
	public static event Action<IPanel> RegisterPanel;

	public InspectorPanel(EditorImGuiController controller)
	{
		RegisterPanel?.Invoke(this);
		this.controller = controller;
	}

	public string PanelName { get; set; } = "Inspector";

	public void Draw(Renderer renderer)
	{
		ImGui.Begin(PanelName);
		if (controller.SelectedGameObject != null)
		{
			var go = controller.SelectedGameObject;
			ImGui.Text(go.Name);
			ImGui.Text(go.Guid.ToString());
			ImGuiHelpers.UndoableCheckbox("Enabled", () => go.Enabled, val => go.Enabled = val,
				"GameObject Enabled Toggled");
			ImGuiHelpers.DrawTransform(go.Transform);
		}

		ImGui.End();
	}
}