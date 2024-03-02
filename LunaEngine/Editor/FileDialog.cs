using System.Text;

namespace Editor;

public static class FileDialog
{
	public static IEnumerable<string> OpenFileDialog(string? filterString)
	{
		var dir = ProjectManager.ActiveProject?.Directory;
		OpenFileDialog openFileDialog = new OpenFileDialog();
		openFileDialog.InitialDirectory = ProjectManager.ActiveProject?.Directory ?? string.Empty;
		openFileDialog.Filter = filterString ?? "";
		openFileDialog.FilterIndex = 2;
		openFileDialog.RestoreDirectory = true;
		if (openFileDialog.ShowDialog() == DialogResult.OK)
		{
			return openFileDialog.FileNames;
		}

		return Enumerable.Empty<string>();
	}

	public static string BuildFileDialogFilter(IEnumerable<string> fileExtensions)
	{
		var filterBuilder = new StringBuilder();

		if (fileExtensions.Any())
		{
			string combinedExtensions = string.Join(";", fileExtensions.Select(ext => "*." + ext.TrimStart('.')));
			filterBuilder.AppendFormat("Supported Files ({0})|{0}", combinedExtensions);
		}
		else
		{
			filterBuilder.Append("All files (*.*)|*.*");
		}

		return filterBuilder.ToString();
	}

	public static string? FilterByType(Type? MemberType) =>
		BuildFileDialogFilter(FileTypeExtensions.GetExtensions(MemberType));
}