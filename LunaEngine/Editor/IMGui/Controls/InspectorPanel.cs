using System.Numerics;
using System.Reflection;
using System.Text;
using Editor.Properties;
using Engine;
using ImGuiNET;
using Silk.NET.SDL;
using Renderer = Engine.Renderer;
using SerializableAttribute = Engine.SerializableAttribute;
using Texture = Engine.Texture;

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
			ProcessProps(component, type);
		}
	}

	private void ProcessProps(object component, Type type)
	{
		var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
		foreach (var memberInfo in type
			         .GetProperties(flags)
			         .Cast<MemberInfo>()
			         .Concat(type.GetFields(flags))
			         .Select<MemberInfo, IMemberAdapter>(memberInfo =>
			         {
				         return memberInfo switch
				         {
					         PropertyInfo prop => new PropertyAdapter(prop),
					         FieldInfo field => new FieldAdapter(field),
				         };
			         }))
		{
			ProcessMember(component, memberInfo);
		}
	}

	private void ProcessMember(object component, IMemberAdapter memberInfo)
	{
		var attribute = memberInfo.GetCustomAttribute<SerializableAttribute>();

		if (memberInfo.GetMemberInfo() is FieldInfo && (attribute == null || !attribute.Show))
		{
			return;
		}

		if (memberInfo.GetMemberInfo() is PropertyInfo propertyInfo)
		{
			if (propertyInfo.GetGetMethod(true)?.IsPublic != true || (attribute?.Show ?? true) == false)
			{
				return;
			}
		}

		object propertyValue = memberInfo.GetValue(component);
		Type? memberType = memberInfo?.MemberType ?? null;
		var typeAttribute = memberType?.GetCustomAttribute<SerializableAttribute>();

		if (propertyValue != null && typeAttribute != null)
		{
			if (!typeAttribute.Show) return;
			ProcessClassAttribute(propertyValue, memberType, memberInfo.Name);
			return;
		}

		try
		{
			DrawProperty(component, memberInfo, attribute, propertyValue);
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
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

	private void DrawProperty(object component, IMemberAdapter member, SerializableAttribute? attribute,
		object? propertyValue)
	{
		if (member == null) return;
		var customEditorAttr = member.GetCustomAttribute<CustomEditorAttribute>();
		var name = GetFormattedName(attribute?.CustomName ?? member.Name);

		var value = member.GetValue(component);

		if (value == null)
		{
			ImGui.Text(name);
			ImGui.SameLine();
			if (ImGui.Button($"Add##{member?.GetHashCode()}")) { }

			return;
		}

		if (customEditorAttr != null)
		{
			if (customEditorAttr.ShowName)
			{
				ImGui.Text(name);
			}

			switch (customEditorAttr.EditorType)
			{
				case Type t when t == typeof(TextureImageCustomEditor):

					int size = 200;

					var editorInstance = (ICustomEditor) Activator.CreateInstance(
						customEditorAttr.EditorType,
						new object[] {value, size, size});

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
			Func<int> getter = () => (int) member.GetValue(component);
			Action<int> setter = (newValue) => member.SetValue(component, newValue);
			UndoableImGui.UndoableDragInt(getter, setter, name, actionDescription);
		}
		else if (propertyValue is float floatValue)
		{
			Func<float> getter = () => (float) member.GetValue(component);
			Action<float> setter = (newValue) => member.SetValue(component, newValue);
			UndoableImGui.UndoableDragFloat(getter, setter, name, actionDescription);
		}
		else if (propertyValue is bool boolValue)
		{
			Func<bool> getter = () => (bool) member.GetValue(component);
			Action<bool> setter = (newValue) => member.SetValue(component, newValue);
			UndoableImGui.UndoableCheckbox(name, getter, setter, actionDescription);
		}
		else if (propertyValue is string stringValue)
		{
			Func<string> getter = () => (string) member.GetValue(component);
			Action<string> setter = (newValue) => member.SetValue(component, newValue);
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

		var upperName = GetFormattedName(memberName);
		var shownname = upperName == memberType.Name ? upperName : $"{memberType.Name} - {upperName}";
		if (ImGui.CollapsingHeader(shownname, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Framed))
		{
			ProcessProps(propertyValue, memberType);
		}

		ImGui.PopStyleColor();

		ImGui.Unindent();
	}
}