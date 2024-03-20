using Engine;
using ImGuiNET;

namespace Editor.Controls;

public class InspectorPanel : IPanel
{
	private IPropertyDrawer? propertyDrawer;
	private IInspectable? selected;
	private bool newChange = false;
	public event Action<IInspectable?> SelectionChanged;

	public InspectorPanel(ISelectableObjectController controller)
	{
		controller.GameObjectSelectionChanged += ControllerOnGameObjectSelectionChanged;
		SceneController.OnActiveSceneChanged += (scene, scene1) => { selected = null; };
	}

	private void ControllerOnGameObjectSelectionChanged(IInspectable? obj)
	{
		selected = obj;
		SelectionChanged?.Invoke(obj);
		newChange = true;
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
		if (CustomEditorLoader.TryGetEditor(selected.GetType(), out var editor))
		{
			editor.Draw(selected, null, null, renderer);
		}
		else
		{
			propertyDrawer.DrawObject(selected);
		}

		ImGui.End();
	}
}