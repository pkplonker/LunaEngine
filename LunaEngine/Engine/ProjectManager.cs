using Editor;
using Engine.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Engine;

public static class ProjectManager
{
	private static Project? activeProject;
	private const string TestProjectPath = @"S:\Users\pkplo\OneDrive\Desktop\LunaTestProject\LunaTestProject.json";
	public static event Action<Project?> ProjectChanged;

	static ProjectManager()
	{
		//ActiveProject ??= new Project(TestProjectPath);
	}

	public static Project? ActiveProject
	{
		get => activeProject;
		set
		{
			if (activeProject != value)
			{
				activeProject = value;
				ProjectChanged?.Invoke(ActiveProject);
			}
		}
	}

	public static string ProjectExtension { get; } = ".LunaProject";

	public static bool CreateNewProject(string dir, string name, bool setActive = true)
	{
		var combinedPath = Path.Join(dir, name);
		if (Path.Exists(combinedPath))
		{
			Logger.Warning("Not a valid directory");
			return false;
		}

		Directory.CreateDirectory(combinedPath);
		string projectFilePath = Path.Combine(combinedPath, name + ProjectExtension);
		var newProject = new Project(projectFilePath) {Name = name};
		ActiveProject = newProject;
		File.WriteAllText(projectFilePath, JsonConvert.SerializeObject(newProject, Formatting.Indented));

		Logger.Info($"New project created at {projectFilePath}");
		if (setActive) ActiveProject = newProject;
		return true;
	}

	public static Project? LoadProject(string? path)
	{
		void Warning()
		{
			Logger.Warning($"Project file not found at path: {path}");
		}

		if (string.IsNullOrEmpty(path))
		{
			Warning();
			return null;
		}

		if (!File.Exists(path))
		{
			Warning();
			return null;
		}

		try
		{
			string json = File.ReadAllText(path);
			Project? project = JsonConvert.DeserializeObject<Project>(json);
			ActiveProject = project;
			return project;
		}
		catch (JsonException ex)
		{
			Logger.Error($"Failed to load project from path {path}: {ex.Message}");
			return null;
		}
	}
}