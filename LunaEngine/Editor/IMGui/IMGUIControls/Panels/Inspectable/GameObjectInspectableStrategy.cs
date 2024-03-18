using Engine;
using Engine.Logging;
using ImGuiNET;

namespace Editor.Controls;

public class GameObjectInspectableStrategy : IInspectableStrategy
{
	const string ADD_COMPONENT_POPUP_NAME = "AddComponentPopup";

	public void Draw(IInspectable obj, IPropertyDrawer? propertyDrawer)
	{
		if (obj is not GameObject go) return;
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
				var cachedGameObject = go;

				UndoManager.RecordAndPerform(
					new Memento(() => go.AddComponent(kvp.Value),
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
	}
}