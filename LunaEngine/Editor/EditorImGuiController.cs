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
		io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
		io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
		ImGuiTheme.ApplyTheme(0);
		SetSize();
	}

	public void ImGuiControllerUpdate(float deltaTime)
	{
		using var tracker = new PerformanceTracker(nameof(ImGuiControllerUpdate));

		imGuiController.Update(deltaTime);
		ImGui.DockSpaceOverViewport();
		ImGui.Begin("Test", ImGuiWindowFlags.DockNodeHost);
		ImGui.End();
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0);
		ImGui.Begin("Viewport", ImGuiWindowFlags.NoScrollbar);
		var size = ImGui.GetContentRegionAvail();
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
		ImGui.ShowDemoWindow();
	}

	public void Render()
	{
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
		SetSize();
	}

	private void SetSize()
	{
		var screenSize = ImGui.GetIO().DisplaySize;
		Vector2D<int> baseResolution = new Vector2D<int>(1920, 1080);

		float scaleFactorX = screenSize.X / (float) baseResolution.X;
		float scaleFactorY = screenSize.Y / (float) baseResolution.Y;
		float scaleFactor =
			Math.Min(scaleFactorX, scaleFactorY);

		ImGui.GetIO().FontGlobalScale = scaleFactor;
	}
}