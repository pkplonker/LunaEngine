namespace Engine;

public class Project
{
	public string ProjectFilePath { get; set; }

	public string? Directory
	{
		get
		{
			if (!string.IsNullOrEmpty(ProjectFilePath))
			{
				return System.IO.Path.GetDirectoryName(ProjectFilePath);
			}

			return string.Empty;
		}
	}

	public string Name { get; set; }
	public string? AssetsDirectory { get; set; }
	public string? CoreAssetsDirectory { get; set; }

	public Project(string projectFilePath)
	{
		this.ProjectFilePath = projectFilePath;
		Name = Path.GetFileName(Path.ChangeExtension(projectFilePath, string.Empty)).Trim('.');
	}
}