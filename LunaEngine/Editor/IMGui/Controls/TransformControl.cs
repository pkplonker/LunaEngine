using System.Numerics;
using Engine;
using ImGuiNET;

namespace Editor.Controls;

public static class TransformControl
{
	public static bool DrawVector(string vectorName, ref Vector3 vector, int hashCode)
	{
		bool dirty = false;
		float labelWidth = ImGui.CalcTextSize("Rotation").X + 20.0f;
		float totalWidth = ImGui.GetContentRegionAvail().X - labelWidth;
		float fieldWidth = totalWidth / 3.0f - ImGui.GetStyle().ItemInnerSpacing.X;

		ImGui.Text(vectorName);
		ImGui.SameLine(labelWidth);

		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(1, 0, 0, 1));
		ImGui.Text("X");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("X").X);
		//drag x
		if (ImGui.DragFloat($"##{vectorName}x{hashCode}", ref vector.X))
		{
			dirty = true;
		}
		ImGui.PopStyleColor();
		ImGui.SameLine();

		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 1, 0, 1));
		ImGui.Text("Y");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Y").X);
		//drag y
		if (ImGui.DragFloat($"##{vectorName}y{hashCode}", ref vector.Y))
		{
			dirty = true;
		}

		ImGui.PopStyleColor();
		ImGui.SameLine();

		ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 1, 1));
		ImGui.Text("Z");
		ImGui.SameLine();
		ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Z").X);
		//drag z
		if (ImGui.DragFloat($"##{vectorName}z{hashCode}", ref vector.Z))
		{
			dirty = true;
		}

		ImGui.PopStyleColor();
		return dirty;
	}
}