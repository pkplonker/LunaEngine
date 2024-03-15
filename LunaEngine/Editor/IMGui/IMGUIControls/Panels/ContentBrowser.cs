using Engine;
using ImGuiNET;

namespace Editor.Controls
{
	public class ContentBrowser : IPanel
	{
		public string PanelName { get; set; } = "Content Browser";

		public void Draw(IRenderer renderer)
		{
			ImGui.Begin(PanelName);
			ImGui.Text("Testing");
			ImGui.End();
		}
	}
}