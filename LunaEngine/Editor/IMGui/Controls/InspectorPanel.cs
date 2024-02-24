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
			ImGui.Text(controller.SelectedGameObject?.Name);
			ImGui.Text(controller.SelectedGameObject?.Guid.ToString());

		}

		ImGui.End();
	}
}