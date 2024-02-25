using System.Text;
using ImGuiNET;

namespace Editor;

public static class UndoableImGui
{
	private static Dictionary<string, float> sliderStates = new();
	private static Dictionary<string, string> textStates = new();

	public static bool UndoableButton(string label, Memento memento)
	{
		bool result = false;
		if (ImGui.Button(label))
		{
			UndoManager.RecordAndPerform(memento);
			result = true;
		}

		return result;
	}

	public static void UndoableCheckbox(string label, Func<bool> getValue, Action<bool> setValue,
		string actionDescription)
	{
		bool originalValue = getValue();
		bool currentValue = originalValue;

		if (!label.StartsWith("##"))
		{
			ImGui.Text(label);
		}

		ImGui.SameLine();
		if (ImGui.Checkbox($"##{label}", ref currentValue))
		{
			setValue(currentValue);

			if (originalValue != currentValue)
			{
				UndoManager.RecordAndPerform(new Memento(
					() => setValue(currentValue),
					() => setValue(originalValue),
					actionDescription));
			}
		}
	}

	private static string DrawLabel(string label)
	{
		if (!label.StartsWith("##"))
		{
			ImGui.Text(label);
			ImGui.SameLine();
		}

		label = label.Insert(0, "##");

		return label;
	}

	public static void UndoableDragFloat(
		Func<float> getValue,
		Action<float> setValue,
		string label,
		string actionDescription,
		float min = float.MinValue,
		float max = float.MaxValue,
		float speed = 1.0f)
	{
		float value = getValue();
		label = DrawLabel(label);
		bool valueChanged = ImGui.DragFloat(label, ref value, speed, min, max);
		HandleSliderState(label, value);
		HandleValueChange(valueChanged, label, value, setValue, actionDescription);
	}

	public static void UndoableDragInt(
		Func<int> getValue,
		Action<int> setValue,
		string label,
		string actionDescription,
		int min = int.MinValue,
		int max = int.MaxValue,
		float speed = 1.0f)
	{
		int value = getValue();
		label = DrawLabel(label);
		bool valueChanged = ImGui.DragInt(label, ref value, speed, min, max);
		HandleSliderState(label, value);
		HandleValueChange(valueChanged, label, value, setValue, actionDescription);
	}

	private static void HandleValueChange<T>(bool valueChanged, string label, T value, Action<T> setValue,
		string actionDescription)
	{
		if (valueChanged)
		{
			setValue(value);
		}

		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			float originalVal = sliderStates.ContainsKey(label) ? sliderStates[label] : ConvertToFloat(value);

			if (ConvertToFloat(value) != originalVal)
			{
				UndoManager.RecordAndPerform(new Memento(
					() => setValue(value),
					() => setValue(ConvertFromFloat<T>(originalVal)),
					actionDescription));
			}
		}
	}

	private static void HandleValueChange(bool valueChanged, string label, string value, Action<string> setValue,
		string actionDescription)
	{
		if (valueChanged)
		{
			setValue(value);
		}

		Console.WriteLine(ImGui.IsItemActive());
		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			string originalVal = textStates.ContainsKey(label) ? textStates[label] : value;

			if (value != originalVal)
			{
				UndoManager.RecordAndPerform(new Memento(
					() => setValue(value),
					() => setValue(originalVal),
					actionDescription));
			}
		}
	}

	private static void HandleSliderState<T>(string label, T value)
	{
		if (ImGui.IsItemActivated())
		{
			sliderStates[label] = ConvertToFloat(value);
		}
	}
	
	private static void HandleTextState(string label, string value)
	{
		if (ImGui.IsItemActivated())
		{
			textStates[label] = value;
		}
	}

	private static float ConvertToFloat<T>(T value)
	{
		try
		{
			return (float) Convert.ChangeType(value, typeof(float));
		}
		catch (InvalidCastException)
		{
			throw new InvalidOperationException($"Cannot convert type {typeof(T).Name} to float.");
		}
	}

	private static T ConvertFromFloat<T>(float value)
	{
		try
		{
			return (T) Convert.ChangeType(value, typeof(T));
		}
		catch (InvalidCastException)
		{
			throw new InvalidOperationException($"Cannot convert float to type {typeof(T).Name}.");
		}
	}

	public static void UndoableTextBox(
		string label,
		Func<string> getValue,
		Action<string> setValue,
		string actionDescription,
		int bufferSize = 100)
	{
		string value = getValue();
		label = DrawLabel(label);
		byte[] buffer = Encoding.UTF8.GetBytes(value.PadRight(bufferSize, '\0'));

		bool valueChanged = ImGui.InputText(label, buffer, (uint) buffer.Length);
		string newValue = Encoding.UTF8.GetString(buffer).TrimEnd('\0');
		HandleTextState(label,value);
		HandleValueChange(valueChanged, label, newValue, setValue, actionDescription);

	}
}