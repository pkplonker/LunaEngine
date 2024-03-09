﻿using Engine;
using Engine.Logging;
using ImGuiNET;
using Renderer = Engine.Renderer;

namespace Editor.Controls;

public class InspectorPanel : IPanel
{
	const string ADD_COMPONENT_POPUP_NAME = "AddComponentPopup";

	private PropertyDrawer? propertyDrawer;
	private GameObject? selectedGameObject;
	private bool newChange = false;
	public event Action<GameObject?> SelectionChanged;

	public InspectorPanel(EditorImGuiController controller) =>
		controller.GameObjectSelectionChanged += ControllerOnGameObjectSelectionChanged;

	private void ControllerOnGameObjectSelectionChanged(GameObject? obj)
	{
		selectedGameObject = obj;
		SelectionChanged?.Invoke(obj);
		newChange = true;
	}

	public string PanelName { get; set; } = "Inspector";
	const float RIGHT_PADDING = 20.0f;

	public void Draw(IRenderer renderer)
	{
		ImGui.Begin(PanelName);
		if (newChange)
		{
			newChange = false;
			ImGui.SetScrollY(0);
		}
		propertyDrawer ??= new PropertyDrawer(renderer);

		if (selectedGameObject == null) return;
		var go = selectedGameObject;
		UndoableImGui.UndoableCheckbox("##Enabled", "GameObject Enabled Toggled", () => go.Enabled,
			val => go.Enabled = val
		);
		ImGui.SameLine();
		ImGui.Text($"{go.Name}: {go.Transform.GUID.ToString()}");
		if (ImGuiHelpers.CenteredButton("Add Component"))
		{
			ImGui.OpenPopup(ADD_COMPONENT_POPUP_NAME);
		}

		if (ImGui.BeginPopup(ADD_COMPONENT_POPUP_NAME))
		{
			foreach (var kvp in ComponentRegistry.Components.OrderBy(x => x.Key))

			{
				if (!ImGui.Selectable(kvp.Key)) continue;
				var cachedGameObject = selectedGameObject;

				UndoManager.RecordAndPerform(
					new Memento(() => selectedGameObject.AddComponent(kvp.Value),
					() =>
					{
						if (cachedGameObject != null)
						{
							cachedGameObject.RemoveComponent(kvp.Value);
						}
					}, $"Added component - {kvp.Key}"));
				ImGui.CloseCurrentPopup();
			}

			ImGui.EndPopup();
		}

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