using Engine;

namespace Editor.Controls;

public interface IControl
{
	string PanelName { get; protected set; }

	public void Draw(Renderer renderer);
	
}