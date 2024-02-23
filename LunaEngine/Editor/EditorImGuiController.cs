using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace Editor;

public class EditorImGuiController : IDisposable
{
	private ImGuiController imGuiController;

	public EditorImGuiController(GL gl, IView view, IInputContext input)
	{
		imGuiController = new ImGuiController(gl, view, input);
		var io = ImGui.GetIO();
		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;

	}

	public void Update(float deltaTime)
	{
		imGuiController.Update(deltaTime);
		ImGui.DockSpaceOverViewport();
		ImGui.Begin("Test", ImGuiWindowFlags.DockNodeHost);
		ImGui.End();
	}

	public void Render()
	{
		ImGui.ShowDemoWindow();
		imGuiController.Render();
	}

	public void Dispose()
	{
		imGuiController.Dispose();
	}
}