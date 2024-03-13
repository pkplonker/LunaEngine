﻿using System.Numerics;
using Editor.Controls;
using Engine;
using Engine.Logging;
using ImGuiNET;
using Silk.NET.Assimp;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using Debug = System.Diagnostics.Debug;
using File = System.IO.File;
using Material = Engine.Material;
using Metadata = Engine.Metadata;
using ProgressBar = Editor.Controls.ProgressBar;
using Scene = Engine.Scene;
using Shader = Engine.Shader;

namespace Editor;

public class EditorImGuiController : IDisposable
{
	private readonly IRenderer renderer;
	private ImGuiController imGuiController;
	public Vector2 CurrentSize { get; private set; }
	private Dictionary<IPanel, bool> controls = new();
	private readonly IEditorCamera editorCamera;
	private const string iniSaveLocation = "imgui.ini";
	public event Action<GameObject?> GameObjectSelectionChanged;
	private GameObject? selectedGameObject;
	private readonly IInputController inputController;
	private SettingsPanel settingsPanel;
	private bool openSettings;
	private bool showDemo;
	private readonly EditorViewport editorViewport;
	private CreateProjectWindow createProjectWindow;
	private const string EDITOR_CATEGORY = "EditorPanels";

	public GameObject? SelectedGameObject
	{
		get => selectedGameObject;
		set
		{
			if (value != selectedGameObject)
			{
				selectedGameObject = value;
				GameObjectSelectionChanged?.Invoke(SelectedGameObject);
			}
		}
	}

	public EditorImGuiController(GL gl, IView view, IInputContext input, IRenderer renderer, IEditorCamera editorCamera,
		IInputController inputController)
	{
		createProjectWindow = new CreateProjectWindow();
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
		this.editorCamera = editorCamera;
		this.inputController = inputController;
		CreateControls(editorCamera);
		showDemo = EditorSettings.GetSetting("showDemo", EDITOR_CATEGORY, false, false);
		editorViewport = new EditorViewport();
	}

	private void CreateControls(IEditorCamera editorCamera)
	{
		controls.Add(new StatsPanel(inputController), true);
		controls.Add(new EditorCameraPanel(editorCamera), true);
		controls.Add(new UndoRedoPanel(inputController), true);
		controls.Add(new HierarchyPanel(this, inputController), true);
		var inspector = new InspectorPanel(this);
		controls.Add(inspector, true);
		controls.Add(new ObjectPreviewPanel(inspector, inputController, renderer), true);
		controls.Add(new ImGuiLoggerWindow(), true);
		controls.Add(new MetadataPanel(), true);
		foreach (var control in controls)
		{
			controls[control.Key] =
				EditorSettings.GetSetting(control.Key.PanelName, EDITOR_CATEGORY, false, control.Value);
		}

		settingsPanel = new SettingsPanel(inputController);
	}

	public void ImGuiControllerUpdate(float deltaTime)
	{
		using PerformanceTracker tracker = new PerformanceTracker(nameof(ImGuiControllerUpdate));
		imGuiController.Update(deltaTime);
		ImGui.DockSpaceOverViewport(ImGui.GetMainViewport());
		ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, 0);
		var currentSize = CurrentSize;
		editorViewport.Update("Viewport", editorCamera, SceneController.ActiveScene, inputController, renderer,
			ref currentSize);
		CurrentSize = currentSize;
		ImGui.PopStyleVar();

		foreach (IPanel control in controls.Where(x => x.Value).Select(x => x.Key))
		{
			control.Draw(renderer);
		}

		if (showDemo)
		{
			ImGui.ShowDemoWindow();
		}

		DecisionBox.Render();
		InfoBox.Render();
		ProgressBar.Render();
		DrawMenu();
	}

	private void DrawMenu()
	{
		//todo remove test scene reference
		var testScenePath =
			Path.Combine(ProjectManager.ActiveProject?.Directory ?? string.Empty, "assets/TestScene.SCENE");
		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				if (ImGui.MenuItem("New Project", "Ctrl+N"))
				{
					createProjectWindow.Create();
				}

				if (ImGui.MenuItem("Save Scene", "Ctrl+S"))
				{
					var pu = new ProgressUpdater();
					ProgressBar.Show("Opening Scene", progressUpdate: pu);
					var result = new SceneSerializer(SceneController.ActiveScene,
						testScenePath).Serialize(progress: pu);
					ProgressBar.Close();
				}

				if (ImGui.MenuItem("Open Project", "Ctrl+N"))
				{
					ProjectManager.LoadProject(FileDialog
						.OpenFileDialog(FileDialog.BuildFileDialogFilter(new List<string>()
							{ProjectManager.ProjectExtension})).FirstOrDefault());
				}

				ImGui.EndMenu();
			}

			if (ImGui.MenuItem("Settings"))
			{
				openSettings = true;
			}

			if (ImGui.BeginMenu("Windows"))
			{
				foreach (var control in controls)
				{
					var isVisible = control.Value;
					if (ImGui.MenuItem(control.Key.PanelName, null, isVisible))
					{
						controls[control.Key] = !isVisible;
						EditorSettings.SaveSetting(control.Key.PanelName, controls[control.Key]);
					}
				}

				if (ImGui.MenuItem("Show IMGUI Demo", null, showDemo))
				{
					showDemo = !showDemo;
					EditorSettings.SaveSetting("showDemo", showDemo);
				}

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Tools"))
			{
				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Development"))
			{
				if (ImGui.BeginMenu("Test Actions"))
				{
					if (ImGui.MenuItem("Test Serialize Material"))
					{
						var mat = new Material(new Guid("5e0d8571-03d1-47b6-a658-ca6255c675a0"));
						ObjectSerializer.Serialize(mat, @"Assets\\Materials\\testmat2.mat".MakeProjectAbsolute());
					}

					if (ImGui.MenuItem("Test Deserialize  Material"))
					{
						try
						{
							var mat = ObjectSerializer.Deserialize(@"Assets\\Materials\\testmat2.mat"
								.MakeProjectAbsolute());
						}
						catch (Exception e)
						{
							Logger.Warning(e);
							throw;
						}
					}

					if (ImGui.MenuItem("Show progress"))
					{
						ProgressBar.Show("Test Title");
					}

					if (ImGui.MenuItem("CancelProgress"))
					{
						ProgressBar.Close();
					}

					ImGui.EndMenu();
				}

				if (ImGui.BeginMenu("Saves"))
				{
					if (ImGui.MenuItem("Open Scene"))
					{
						var pu = new ProgressUpdater();
						ProgressBar.Show("Opening Scene", progressUpdate: pu);
						Scene? result = new SceneDeserializer(testScenePath).Deserialize(ProgressUpdater: pu);
						ProgressBar.Close();

						if (result != null)
						{
							SceneController.ActiveScene = result;
						}
					}

					if (ImGui.MenuItem("New Scene"))
					{
						SceneController.ActiveScene = new Scene();
					}

					ImGui.EndMenu();
				}

				if (ImGui.BeginMenu("Metadata"))
				{
					if (ImGui.MenuItem("Clear Metadata"))
					{
						try
						{
							DecisionBox.Show("Are you sure you want to clear all metadata?", () =>
							{
								if (!string.IsNullOrEmpty(ProjectManager.ActiveProject?.Directory))
								{
									var paths = ResourceManager.Instance.GetFilesFromFolder(
										ProjectManager.ActiveProject.Directory,
										new[] {Metadata.MetadataFileExtension});
									foreach (var path in paths)
									{
										File.Delete(path);
									}

									ResourceManager.Instance.ClearMetadatas();
								}
							});
						}
						catch (Exception e)
						{
							Logger.Warning(e);
							throw;
						}
					}

					if (ImGui.MenuItem("Import All Metadata"))
					{
						ResourceManager.Instance.LoadMetadata(ProjectManager.ActiveProject?.Directory);
					}

					ImGui.EndMenu();
				}

				if (ImGui.BeginMenu("File Import"))
				{
					if (ImGui.MenuItem("File Import Selection"))
					{
						FileDialog.Import();
					}

					if (ImGui.MenuItem("Import All Files"))
					{
						if (!string.IsNullOrEmpty(ProjectManager.ActiveProject?.Directory))
						{
							FileImporter.ImportAllFromDirectory(ProjectManager.ActiveProject?.Directory ??
							                                    string.Empty);
						}
					}

					ImGui.EndMenu();
				}

				ImGui.EndMenu();
			}

			ImGui.EndMainMenuBar();

			if (openSettings)
			{
				ImGui.OpenPopup("Settings");
				openSettings = false;
			}

			settingsPanel.Draw(renderer);
			createProjectWindow.Draw();
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