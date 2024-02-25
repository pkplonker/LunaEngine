using Engine;
using ImGuiNET;

namespace Editor.Controls;

public class EditorCameraPanel : IPanel
{
	private readonly EditorCamera editorCamera;
	public static event Action<IPanel> RegisterPanel;

	public EditorCameraPanel(EditorCamera editorCamera)
	{
		RegisterPanel?.Invoke(this);
		this.editorCamera = editorCamera;
	}

	public string PanelName { get; set; } = "Editor Camera";

	public void Draw(Renderer renderer)
	{
		ImGui.Begin(PanelName);
		ImGui.Text($"Aspect Ratio: {editorCamera.AspectRatio}");
		var originalPos = editorCamera.Transform.Position;
		var originalRotation = editorCamera.Transform.Rotation;

		var originalAspect = editorCamera.AspectRatio;

		UndoableImGui.UndoableButton("Reset",
			new Memento(() => { editorCamera.Reset(); },
				() =>
				{
					editorCamera.Transform.Rotation = originalRotation;
					editorCamera.Transform.Position = originalPos;
					editorCamera.AspectRatio = originalAspect;
				}, "Reset Editor Camera"));
		ImGuiHelpers.DrawTransform(editorCamera.Transform);
		ImGui.End();
	}
}