using ImGuiNET;

namespace Editor.Controls;

public class ContentTreeView
{
	private readonly ContentBrowser contentBrowser;

	public ContentTreeView(ContentBrowser contentBrowser)
	{
		this.contentBrowser = contentBrowser;
	}
	public void Draw()
	{
		DrawNavTree();
	}
	
	private void DrawNavTree()
	{
		for (int i = 0; i < 10; i++)
		{
			ImGui.Text("Tes1t");
		}
	}
}