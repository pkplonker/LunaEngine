using Engine;
using ImGuiNET;

namespace Editor.Controls
{
	public class ContentBrowser : IPanel
	{
		public string PanelName { get; set; } = "Content Browser";
		private string? currentPath = string.Empty;
		private readonly ContentThumbnailView contentThumbnailView;

		public void Draw(IRenderer renderer)
		{
			if (string.IsNullOrEmpty(currentPath))
			{
				currentPath = ProjectManager.ActiveProject?.Directory ?? string.Empty;
			}

			ImGui.Begin(PanelName);
			contentThumbnailView.Draw();
			ImGui.End();
		}

		public ContentBrowser()
		{
			contentThumbnailView = new ContentThumbnailView(this);
			ProjectManager.ProjectChanged += OnProjectChanged;
		}

		private void OnProjectChanged(IProject? obj)
		{
			contentThumbnailView.CurrentPath = obj?.Directory ?? string.Empty;
		}

		public static bool IsHigherUpInTheDirectory(string? path1, string? path2)
		{
			if (string.IsNullOrEmpty(path1) || string.IsNullOrEmpty(path2)) return false;
			var absolutePath1 = Path.GetFullPath(path1);
			var absolutePath2 = Path.GetFullPath(path2);

			return absolutePath2.StartsWith(absolutePath1, StringComparison.OrdinalIgnoreCase);
		}
	}
}