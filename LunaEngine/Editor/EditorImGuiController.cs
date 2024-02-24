using System.Numerics;
using Engine;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace Editor;

public class EditorImGuiController : IDisposable
{
	private readonly GL gl;
	private readonly Renderer renderer;
	private ImGuiController imGuiController;
	private Vector2 previousSize;

	public EditorImGuiController(GL gl, IView view, IInputContext input, Renderer renderer)
	{
		this.gl = gl;
		this.renderer = renderer;
		imGuiController = new ImGuiController(gl, view, input);
		var io = ImGui.GetIO();
		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.DockingEnable;
		ImGui.GetIO().ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
		ImGuiTheme.ApplyTheme(0);
	}

	public void Update(float deltaTime)
	{
		imGuiController.Update(deltaTime);
		ImGui.DockSpaceOverViewport();
		ImGui.Begin("Test", ImGuiWindowFlags.DockNodeHost);
		ImGui.End();
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0);
		ImGui.Begin("Viewport", ImGuiWindowFlags.NoScrollbar);
		var size = ImGui.GetContentRegionAvail();
		Console.WriteLine(size);
		if (size != previousSize)
		{
			renderer.SetRenderTargetSize(new Vector2D<float>(size.X, size.Y));
			previousSize = size;
		}

		ImGui.Image((IntPtr) renderer.renderTexture.Handle, new Vector2(size.X, size.Y), Vector2.Zero, Vector2.One,
			Vector4.One,
			Vector4.Zero);
		ImGui.End();
		ImGui.PopStyleVar();
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

	public void Resize(Vector2D<int> size)
	{
		var io = ImGui.GetIO();
		io.DisplaySize = (Vector2) size;
	}
}