using System.Numerics;
using Engine;
using Engine.Logging;
using ImGuiNET;

namespace Editor.Controls;

public class ContentThumbnailView
{
	private ContentBrowser contentBrowser;
	public string? CurrentPath { get; set; } = string.Empty;
	private const float ORIGINAL_PADDING = 30;
	private Vector2 imageSize => new(size, size);
	private int size = ORIGINAL_SIZE;
	private const int ORIGINAL_SIZE = 200;
	private float padding => ORIGINAL_PADDING / ((float) ORIGINAL_SIZE / size);

	public ContentThumbnailView(ContentBrowser contentBrowser)
	{
		this.contentBrowser = contentBrowser;
		if (string.IsNullOrEmpty(CurrentPath))
		{
			CurrentPath = ProjectManager.ActiveProject?.Directory ?? string.Empty;
		}
	}

	public void Draw()
	{
		DrawNav();
		DrawContent();
	}

	private void DrawNav()
	{
		ImGui.Text("Thumbnail size");
		ImGui.SetNextItemWidth(150);
		ImGui.SameLine();
		ImGui.SliderInt("##sizeSlider", ref size, 50, 500);
	}

	private void DrawContent()
	{
		if (string.IsNullOrEmpty(CurrentPath))
		{
			return;
		}

		ImGui.BeginChild("##ContentArea");
		var parent = Directory.GetParent(CurrentPath)?.FullName;

		var numberPerRow = NumberPerRow();
		var count = 0;
		var directories = Directory.GetDirectories(CurrentPath).ToList();

		if (ContentBrowser.IsHigherUpInTheDirectory(ProjectManager.ActiveProject?.Directory, parent))
		{
			directories.Insert(0, null);
		}

		foreach (var dir in directories.Where(dir => dir != CurrentPath))
		{
			CalculatePosition(count++, numberPerRow);
			DrawFolder(dir);
		}

		foreach (var file in Directory.GetFiles(CurrentPath)
			         .Where(x => Path.GetExtension(x).Trim('.') == Metadata.MetadataFileExtension))
		{
			CalculatePosition(count++, numberPerRow);
			DrawMetaData(file);
		}

		ImGui.EndChild();
	}

	private void CalculatePosition(int count, int numberPerRow)
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
	}

	private int NumberPerRow()
	{
		var availableWidth = ImGui.GetWindowWidth() - padding;
		float calculatedWidth = imageSize.X + padding;
		int numberPerRow = (int) Math.Floor(availableWidth / calculatedWidth);
		numberPerRow = numberPerRow >= 1 ? numberPerRow : 1;
		return numberPerRow;
	}

	private void DrawMetaData(string path)
	{
		Metadata? metadata;
		metadata = Metadata.CreateMetadataFromMetadataFile(path);

		ImGui.BeginGroup();

		if (metadata != null && ImGui.ImageButton($"{path}",
			    IconLoader.LoadIcon(FileExtensions.MakeAbsolute(GetIcon(metadata))), imageSize))
		{
			Logger.Info($"Selected {path.MakeProjectRelative()}");
		}

		ImGui.PushItemWidth(size);
		ImGui.TextWrapped(metadata?.Name ?? "Unknown");
		ImGui.PopItemWidth();

		ImGui.EndGroup();
	}

	private void DrawFolder(string? path)
	{
		var displayPath = path ?? Directory.GetParent(CurrentPath)?.FullName ?? "Unknown";
		var iconPath = IconLoader.LoadIcon(FileExtensions.MakeAbsolute(GetIcon(null)));

		ImGui.BeginGroup();

		if (ImGui.ImageButton(displayPath.MakeProjectRelative(), iconPath, imageSize))
		{
			if (displayPath != null)
			{
				CurrentPath = displayPath;
			}
		}

		ImGui.PushItemWidth(size);
		string folderName = path != null ? Path.GetFileName(path) : "...";
		ImGui.TextWrapped(folderName);
		ImGui.PopItemWidth();

		ImGui.EndGroup();
	}

	private static string GetIcon(Metadata? metadata) => @"resources/icons/AddTexture.png";
}