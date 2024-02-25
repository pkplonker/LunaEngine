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
		var transformPosition = trans.Position;

		if (TransformControl.DrawVector("Position", ref transformPosition, trans.GetHashCode()))
		{
			trans.Position = transformPosition;
		}

		var transformRotation = trans.Rotation.ToEulerDegrees();
		if (TransformControl.DrawVector("Rotation", ref transformRotation, trans.GetHashCode()))
		{
			trans.Rotation = transformRotation.ToQuaternionFromDegrees();
		}

		var transformScale = trans.Scale;

		if (TransformControl.DrawVector("Scale", ref transformScale, trans.GetHashCode()))
		{
			trans.Scale = transformScale;
		}
	}
}