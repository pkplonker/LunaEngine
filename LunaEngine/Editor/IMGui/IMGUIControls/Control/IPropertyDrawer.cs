using Engine;

namespace Editor.Controls;

public interface IPropertyDrawer
{
	void DrawObject(object component, string? name = null,int depth = 0);
	void ProcessProps(object component, int depth=0);

	void CreateNestedHeader(int depth,
		string? name, Action content, Action handleDragDrop = null);
}