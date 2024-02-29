namespace Editor;

public static class ProjectManager
{
	private const string TestProjectPath = @"S:\Users\pkplo\OneDrive\Desktop\LunaTestProject\LunaTestProject.json";

	static ProjectManager()
	{
		ActiveProject ??= new Project(TestProjectPath);
	}

	public static Project? ActiveProject { get; set; }
}