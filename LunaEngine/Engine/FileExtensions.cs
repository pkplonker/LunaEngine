using Editor;

namespace Engine;

public static class FileExtensions
{
	public static string MakeProjectAbsolute(this string relativePath)
	{
		var basePath = String.Empty;
		var currentProject = ProjectManager.ActiveProject;
		if (currentProject != null)
		{
			basePath = Path.GetDirectoryName(currentProject.Path);
		}

		if (string.IsNullOrWhiteSpace(relativePath))
		{
			return string.Empty;
		}

		relativePath = relativePath.Trim('/');

		string? absolutePath = null;
		if (!Path.IsPathFullyQualified(relativePath) && relativePath != null && basePath != null)
		{
			absolutePath = Path.Combine(basePath, relativePath);
		}

		return absolutePath ?? string.Empty;
	}

	public static string MakeAbsolute(this string path)
	{
		if (string.IsNullOrWhiteSpace(path))
		{
			return string.Empty;
		}

		path = path.Trim('/');

		if (!Path.IsPathFullyQualified(path))
		{
			path = Path.GetFullPath(path);
		}

		return path;
	}
}