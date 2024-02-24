using System.Numerics;
using Editor.Controls;
using Engine;
using ImGuiNET;
using Silk.NET.Assimp;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace Editor;

public class EditorImGuiController : IDisposable
{
	private readonly Renderer renderer;
	private ImGuiController imGuiController;
	private Vector2 previousSize;
	private Dictionary<IPanel, bool> controls = new();
	private readonly EditorCamera editorCamera;

	public EditorImGuiController(GL gl, IView view, IInputContext input, Renderer renderer, EditorCamera editorCamera)
	{
		this.renderer = renderer;
		imGuiController = new ImGuiController(gl, view, input);
		var io = ImGui.GetIO();
		io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
		io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
		ImGuiTheme.ApplyTheme(0);
		SetSize();
		CreateControls(editorCamera);
		this.editorCamera = editorCamera;
	}

	public GameObject SelectedGameObject { get; set; }

	private void CreateControls(EditorCamera editorCamera)
	{
		controls.Add(new Stats(), true);
		controls.Add(new EditorCameraPanel(editorCamera), true);
		controls.Add(new UndoRedoPanel(), true);
		controls.Add(new HierarchyPanel(this),true);
		controls.Add(new InspectorPanel(this),true);

	}

	public void ImGuiControllerUpdate(float deltaTime)
	{
		using var tracker = new PerformanceTracker(nameof(ImGuiControllerUpdate));

		imGuiController.Update(deltaTime);
		ImGui.DockSpaceOverViewport();
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0);
		ImGui.Begin("Viewport", ImGuiWindowFlags.NoScrollbar);
		var size = ImGui.GetContentRegionAvail();
		if (size != previousSize)
		{
			renderer.SetRenderTargetSize(new Vector2D<float>(size.X, size.Y));
			previousSize = size;
		}

		editorCamera.IsWindowFocused = ImGui.IsWindowFocused();
		
		ImGui.Image((IntPtr) renderer.renderTexture.Handle, new Vector2(size.X, size.Y), Vector2.Zero, Vector2.One,
			Vector4.One,
			Vector4.Zero);
		ImGui.End();
		ImGui.PopStyleVar();

		foreach (var control in controls.Where(x => x.Value).Select(x => x.Key))
		{
			control.Draw(renderer);
		}

		ImGui.ShowDemoWindow();
		MessageBox.Render();
	}

	public void Render()
	{
		imGuiController.Render();
	}

	public void Dispose()
	{
		imGuiController?.Dispose();
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