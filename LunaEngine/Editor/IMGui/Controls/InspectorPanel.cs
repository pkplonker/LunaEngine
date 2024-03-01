using Engine;
using ImGuiNET;
using Renderer = Engine.Renderer;

namespace Editor.Controls;

public class InspectorPanel : IPanel
{
	private readonly EditorImGuiController controller;
	private PropertyDrawer propertyDrawer;
	private GameObject? selectedGameObject;
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
		if (propertyDrawer == null)
		{
			propertyDrawer = new PropertyDrawer(renderer);
		}

		ImGui.Begin(PanelName);
		if (selectedGameObject != null)
		{
			var go = selectedGameObject;
			UndoableImGui.UndoableCheckbox("##Enabled", () => go.Enabled, val => go.Enabled = val,
				"GameObject Enabled Toggled");
			ImGui.SameLine();
			ImGui.Text($"{go.Name}: {go.Guid.ToString()}");
			ImGuiHelpers.DrawTransform(go.Transform);

			foreach (var component in go.GetComponents())
			{
				try
				{
					propertyDrawer.DrawObject(component, 0, null);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}

			ImGui.End();
		}
	}
}