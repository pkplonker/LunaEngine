using System.Numerics;
using System.Reflection;
using Engine;
using ImGuiNET;
using SerializableAttribute = Engine.SerializableAttribute;

namespace Editor.Controls;

public class InspectorPanel : IPanel
{
	private readonly EditorImGuiController controller;
	public static event Action<IPanel> RegisterPanel;

	public InspectorPanel(EditorImGuiController controller)
	{
		RegisterPanel?.Invoke(this);
		this.controller = controller;
	}

	public string PanelName { get; set; } = "Inspector";

	public void Draw(Renderer renderer)
	{
		ImGui.Begin(PanelName);
		if (controller.SelectedGameObject != null)
		{
			var go = controller.SelectedGameObject;
			ImGuiHelpers.UndoableCheckbox("##Enabled", () => go.Enabled, val => go.Enabled = val,
				"GameObject Enabled Toggled");
			ImGui.SameLine();
			ImGui.Text($"{go.Name}: {go.Guid.ToString()}");
			ImGuiHelpers.DrawTransform(go.Transform);

			foreach (var component in go.GetComponents())
			{
				try
				{
					DrawComponent(component);
				}
				catch (Exception e)
				{
					Console.WriteLine(e);
				}
			}

			ImGui.End();
		}
	}

	private void DrawComponent(IComponent component)
	{
		var type = component.GetType();
		if (ImGui.CollapsingHeader(type.Name, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Framed))
		{
			foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public |
			                                                     BindingFlags.NonPublic))
			{
				ProcessMember(component, property);
			}

			foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public |
			                                           BindingFlags.NonPublic))
			{
				ProcessMember(component, field);
			}
		}
	}

	private static void ProcessMember(IComponent component, MemberInfo member)
	{
		var attribute = member.GetCustomAttribute<SerializableAttribute>();
		if (member is FieldInfo && (attribute == null ||
		                            attribute.Show == false))
		{
			return;
		}

		object propertyValue = null;
		if (member is PropertyInfo propertyInfo)
		{
			if (propertyInfo.GetGetMethod(true)?.IsPublic == true)
			{
				if ((attribute?.Show ?? true) == false)
				{
					return;
				}
			}

			var typeAttribute = propertyInfo.PropertyType.GetCustomAttribute<SerializableAttribute>();
			if (typeAttribute != null && typeAttribute.Show == false)
			{
				return;
			}

			propertyValue = propertyInfo.GetValue(component);
		}
		else if (member is FieldInfo fieldInfo)
		{
			var typeAttribute = fieldInfo.FieldType.GetCustomAttribute<SerializableAttribute>();
			if (typeAttribute != null && typeAttribute.Show == false)
			{
				return;
			}

			propertyValue = fieldInfo.GetValue(component);
		}

		ImGui.Text($"{attribute?.CustomName ?? member.Name} : {propertyValue}");
	}
}