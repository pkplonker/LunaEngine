using Editor;
using Engine.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Engine;

public static class ProjectManager
{
	private const string TestProjectPath = @"S:\Users\pkplo\OneDrive\Desktop\LunaTestProject\LunaTestProject.json";

	static ProjectManager()
	{
		//ActiveProject ??= new Project(TestProjectPath);
	}

	public static Project? ActiveProject { get; set; }

	public static bool CreateNewProject(string dir, string name)
	{
		var combinedPath = Path.Join(dir, name);
		if (Path.Exists(combinedPath))
		{
			Logger.Warning("Not a valid directory");
			return false;
		}

		Directory.CreateDirectory(combinedPath);
		string projectFilePath = Path.Combine(combinedPath,name +".json");
		var newProject = new Project(projectFilePath) {Name = name};
		ActiveProject = newProject;
		//File.Create(projectFilePath);
		File.WriteAllText(projectFilePath, JsonConvert.SerializeObject(newProject, Formatting.Indented));

		Logger.Info($"New project created at {projectFilePath}");
		return true;
	}

	public static Project? LoadProject(string path)
	{
		if (!File.Exists(path))
		{
			Logger.Warning($"Project file not found at path: {path}");
			return null;
		}

		try
		{
			string json = File.ReadAllText(path);
			var project = JsonConvert.DeserializeObject<Project>(json);
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