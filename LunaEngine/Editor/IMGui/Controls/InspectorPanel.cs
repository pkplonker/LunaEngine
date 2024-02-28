using ImGuiNET;
using Renderer = Engine.Renderer;

namespace Editor.Controls;

public class InspectorPanel : IPanel
{
	private readonly EditorImGuiController controller;
	private PropertyDrawer propertyDrawer;
	public static event Action<IPanel> RegisterPanel;

	public InspectorPanel(EditorImGuiController controller)
	{
		RegisterPanel?.Invoke(this);
		this.controller = controller;
	}

	public string PanelName { get; set; } = "Inspector";

	public void Draw(Renderer renderer)
	{
		if (propertyDrawer == null)
		{
			propertyDrawer = new PropertyDrawer(renderer);
		}

		ImGui.Begin(PanelName);
		if (controller.SelectedGameObject != null)
		{
			var go = controller.SelectedGameObject;
			UndoableImGui.UndoableCheckbox("##Enabled", () => go.Enabled, val => go.Enabled = val,
				"GameObject Enabled Toggled");
			ImGui.SameLine();
			ImGui.Text($"{go.Name}: {go.Guid.ToString()}");
			ImGuiHelpers.DrawTransform(go.Transform);

			foreach (var component in go.GetComponents())
			{
				try
				{
					propertyDrawer.DrawObject(component, 0);
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