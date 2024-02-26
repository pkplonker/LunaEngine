using System.Numerics;
using System.Reflection;
using System.Text;
using Engine;
using ImGuiNET;
using Silk.NET.SDL;
using Renderer = Engine.Renderer;
using SerializableAttribute = Engine.SerializableAttribute;

namespace Editor.Controls;

public class InspectorPanel : IPanel
{
	private readonly EditorImGuiController controller;
	private Renderer renderer;
	public static event Action<IPanel> RegisterPanel;

	public InspectorPanel(EditorImGuiController controller)
	{
		RegisterPanel?.Invoke(this);
		this.controller = controller;
	}

	public string PanelName { get; set; } = "Inspector";

	public void Draw(Renderer renderer)
	{
		this.renderer = renderer;
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

	private void ProcessMember(object component, MemberInfo member)
	{
		var attribute = member.GetCustomAttribute<SerializableAttribute>();

		if (member is FieldInfo && (attribute == null || !attribute.Show))
		{
			return;
		}

		object propertyValue = null;
		if (member is PropertyInfo propertyInfo)
		{
			if (propertyInfo.GetGetMethod(true)?.IsPublic != true || (attribute?.Show ?? true) == false)
			{
				return;
			}

			propertyValue = propertyInfo.GetValue(component);
		}
		else if (member is FieldInfo fieldInfo)
		{
			propertyValue = fieldInfo.GetValue(component);
		}

		//if (propertyValue == null) return;

		Type memberType = null;
		if (member is PropertyInfo propInfo)
		{
			memberType = propInfo.PropertyType;
		}
		else if (member is FieldInfo fieldInfo)
		{
			memberType = fieldInfo.FieldType;
		}

		var typeAttribute = memberType?.GetCustomAttribute<SerializableAttribute>();
		if (propertyValue != null && typeAttribute != null)
		{
			if (typeAttribute.Show)
			{
				ProcessClassAttribute(propertyValue, memberType, member.Name);
				return;
			}

			return;
		}

		try
		{
			DrawProperty(component, member, attribute, propertyValue);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
		}
	}

	private static object GetValue(object component, MemberInfo member)
	{
		switch (member)
		{
			case PropertyInfo property:
				return property.GetValue(component);
			case FieldInfo field:
				return field.GetValue(component);
			default:
				throw new InvalidOperationException("Unsupported member type.");
		}
	}

	private static void SetValue(object component, MemberInfo member, object value)
	{
		switch (member)
		{
			case PropertyInfo property:
				property.SetValue(component, value);
				break;
			case FieldInfo field:
				field.SetValue(component, value);
				break;
			default:
				throw new InvalidOperationException("Unsupported member type.");
		}
	}

	private Dictionary<string, string> nameCache = new Dictionary<string, string>();

	private string GetFormattedName(string camelCaseName)
	{
		if (string.IsNullOrEmpty(camelCaseName))
		{
			return camelCaseName;
		}

		// Check if the name is already processed and stored in the cache
		if (nameCache.TryGetValue(camelCaseName, out string formattedName))
		{
			return formattedName;
		}

		// Process the camelCase name to create a formatted name
		formattedName = ConvertCamelCaseToProperCase(camelCaseName);

		// Store the formatted name in the cache for future use
		nameCache[camelCaseName] = formattedName;

		return formattedName;
	}

	private static string ConvertCamelCaseToProperCase(string name)
	{
		if (string.IsNullOrEmpty(name))
			return name;
		if (name == name.ToUpper()) return name;
		var result = new StringBuilder();
		result.Append(char.ToUpper(name[0]));

		for (var i = 1; i < name.Length; i++)
		{
			if (char.IsUpper(name[i]))
			{
				result.Append(' ');
			}

			result.Append(name[i]);
		}

		return result.ToString();
	}

	private void DrawProperty(object component, MemberInfo member, SerializableAttribute? attribute,
		object? propertyValue)
	{
		var customEditorAttr = member.GetCustomAttribute<CustomEditorAttribute>();
		var name = ConvertCamelCaseToProperCase(attribute?.CustomName ?? member.Name);
		if (customEditorAttr != null)
		{
			if (customEditorAttr.ShowName)
			{
				ImGui.Text(name);
			}

			switch (customEditorAttr.EditorType)
			{
				case Type t when t == typeof(TextureImageCustomEditor):
					uint textureId;

					if (member is PropertyInfo propertyInfo)
					{
						textureId = (uint) propertyInfo.GetValue(component);
					}
					else if (member is FieldInfo fieldInfo)
					{
						textureId = (uint) fieldInfo.GetValue(component);
					}
					else
					{
						throw new InvalidCastException($"Failed to cast {typeof(MemberInfo)}");
					}

					int size = 200;

					var editorInstance = (iCustomEditor) Activator.CreateInstance(
						customEditorAttr.EditorType,
						new object[] {textureId, size, size});

					editorInstance.Draw(renderer);
					break;
				default:
					throw new InvalidOperationException("Unsupported editor type");
			}

			return;
		}

		var actionDescription = $"Modifying {name}";
		if (propertyValue is int intValue)
		{
			Func<int> getter = () => (int) GetValue(component, member);
			Action<int> setter = (newValue) => SetValue(component, member, newValue);
			UndoableImGui.UndoableDragInt(getter, setter, name, actionDescription);
		}
		else if (propertyValue is float floatValue)
		{
			Func<float> getter = () => (float) GetValue(component, member);
			Action<float> setter = (newValue) => SetValue(component, member, newValue);
			UndoableImGui.UndoableDragFloat(getter, setter, name, actionDescription);
		}
		else if (propertyValue is bool boolValue)
		{
			Func<bool> getter = () => (bool) GetValue(component, member);
			Action<bool> setter = (newValue) => SetValue(component, member, newValue);
			UndoableImGui.UndoableCheckbox(name, getter, setter, actionDescription);
		}
		else if (propertyValue is string stringValue)
		{
			Func<string> getter = () => (string) GetValue(component, member);
			Action<string> setter = (newValue) => SetValue(component, member, newValue);
			UndoableImGui.UndoableTextBox(name, getter, setter, actionDescription);
		}
		else if (propertyValue is Vector2 vector2Value)
		{
			// Example: ImGui.InputFloat2($"{member.Name}", ref vector2Value);
		}
		else if (propertyValue is Vector3 vector3Value)
		{
			// Example: ImGui.InputFloat3($"{member.Name}", ref vector3Value);
		}
		else if (propertyValue is Vector4 vector4Value)
		{
			// Example: ImGui.InputFloat4($"{member.Name}", ref vector4Value);
		}
		else
		{
			ImGui.Text($"{name} : {propertyValue}");
		}
	}

	private void ProcessClassAttribute(object propertyValue, Type memberType, string memberName)
	{
		ImGui.Indent();

		ImGui.PushStyleColor(ImGuiCol.Header, new Vector4(0.4f, 0.4f, 0.4f, 1.0f));

		var upperName = ConvertCamelCaseToProperCase(memberName);
		var shownname = upperName == memberType.Name ? upperName : $"{memberType.Name} - {upperName}";
		if (ImGui.CollapsingHeader(shownname, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Framed))
		{
			foreach (PropertyInfo property in memberType.GetProperties(BindingFlags.Instance | BindingFlags.Public |
			                                                           BindingFlags.NonPublic))
			{
				ProcessMember(propertyValue, property);
			}

			foreach (FieldInfo field in memberType.GetFields(BindingFlags.Instance | BindingFlags.Public |
			                                                 BindingFlags.NonPublic))
			{
				ProcessMember(propertyValue, field);
			}
		}

		ImGui.PopStyleColor();

		ImGui.Unindent();
	}
}