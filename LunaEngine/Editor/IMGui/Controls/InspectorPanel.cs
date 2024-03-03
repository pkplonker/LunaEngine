using Engine;
using Engine.Logging;
using ImGuiNET;
using Renderer = Engine.Renderer;

namespace Editor.Controls;

public class InspectorPanel : IPanel
{
	private readonly EditorImGuiController controller;
	private PropertyDrawer propertyDrawer;
	private GameObject? selectedGameObject;
	private float test;
	public event Action<object> SelectionChanged;

	public InspectorPanel(EditorImGuiController controller)
	{
		controller.GameObjectSelectionChanged += ControllerOnGameObjectSelectionChanged;
	}

	private void ControllerOnGameObjectSelectionChanged(GameObject? obj)
	{
		selectedGameObject = obj;
		SelectionChanged?.Invoke(obj);
	}

	public string PanelName { get; set; } = "Inspector";

	public void Draw(Renderer renderer)
	{
		ImGui.Begin(PanelName);

		if (propertyDrawer == null)
		{
			propertyDrawer = new PropertyDrawer(renderer);
		}

		if (selectedGameObject != null)
		{
			var go = selectedGameObject;
			UndoableImGui.UndoableCheckbox("##Enabled", "GameObject Enabled Toggled", () => go.Enabled,
				val => go.Enabled = val
			);
			ImGui.SameLine();
			ImGui.Text($"{go.Name}: {go.Transform.GUID.ToString()}");
			ImGuiHelpers.DrawTransform(go.Transform);

			foreach (var component in go.GetComponents())
			{
				try
				{
					propertyDrawer.DrawObject(component, 0, null);
				}
				catch (Exception e)
				{
					Logger.Warning(e.ToString());
				}
			}

			ImGui.End();
		}
	}
}