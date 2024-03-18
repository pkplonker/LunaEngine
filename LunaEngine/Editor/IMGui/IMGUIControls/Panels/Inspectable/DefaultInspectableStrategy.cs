using Engine;

namespace Editor.Controls;

public class DefaultInspectableStrategy : IInspectableStrategy
{
	public void Draw(IInspectable obj, IPropertyDrawer? propertyDrawer)
	{
		propertyDrawer?.DrawObject(obj, 0, null);
	}
}