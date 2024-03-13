﻿namespace Engine;

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
	public string AssetsDirectory { get; set; }
	public string CoreAssetsDirectory { get; set; }

	public Project(string projectFilePath, string name, string assetsDirectory, string coreAssetsDirectory)
	{
		ArgumentNullException.ThrowIfNull(projectFilePath);
		ArgumentNullException.ThrowIfNull(name);
		ArgumentNullException.ThrowIfNull(assetsDirectory);
		ArgumentNullException.ThrowIfNull(coreAssetsDirectory);

		this.ProjectFilePath = projectFilePath;
		this.Name = name;
		this.AssetsDirectory = assetsDirectory;
		this.CoreAssetsDirectory = coreAssetsDirectory;
	}
}