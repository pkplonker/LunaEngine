using Engine;
using Engine.Logging;
using ImGuiNET;
using File = Silk.NET.Assimp.File;

namespace Editor.Controls
{
	public class ContentBrowser : IPanel
	{
		public string PanelName { get; set; } = "Content Browser";
		private string? currentPath = string.Empty;

		public void Draw(IRenderer renderer)
		{
			if (string.IsNullOrEmpty(currentPath))
			{
				currentPath = ProjectManager.ActiveProject?.Directory ?? string.Empty;
			}

			ImGui.Begin(PanelName);
			DrawNav();
			DrawConent();

			ImGui.End();
		}

		private void DrawNav()
		{
			BackButton();
			ImGui.SameLine();
			ImGui.Text(currentPath);
		}

		private void BackButton()
		{
			if (ImGui.Button("Back"))
			{
				var parent = Directory.GetParent(currentPath).FullName;
				if (IsHigherUpInTheDirectory(ProjectManager.ActiveProject?.Directory, parent))
				{
					currentPath = parent;
				}
			}
		}

		private void DrawConent()
		{
			if (string.IsNullOrEmpty(currentPath))
			{
				return;
			}

			ImGui.BeginChild("##ContentArea");

			foreach (var dir in Directory.GetDirectories(currentPath))
			{
				if (dir == currentPath) continue;
				if (ImGui.Selectable(Path.GetRelativePath(currentPath, dir)))
				{
					currentPath = dir;
				}
			}

			Metadata? metadata = null;
			foreach (var file in Directory.GetFiles(currentPath)
				         .Where(x => Path.GetExtension(x).Trim('.') == Metadata.MetadataFileExtension))
			{
				DrawMetaData(file);
			}

			ImGui.EndChild();
		}

		private static void DrawMetaData(string file)
		{
			Metadata? metadata;
			metadata = Metadata.CreateMetadataFromMetadataFile(file);
			if (metadata != null && ImGui.Selectable(metadata.Name))
			{
				Logger.Info($"Selected{file}");
			}
		}

		public static bool IsHigherUpInTheDirectory(string path1, string path2)
		{
			var absolutePath1 = Path.GetFullPath(path1);
			var absolutePath2 = Path.GetFullPath(path2);

			return absolutePath2.StartsWith(absolutePath1, StringComparison.OrdinalIgnoreCase);
		}
	}
}