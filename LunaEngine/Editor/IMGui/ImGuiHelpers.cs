using System.Numerics;
using Editor.Controls;
using Engine;
using ImGuiNET;

namespace Editor;

public static class ImGuiHelpers
{
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

	public static void DrawTransform(Transform trans)
	{
		ImGui.Separator();
		float labelWidth = ImGui.CalcTextSize("Rotation").X + 20.0f;
		float totalWidth = ImGui.GetContentRegionAvail().X - labelWidth;
		float fieldWidth = totalWidth / 3.0f - ImGui.GetStyle().ItemInnerSpacing.X;

		ImGui.Text("Position");
		ImGui.SameLine(labelWidth);
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(1, 0, 0, 1));
		ImGui.Text("X");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("X").X);
		UndoableDrag(
			() => trans.Position.X,
			value => trans.Position = new Vector3(value, trans.Position.Y, trans.Position.Z),
			$"##PosX{trans.GetHashCode()}",
			"Change Position X");
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 1, 0, 1));
		ImGui.Text("Y");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Y").X);
		UndoableDrag(
			() => trans.Position.Y,
			value => trans.Position = new Vector3(trans.Position.X, value, trans.Position.Z),
			$"##PosY{trans.GetHashCode()}",
			"Change Position Y");
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 1, 1));
		ImGui.Text("Z");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Z").X);
		UndoableDrag(
			() => trans.Position.Z,
			value => trans.Position = new Vector3(trans.Position.X, value, trans.Position.Z),
			$"##PosZ{trans.GetHashCode()}",
			"Change Position Z");
		ImGui.PopStyleColor();

		ImGui.Text("Rotation");
		ImGui.SameLine(labelWidth);
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(1, 0, 0, 1));
		ImGui.Text("X");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("X").X);
		UndoableDrag(
			() => trans.Rotation.ToEulerDegrees().X,
			value => trans.Rotation = new Vector3(value, trans.Rotation.Y, trans.Rotation.Z).ToQuaternionFromDegrees(),
			$"##RotX{trans.GetHashCode()}",
			"Change Rotation X");
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 1, 0, 1));
		ImGui.Text("Y");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Y").X);
		UndoableDrag(
			() => trans.Rotation.ToEulerDegrees().Y,
			value => trans.Rotation = new Vector3(trans.Rotation.X, value, trans.Rotation.Z).ToQuaternionFromDegrees(),
			$"##RotY{trans.GetHashCode()}",
			"Change Rotation Y");
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 1, 1));
		ImGui.Text("Z");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Z").X);
		UndoableDrag(
			() => trans.Rotation.ToEulerDegrees().Z,
			value => trans.Rotation = new Vector3(trans.Rotation.X, trans.Rotation.Y, value).ToQuaternionFromDegrees(),
			$"##RotZ{trans.GetHashCode()}",
			"Change Rotation Z");
		ImGui.PopStyleColor();

		ImGui.Text("Scale");
		ImGui.SameLine(labelWidth);
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(1, 0, 0, 1));
		ImGui.Text("X");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("X").X);
		UndoableDrag(
			() => trans.Scale.X,
			value => trans.Scale = new Vector3(value, trans.Scale.Y, trans.Scale.Z),
			$"##ScaleX{trans.GetHashCode()}",
			"Change Scale X");
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 1, 0, 1));
		ImGui.Text("Y");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Y").X);
		UndoableDrag(
			() => trans.Scale.Y,
			value => trans.Scale = new Vector3(trans.Scale.X, value, trans.Scale.Z),
			$"##ScaleY{trans.GetHashCode()}",
			"Change Scale Y");
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 1, 1));
		ImGui.Text("Z");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Z").X);
		UndoableDrag(
			() => trans.Scale.Z,
			value => trans.Scale = new Vector3(trans.Scale.X, trans.Scale.Y, value),
			$"##ScaleZ{trans.GetHashCode()}",
			"Change Scale Z");
		ImGui.PopStyleColor();
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

	private static Dictionary<string, float> sliderStates = new Dictionary<string, float>();

	public static void UndoableDrag(
		Func<float> getValue,
		Action<float> setValue,
		string label,
		string actionDescription,
		float min = float.MinValue,
		float max = float.MaxValue,
		float speed = 1.0f)
	{
		float value = getValue();

		bool valueChanged = ImGui.DragFloat(label, ref value, speed, min, max);
		if (ImGui.IsItemActivated())
		{
			sliderStates[label] = value;
		}

		if (valueChanged)
		{
			setValue(value);
		}

		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			float originalVal = sliderStates.ContainsKey(label) ? sliderStates[label] : value;

			if (value != originalVal)
			{
				UndoManager.RecordAndPerform(new Memento(
					() => setValue(value),
					() => setValue(originalVal),
					actionDescription));
			}
		}
	}
}