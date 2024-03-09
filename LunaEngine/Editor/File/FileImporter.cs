using Engine;
using Engine.Logging;

namespace Editor;

public static class FileImporter
{
	public static void Import()
	{
		var paths = FileDialog.OpenFileDialog(
			FileDialog.BuildFileDialogFilter(FileTypeExtensions.GetAllExtensions()), true);
		int result = paths.Count(path => Import(path));

		Logger.Info($"Imported {result}/{paths.Count()} assets");
	}

	public static void ImportAllFromDirectory(string path)
	{
		var paths = ResourceManager.GetFilesFromFolder(path, FileTypeExtensions.GetAllExtensions());
		int result = paths.Count(p => Import(p));
		Logger.Info($"Imported {result}/{paths.Count()} assets");
	}

	public static bool Import(string path)
	{
		string metadataFilePath = Path.ChangeExtension(path, Metadata.MetadataFileExtension);
		string relativePath = path.MakeProjectRelative();

		if (ResourceManager.MetadataExistsWithPath(relativePath))
		{
			return false; 
		}

		if (File.Exists(metadataFilePath))
		{
			return false;
		}

		var ext = Path.GetExtension(path);
		Type type = FileTypeExtensions.GetTypeFromExtension(ext);
		var metadata = Metadata.Create(type, relativePath);

		if (!ResourceManager.AddMetaData(metadata))
		{
			return false;
		}

		metadata.Serialize();
		return true;
	}
}