using Editor.Properties;
using Engine;

namespace Editor.Controls;

public interface IPropertyDrawInterceptStrategy
{
	bool Draw(object component, IMemberAdapter member, Renderer renderer);
}