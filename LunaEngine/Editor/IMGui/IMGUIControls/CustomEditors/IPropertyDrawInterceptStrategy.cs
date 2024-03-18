using Editor.Properties;
using Engine;

namespace Editor.Controls;

public interface IPropertyDrawInterceptStrategy
{
	bool Draw(object component, IMemberAdapter memberInfo, IRenderer renderer);

	void DrawEmpty(int depth, string name,
		PropertyDrawer propertyDrawer, IMemberAdapter? memberInfo, object component, Action dragDrop = null)
	{
		propertyDrawer.CreateNestedHeader(depth, name, () => DrawEmptyContent(memberInfo, component),dragDrop);
	}

	void DrawEmptyContent(IMemberAdapter? memberInfo,object component);
}