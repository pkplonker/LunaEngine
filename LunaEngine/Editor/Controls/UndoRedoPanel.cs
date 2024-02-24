using Engine;
using ImGuiNET;
using System;
using System.Linq;

namespace Editor.Controls;

public class UndoRedoPanel : IPanel
{
	public string PanelName { get; set; } = "UndoRedo";

	public static event Action<IPanel> RegisterPanel;

	public UndoRedoPanel()
	{
		RegisterPanel?.Invoke(this);
	}

	public void Draw(Renderer renderer)
	{
		ImGui.Begin(PanelName);

		if (ImGui.Button("Undo"))
		{
			UndoManager.Undo();
		}

		if (ImGui.Button("Redo"))
		{
			UndoManager.Redo();
		}

		ImGui.Separator();

		ImGui.Text("Undo Stack:");
		if (ImGui.BeginChild("UndoStack", new System.Numerics.Vector2(0, 150)))
		{
			foreach (var action in UndoManager.GetUndoActions().Reverse())
			{
				ImGui.Text(action); 
			}
			ImGui.EndChild();
		}

		ImGui.Separator();

		// Scrollable box for Redo Stack
		ImGui.Text("Redo Stack:");
		if (ImGui.BeginChild("RedoStack", new System.Numerics.Vector2(0, 150)))
		{
			foreach (var action in UndoManager.GetRedoActions().Reverse())
			{
				ImGui.Text(action); 
			}
			ImGui.EndChild();
		}

		ImGui.End();
	}
}