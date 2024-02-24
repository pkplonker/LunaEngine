using Engine;
using ImGuiNET;

namespace Editor.Controls;

public class SettingsPanel : IPanel
{
	public string PanelName { get; set; } = "Settings";

	public void Draw(Renderer renderer)
	{
		if (ImGui.BeginPopupModal(PanelName))
		{
			ImGui.EndPopup();
		}
	}
}