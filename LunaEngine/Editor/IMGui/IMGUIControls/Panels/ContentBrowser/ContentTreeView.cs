using ImGuiNET;
using System.IO;
using System.Collections.Generic;
using Engine;

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
		var dir = ProjectManager.ActiveProject?.Directory;
		if (!string.IsNullOrEmpty(ProjectManager.ActiveProject?.Directory))
		{
			DrawDirectoryTree(dir);
		}
	}

	private void DrawDirectoryTree(string directoryPath)
	{
		foreach (var directory in new DirectoryInfo(directoryPath).GetDirectories())
		{
			var isDirectoryEmpty = !directory.GetDirectories().Any();
			bool nodeOpen = false;

			if (isDirectoryEmpty)
			{
				ImGui.Bullet();
				ImGui.SameLine();
				if (ImGui.Selectable(directory.Name, false, ImGuiSelectableFlags.None))
				{
					contentBrowser.CurrentPath = directory.FullName;
				}
			}
			else
			{
				nodeOpen = ImGui.TreeNodeEx(directory.Name, ImGuiTreeNodeFlags.None);

				if (ImGui.IsItemClicked())
				{
					contentBrowser.CurrentPath = directory.FullName;
				}

				if (nodeOpen)
				{
					DrawDirectoryTree(directory.FullName);
					ImGui.TreePop();
				}
			}
		}
	}
}