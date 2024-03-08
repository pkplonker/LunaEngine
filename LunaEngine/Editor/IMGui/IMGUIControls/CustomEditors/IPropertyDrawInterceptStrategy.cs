using Editor.Properties;
using Engine;

namespace Editor.Controls;

public interface IPropertyDrawInterceptStrategy
{
	bool Draw(object component, IMemberAdapter memberInfo, Renderer renderer);

	void DrawEmpty(int depth, string name,
		PropertyDrawer propertyDrawer, IMemberAdapter? memberInfo)
	{
		propertyDrawer.CreateNestedHeader(depth, name, () => DrawEmptyContent(memberInfo));
	}

	void DrawEmptyContent(IMemberAdapter? memberInfo);
}