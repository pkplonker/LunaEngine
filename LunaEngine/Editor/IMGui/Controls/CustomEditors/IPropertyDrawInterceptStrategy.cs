using Editor.Properties;

namespace Editor.Controls;

public interface IInterceptStrategy
{
	bool Draw(IMemberAdapter member);
}