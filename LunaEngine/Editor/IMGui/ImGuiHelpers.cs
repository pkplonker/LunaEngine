using System.Numerics;
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
}