using System.Numerics;
using Engine;
using ImGuiNET;

namespace Editor.Controls;

public static class TransformControl
{
	// public static bool DrawVector(string vectorName, ref Vector3 vector, int hashCode)
	// {
	// 	bool dirty = false;
	// 	float labelWidth = ImGui.CalcTextSize("Rotation").X + 20.0f;
	// 	float totalWidth = ImGui.GetContentRegionAvail().X - labelWidth;
	// 	float fieldWidth = totalWidth / 3.0f - ImGui.GetStyle().ItemInnerSpacing.X;
	//
	// 	ImGui.Text(vectorName);
	// 	ImGui.SameLine(labelWidth);
	//
	// 	ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(1, 0, 0, 1));
	// 	ImGui.Text("X");
	// 	ImGui.SameLine();
	// 	ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("X").X);
	// 	Vector3 localVector = vector;
	//
	// 	ImGuiHelpers.UndoableDrag(
	// 		() => localVector.X,
	// 		value =>
	// 		{
	// 			localVector.X = value;
	// 			dirty = true;
	// 		},
	// 		$"##{vectorName}x{hashCode}",
	// 		"Change X Value");
	// 	ImGui.PopStyleColor();
	// 	ImGui.SameLine();
	//
	// 	ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 1, 0, 1));
	// 	ImGui.Text("Y");
	// 	ImGui.SameLine();
	// 	ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Y").X);
	// 	ImGuiHelpers.UndoableDrag(
	// 		() => localVector.Y,
	// 		value =>
	// 		{
	// 			localVector.Y = value;
	// 			dirty = true;
	// 		},
	// 		$"##{vectorName}Y{hashCode}",
	// 		"Change Y Value");
	//
	// 	ImGui.PopStyleColor();
	// 	ImGui.SameLine();
	//
	// 	ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 1, 1));
	// 	ImGui.Text("Z");
	// 	ImGui.SameLine();
	// 	ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Z").X);
	// 	ImGuiHelpers.UndoableDrag(
	// 		() => localVector.Z,
	// 		value =>
	// 		{
	// 			localVector.Z = value;
	// 			dirty = true;
	// 		},
	// 		$"##{vectorName}z{hashCode}",
	// 		"Change Z Value");
	// 	ImGui.PopStyleColor();
	//
	// 	ImGui.PopStyleColor();
	//
	// 	if (dirty)
	// 	{
	// 		vector = localVector;
	// 	}
	//
	// 	return dirty;
	// }
}