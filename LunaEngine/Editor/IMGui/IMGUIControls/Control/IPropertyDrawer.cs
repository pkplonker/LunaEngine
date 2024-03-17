namespace Editor.Controls;

public interface IPropertyDrawer
{
	void DrawObject(object component, int depth, IPropertyDrawInterceptStrategy? interceptStrategy, string? name = null,
		Action handleDragDrop = null);
}