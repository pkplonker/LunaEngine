namespace Editor.Controls;

public interface IPropertyDrawer
{
	void DrawObject(object component, int depth, string? name = null);
}