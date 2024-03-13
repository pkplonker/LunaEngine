using Engine;
using ImGuiNET;
using System;
using Engine.Logging;

namespace Editor.Controls;

public class ProjectPanel : IPanel
{
	public string PanelName { get; set; } = "Project";

	private readonly IInputController inputController;

	public ProjectPanel(IInputController inputController)
	{
		this.inputController = inputController;
	}

	public void Draw(IRenderer renderer)
	{
		ImGui.Begin(PanelName);
		var project = ProjectManager.ActiveProject;
		if (project == null)
		{
			ImGui.Text($"No active project");
		}
		else
		{
			ImGui.TextWrapped($"Name: {project.Name}");
			ImGui.TextWrapped($"Name: {project.Directory}");
			ImGui.TextWrapped($"Name: {project.AssetsDirectory}");
#if DEVELOP
			ImGui.TextWrapped($"Core Assets Directory: {project.CoreAssetsDirectory}");
#endif
		}

		ImGui.End();
	}
}