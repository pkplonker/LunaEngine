using System.Numerics;
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
			DrawContent();

			ImGui.End();
		}

		private const int ORIGINAL_SIZE = 200;
		private int size = ORIGINAL_SIZE;
		private const float ORIGINAL_PADDING = 30;
		private float padding => ORIGINAL_PADDING / ((float) ORIGINAL_SIZE / size);
		private Vector2 imageSize => new(size, size);

		private void DrawNav()
		{
			BackButton();
			ImGui.SameLine();
			ImGui.Text(currentPath);
			ImGui.SameLine();
			ImGui.Text("Thumbnail size");
			ImGui.SetNextItemWidth(150);
			ImGui.SameLine();
			ImGui.SliderInt("##sizeSlider", ref size, 50, 500);
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

		private void DrawContent()
		{
			if (string.IsNullOrEmpty(currentPath))
			{
				return;
			}

			ImGui.BeginChild("##ContentArea");
			var parent = Directory.GetParent(currentPath)?.FullName;

			if (IsHigherUpInTheDirectory(ProjectManager.ActiveProject?.Directory, parent))
			{
				if (ImGui.Selectable("..."))
				{
					currentPath = parent;
				}
			}

			foreach (var dir in Directory.GetDirectories(currentPath))
			{
				if (dir == currentPath) continue;
				if (ImGui.Selectable(Path.GetRelativePath(currentPath, dir)))
				{
					currentPath = dir;
				}
			}

			var numberPerRow = NumberPerRow();
			var count = 0;

			foreach (var file in Directory.GetFiles(currentPath)
				         .Where(x => Path.GetExtension(x).Trim('.') == Metadata.MetadataFileExtension))
			{
				if (count % numberPerRow == 0)
				{
					if (count > 0) ImGui.NewLine();
					ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padding / 2);
				}
				else
				{
					ImGui.SameLine();
					ImGui.SetCursorPosX(ImGui.GetCursorPosX() + padding);
				}

				DrawMetaData(file);
				count++;
			}

			ImGui.EndChild();
		}

		private int NumberPerRow()
		{
			var availableWidth = ImGui.GetWindowWidth() - padding;
			float calculatedWidth = imageSize.X + padding;
			int numberPerRow = (int) Math.Floor(availableWidth / calculatedWidth);
			numberPerRow = numberPerRow >= 1 ? numberPerRow : 1;
			return numberPerRow;
		}

		private void DrawMetaData(string file)
		{
			Metadata? metadata;
			metadata = Metadata.CreateMetadataFromMetadataFile(file);

			ImGui.BeginGroup();

			if (metadata != null && ImGui.ImageButton("Select Texture",
				    IconLoader.LoadIcon(@"resources/icons/AddTexture.png".MakeAbsolute()),
				    imageSize))
			{
				Logger.Info($"Selected {file.MakeProjectRelative()}");
			}

			ImGui.PushItemWidth(size);
			ImGui.TextWrapped(metadata?.Name ?? "Unknown");
			ImGui.PopItemWidth();

			ImGui.EndGroup();
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