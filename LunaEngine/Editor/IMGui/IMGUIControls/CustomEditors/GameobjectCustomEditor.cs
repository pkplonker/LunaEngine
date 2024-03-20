using System.Linq.Expressions;
using System.Numerics;
using Editor.IMGUIControls;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;
using Silk.NET.OpenGL;
using Shader = Engine.Shader;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.GameObject))]
public class GameobjectCustomEditor : BaseCustomEditor
{
	private static PropertyDrawer? propertyDrawer;

	public override void Draw(object component, object owningObject, IMemberAdapter memberInfoToSetObjectOnOwner,
		IRenderer renderer, int depth = 0)
	{
		if (component == owningObject)
		{
			Logger.Error("er");
		}

		propertyDrawer ??= new PropertyDrawer(renderer);

		Draw(component as GameObject);
	}

	const string ADD_COMPONENT_POPUP_NAME = "AddComponentPopup";

	private static void Draw(GameObject go)
	{
		UndoableImGui.UndoableCheckbox("##Enabled", "GameObject Enabled Toggled", () => go.Enabled,
			val => go.Enabled = val
		);
		ImGui.SameLine();
		ImGui.Text($"{go.Name}: {go.Transform.GUID.ToString()}");
		ImGui.SameLine();
		if (ImGuiHelpers.CenteredButton("Add Component"))
		{
			ImGui.OpenPopup(ADD_COMPONENT_POPUP_NAME);
		}

		ImGuiHelpers.DrawTransform(go.Transform);

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

		foreach (var comp in go.GetComponents())
		{
			propertyDrawer.DrawObject(comp);
		}
	}
}