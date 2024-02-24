using Engine;

namespace Editor.Controls;

public interface IPanel
{
	string PanelName { get; protected set; }

	public void Draw(Renderer renderer);

	
}