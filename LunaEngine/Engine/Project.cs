namespace Editor;

public class Project
{
	public string Path { get; set; }

	public string Directory
	{
		get
		{
			if (!string.IsNullOrEmpty(Path))
			{
				return System.IO.Path.GetDirectoryName(Path);
			}

			return string.Empty;
		}
	}

	public Project(string path)
	{
		this.Path = path;
	}
}