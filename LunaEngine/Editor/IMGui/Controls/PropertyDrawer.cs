using System.Numerics;
using System.Reflection;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;

namespace Editor.Controls;

public class PropertyDrawer : IPropertyDrawer
{
	private Renderer renderer;

	public PropertyDrawer(Renderer renderer)
	{
		this.renderer = renderer;
	}

	public void DrawObject(object component, int depth, IPropertyDrawInterceptStrategy? interceptStrategy,
		string? name = null)
	{
		var type = component.GetType();
		if (depth > 0)
		{
			ImGui.Indent();
			Vector4 currentColor = ImGui.GetStyle().Colors[(int) ImGuiCol.Header];

			float darkenFactor = 1.2f;
			Vector4 darkerColor = new Vector4(
				currentColor.X * darkenFactor,
				currentColor.Y * darkenFactor,
				currentColor.Z * darkenFactor,
				currentColor.W);

			ImGui.PushStyleColor(ImGuiCol.Header, darkerColor);
		}

		if (ImGui.CollapsingHeader(name ?? type.Name, ImGuiTreeNodeFlags.DefaultOpen | ImGuiTreeNodeFlags.Framed))
		{
			ProcessProps(component, type, depth, interceptStrategy);
		}

		if (depth > 0)
		{
			ImGui.PopStyleColor();
			ImGui.Unindent();
		}
	}

	private void ProcessProps(object component, Type type, int depth, IPropertyDrawInterceptStrategy? interceptStrategy)
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
			ProcessMember(component, memberInfo, depth, interceptStrategy);
		}
	}

	private void ProcessMember(object component, IMemberAdapter memberInfo, int depth,
		IPropertyDrawInterceptStrategy? interceptStrategy)
	{
		if (interceptStrategy?.Draw(component, memberInfo, renderer) ?? false)
		{
			return;
		}

		var attribute = memberInfo.GetCustomAttribute<InspectableAttribute>();

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
		var typeAttribute = memberType?.GetCustomAttribute<InspectableAttribute>();

		if (propertyValue != null && typeAttribute != null)
		{
			if (!typeAttribute.Show) return;

			if (CustomEditorLoader.TryGetEditor(memberType, out var customEditor))
			{
				customEditor.Draw(memberInfo, propertyValue, renderer, depth);
			}
			else
			{
				var upperName = CamelCaseRenamer.GetFormattedName(memberInfo.Name);
				var shownName = upperName == memberType.Name ? upperName : $"{memberType.Name} - {upperName}";
				DrawObject(memberInfo.GetValue(component), ++depth, interceptStrategy, shownName);
			}

			return;
		}

		try
		{
			DrawProperty(component, memberInfo, attribute, propertyValue);
		}
		catch (Exception e)
		{
			Debug.Warning(e.ToString());
		}
	}

	private void DrawProperty(object component, IMemberAdapter member, InspectableAttribute? attribute,
		object? propertyValue)
	{
		if (member == null) return;
		if (HasCustomEditor(component, member, attribute))
		{
			return;
		}

		var name = CamelCaseRenamer.GetFormattedName(attribute?.CustomName ?? member.Name);

		var value = member.GetValue(component);

		if (value == null)
		{
			ImGuiHelpers.AddProperty(member);
			return;
		}

		var actionDescription = $"Modifying {name}";

		PerformDraw(component, member, propertyValue, name, actionDescription);
	}

	private static void PerformDraw(object component, IMemberAdapter member, object? propertyValue, string name,
		string actionDescription)
	{
		if (propertyValue is int intValue)
		{
			Func<int> getter = () => (int) member.GetValue(component);
			Action<int> setter = (newValue) => member.SetValue(component, newValue);
			UndoableImGui.UndoableDragInt(name, actionDescription, getter, setter);
		}
		else if (propertyValue is float floatValue)
		{
			Func<float> getter = () => (float) member.GetValue(component);
			Action<float> setter = (newValue) => member.SetValue(component, newValue);
			UndoableImGui.UndoableDragFloat(name, actionDescription, getter, setter);
		}
		else if (propertyValue is bool boolValue)
		{
			Func<bool> getter = () => (bool) member.GetValue(component);
			Action<bool> setter = (newValue) => member.SetValue(component, newValue);
			UndoableImGui.UndoableCheckbox(name, actionDescription, getter, setter);
		}
		else if (propertyValue is string stringValue)
		{
			Func<string> getter = () => (string) member.GetValue(component);
			Action<string> setter = (newValue) => member.SetValue(component, newValue);
			UndoableImGui.UndoableTextBox(name, actionDescription, getter, setter);
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

	private bool HasCustomEditor(object component, IMemberAdapter member, InspectableAttribute? attribute)
	{
		return false;
	}
}