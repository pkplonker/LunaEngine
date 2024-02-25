using System.Numerics;
using Editor.Controls;
using Engine;
using ImGuiNET;

namespace Editor;

public static class ImGuiHelpers
{
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
		UndoableImGui.UndoableDragFloat(
			() => trans.Position.X,
			value => trans.Position = new Vector3(value, trans.Position.Y, trans.Position.Z),
			$"##PosX{trans.GetHashCode()}",
			"Change Position X", stretch: false);
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 1, 0, 1));
		ImGui.Text("Y");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Y").X);
		UndoableImGui.UndoableDragFloat(
			() => trans.Position.Y,
			value => trans.Position = new Vector3(trans.Position.X, value, trans.Position.Z),
			$"##PosY{trans.GetHashCode()}",
			"Change Position Y", stretch: false);
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 1, 1));
		ImGui.Text("Z");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Z").X);
		UndoableImGui.UndoableDragFloat(
			() => trans.Position.Z,
			value => trans.Position = new Vector3(trans.Position.X, value, trans.Position.Z),
			$"##PosZ{trans.GetHashCode()}",
			"Change Position Z", stretch: false);
		ImGui.PopStyleColor();

		ImGui.Text("Rotation");
		ImGui.SameLine(labelWidth);
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(1, 0, 0, 1));
		ImGui.Text("X");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("X").X);
		UndoableImGui.UndoableDragFloat(
			() => trans.Rotation.ToEulerDegrees().X,
			value => trans.Rotation = new Vector3(value, trans.Rotation.Y, trans.Rotation.Z).ToQuaternionFromDegrees(),
			$"##RotX{trans.GetHashCode()}",
			"Change Rotation X", stretch: false);
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 1, 0, 1));
		ImGui.Text("Y");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Y").X);
		UndoableImGui.UndoableDragFloat(
			() => trans.Rotation.ToEulerDegrees().Y,
			value => trans.Rotation = new Vector3(trans.Rotation.X, value, trans.Rotation.Z).ToQuaternionFromDegrees(),
			$"##RotY{trans.GetHashCode()}",
			"Change Rotation Y", stretch: false);
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 1, 1));
		ImGui.Text("Z");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Z").X);
		UndoableImGui.UndoableDragFloat(
			() => trans.Rotation.ToEulerDegrees().Z,
			value => trans.Rotation = new Vector3(trans.Rotation.X, trans.Rotation.Y, value).ToQuaternionFromDegrees(),
			$"##RotZ{trans.GetHashCode()}",
			"Change Rotation Z", stretch: false);
		ImGui.PopStyleColor();

		ImGui.Text("Scale");
		ImGui.SameLine(labelWidth);
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(1, 0, 0, 1));
		ImGui.Text("X");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("X").X);
		UndoableImGui.UndoableDragFloat(
			() => trans.Scale.X,
			value => trans.Scale = new Vector3(value, trans.Scale.Y, trans.Scale.Z),
			$"##ScaleX{trans.GetHashCode()}",
			"Change Scale X", stretch: false);
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 1, 0, 1));
		ImGui.Text("Y");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Y").X);
		UndoableImGui.UndoableDragFloat(
			() => trans.Scale.Y,
			value => trans.Scale = new Vector3(trans.Scale.X, value, trans.Scale.Z),
			$"##ScaleY{trans.GetHashCode()}",
			"Change Scale Y", stretch: false);
		ImGui.PopStyleColor();
		ImGui.SameLine();
		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 1, 1));
		ImGui.Text("Z");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Z").X);
		UndoableImGui.UndoableDragFloat(
			() => trans.Scale.Z,
			value => trans.Scale = new Vector3(trans.Scale.X, trans.Scale.Y, value),
			$"##ScaleZ{trans.GetHashCode()}",
			"Change Scale Z", stretch: false);
		ImGui.PopStyleColor();
	}
}