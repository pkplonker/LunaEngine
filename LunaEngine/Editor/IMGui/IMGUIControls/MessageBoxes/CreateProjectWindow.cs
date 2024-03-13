using System.Numerics;
using Engine;
using ImGuiNET;

namespace Editor.Controls;

public class CreateProjectWindow
{
	private bool ShouldShowCreateProjectModal;
	private string projectName;
	private string projectLocation;

	public void Create()
	{
		ShouldShowCreateProjectModal = true;
		Reset();
	}

	private void Reset()
	{
		projectName = "";
		projectLocation = "";
	}

	public void Draw()
	{
		if (ShouldShowCreateProjectModal)
		{
			ImGui.BeginPopupModal("Create New Project");
			ImGui.Begin("Create Project");
			ImGui.Text("Project Name:");
			ImGui.InputText("##projectname", ref projectName, 25);

			if (ImGui.Button("Choose Location"))
			{
				projectLocation = FileDialog.SelectFolderDialog();
			}

			ImGui.SameLine();
			ImGui.Text($"{projectLocation}");

			if (ImGui.Button("Create Project") && !string.IsNullOrEmpty(projectName) &&
			    !string.IsNullOrEmpty(projectLocation))
			{
				ProjectManager.CreateNewProject(projectLocation, projectName);
				Close();
			}

			ImGui.SameLine();

			if (ImGui.Button("Cancel"))
			{
				Close();
			}

			ImGui.End();
			ImGui.EndPopup();
		}
	}

	private void Close()
	{
		ShouldShowCreateProjectModal = false;
		Reset();
	}
}