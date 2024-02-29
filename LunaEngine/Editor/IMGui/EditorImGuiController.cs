﻿using System.Numerics;
using Editor.Controls;
using Engine;
using ImGuiNET;
using Silk.NET.Assimp;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using MessageBox = Editor.Controls.MessageBox;

namespace Editor;

public class EditorImGuiController : IDisposable
{
	private readonly Renderer renderer;
	private ImGuiController imGuiController;
	private Vector2 previousSize;
	private Dictionary<IPanel, bool> controls = new();
	private readonly EditorCamera editorCamera;
	private const string iniSaveLocation = "imgui.ini";

	public EditorImGuiController(GL gl, IView view, IInputContext input, Renderer renderer, EditorCamera editorCamera)
	{
		this.renderer = renderer;
		imGuiController = new ImGuiController(gl, view, input);
		var io = ImGui.GetIO();
		io.ConfigFlags |= ImGuiConfigFlags.DockingEnable;
		io.ConfigFlags |= ImGuiConfigFlags.ViewportsEnable;
		io.ConfigFlags |= ImGuiConfigFlags.DpiEnableScaleFonts;
		io.ConfigFlags |= ImGuiConfigFlags.DpiEnableScaleViewports;
		ImGui.LoadIniSettingsFromDisk(iniSaveLocation);

		ImGuiTheme.ApplyTheme(0);
		SetSize();
		CreateControls(editorCamera);
		this.editorCamera = editorCamera;
	}

	public GameObject? SelectedGameObject { get; set; }

	private void CreateControls(EditorCamera editorCamera)
	{
		controls.Add(new Stats(), true);
		controls.Add(new EditorCameraPanel(editorCamera), true);
		controls.Add(new UndoRedoPanel(), true);
		controls.Add(new HierarchyPanel(this), true);
		controls.Add(new InspectorPanel(this), true);
	}

	public void ImGuiControllerUpdate(float deltaTime)
	{
		using var tracker = new PerformanceTracker(nameof(ImGuiControllerUpdate));

		imGuiController.Update(deltaTime);
		ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0);
		ImGui.Begin("Viewport",
			ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
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

		foreach (var control in controls.Where(x => x.Value).Select(x => x.Key))
		{
			control.Draw(renderer);
		}

		ImGui.ShowDemoWindow();
		MessageBox.Render();
		DrawMenu();
	}

	private void DrawMenu()
	{
		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				if (ImGui.MenuItem("New", "Ctrl+N"))
				{
					//New();
				}

				if (ImGui.MenuItem("Save", "Ctrl+S"))
				{
					//Save();
				}

				if (ImGui.MenuItem("Save As", "Ctrl+Shft+S"))
				{
					//SaveAs();
				}

				if (ImGui.MenuItem("Open", "Ctrl+O"))
				{
					//LoadScene();
				}

				ImGui.EndMenu();
			}
			if (ImGui.MenuItem("Add Scene"))
			{
				//LoadScene();
			}
			if (ImGui.MenuItem("Settings"))
			{
				//openSettings = true;
			}

			if (ImGui.BeginMenu("Windows"))
			{
				foreach (var control in controls)
				{
					var isVisible = control.Value;
					if (ImGui.MenuItem(control.Key.PanelName, null, isVisible))
					{
						controls[control.Key] = !isVisible;
					}
				}

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Tools")) { }

			ImGui.EndMainMenuBar();
		}
	}

	public void Render()
	{
		imGuiController.Render();
	}

	public void Close()
	{
		Save();
	}

	public void Save()
	{
		ImGui.UpdatePlatformWindows();
		ImGui.RenderPlatformWindowsDefault();
		ImGui.SaveIniSettingsToDisk(iniSaveLocation);
	}

	public void Dispose()
	{
		Save();
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