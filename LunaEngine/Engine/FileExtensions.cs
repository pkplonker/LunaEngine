namespace Engine;

public static class FileExtensions
{
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