using Engine;
using ImGuiNET;

namespace Editor.Controls;

public class InspectorPanel : IPanel
{
	private static readonly IInspectableStrategy DEFAULT_STRATEGY = new DefaultInspectableStrategy();

	private Dictionary<Type, IInspectableStrategy> drawStrategies = new()
	{
		{typeof(GameObject), new GameObjectInspectableStrategy()},
		{typeof(Material), DEFAULT_STRATEGY},
		{typeof(Shader), DEFAULT_STRATEGY},
		{typeof(Mesh), DEFAULT_STRATEGY}
	};

	private IPropertyDrawer? propertyDrawer;
	private IInspectable? selected;
	private bool newChange = false;
	private IInspectableStrategy drawStrategy;
	public event Action<IInspectable?> SelectionChanged;

	public InspectorPanel(ISelectableObjectController controller)
	{
		drawStrategy = DEFAULT_STRATEGY;
		controller.GameObjectSelectionChanged += ControllerOnGameObjectSelectionChanged;
		SceneController.OnActiveSceneChanged += (scene, scene1) => { selected = null; };
	}

	private void ControllerOnGameObjectSelectionChanged(IInspectable? obj)
	{
		selected = obj;
		SelectionChanged?.Invoke(obj);
		newChange = true;
		if (obj != null)
		{
			if (drawStrategies.TryGetValue(selected.GetType(), out var strategy))
			{
				drawStrategy = strategy;
			}
		}
	}

	public string PanelName { get; set; } = "Inspector";

	public void Draw(IRenderer renderer)
	{
		ImGui.Begin(PanelName);
		if (newChange)
		{
			newChange = false;
			ImGui.SetScrollY(0);
		}

		propertyDrawer ??= new PropertyDrawer(renderer);

		if (selected == null) return;
		drawStrategy.Draw(selected, propertyDrawer);

		ImGui.End();
	}
}