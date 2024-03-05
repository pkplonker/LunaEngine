using System.Numerics;
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
using Material = Engine.Material;
using MessageBox = Editor.Controls.MessageBox;
using Scene = Engine.Scene;
using Shader = Engine.Shader;

namespace Editor;

public class EditorImGuiController : IDisposable
{
	private readonly Renderer renderer;
	private ImGuiController imGuiController;
	private Vector2 previousSize;
	private Dictionary<IPanel, bool> controls = new();
	private readonly EditorCamera editorCamera;
	private const string iniSaveLocation = "imgui.ini";
	public event Action<GameObject?> GameObjectSelectionChanged;
	private GameObject? selectedGameObject;
	private readonly InputController inputController;
	private SettingsPanel settingsPanel;
	private bool openSettings;
	private bool showDemo;
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

	public EditorImGuiController(GL gl, IView view, IInputContext input, Renderer renderer, EditorCamera editorCamera,
		InputController inputController)
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
		this.editorCamera = editorCamera;
		this.inputController = inputController;
		CreateControls(editorCamera);
		showDemo = EditorSettings.GetSetting("showDemo", EDITOR_CATEGORY, false, false);
	}

	private void CreateControls(EditorCamera editorCamera)
	{
		controls.Add(new Stats(inputController), true);
		controls.Add(new EditorCameraPanel(editorCamera), true);
		controls.Add(new UndoRedoPanel(), true);
		controls.Add(new HierarchyPanel(this, inputController), true);
		var inspector = new InspectorPanel(this);
		controls.Add(inspector, true);
		controls.Add(new ObjectPreviewPanel(inspector, inputController), true);
		controls.Add(new ImGuiLoggerWindow(), true);
		controls.Add(new MetadataViewer(), true);
		foreach (var control in controls)
		{
			controls[control.Key] =
				EditorSettings.GetSetting(control.Key.PanelName, EDITOR_CATEGORY, false, control.Value);
		}

		settingsPanel = new SettingsPanel();
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
		if (ImGui.IsWindowFocused() || (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right)))
		{
			ImGui.SetWindowFocus();
			editorCamera.Update(inputController);
		}

		if (SceneController.ActiveScene != null)
		{
			if (size != previousSize)
			{
				renderer.SetRenderTargetSize(SceneController.ActiveScene, new Vector2D<float>(size.X, size.Y));
				previousSize = size;
			}

			var rt = renderer.GetSceneRenderTarget(SceneController.ActiveScene);
			if (rt != null && rt is FrameBufferRenderTarget fbrtt)
			{
				ImGui.Image(fbrtt.GetTextureHandlePtr(),
					new Vector2(size.X, size.Y), Vector2.Zero,
					Vector2.One,
					Vector4.One,
					Vector4.Zero);
			}
		}

		ImGui.End();
		ImGui.PopStyleVar();

		foreach (var control in controls.Where(x => x.Value).Select(x => x.Key))
		{
			control.Draw(renderer);
		}

		if (showDemo)
		{
			ImGui.ShowDemoWindow();
		}

		MessageBox.Render();
		InfoBox.Render();
		DrawMenu();
	}

	private void DrawMenu()
	{
		var testScenePath = Path.Combine(ProjectManager.ActiveProject.Directory, "assets/TestScene.SCENE");
		if (ImGui.BeginMainMenuBar())
		{
			if (ImGui.BeginMenu("File"))
			{
				if (ImGui.MenuItem("New", "Ctrl+N"))
				{
					SceneController.ActiveScene = new Scene();
				}

				if (ImGui.MenuItem("Save", "Ctrl+S"))
				{
					var result = new SceneSerializer(SceneController.ActiveScene,
						testScenePath).Serialize();
				}

				if (ImGui.MenuItem("Save As", "Ctrl+Shft+S"))
				{
					//SaveAs();
				}

				if (ImGui.MenuItem("Open", "Ctrl+O"))
				{
					Scene? result = new SceneDeserializer(testScenePath).Deserialize();
					if (result != null)
					{
						SceneController.ActiveScene = result;
					}
				}

				ImGui.EndMenu();
			}

			if (ImGui.MenuItem("Add Scene"))
			{
				//LoadScene();
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
				if (ImGui.MenuItem("Test Material"))
				{
					ResourceManager.TryGetResourceByGuid(Guid.Parse("5e0d8571-03d1-47b6-a658-ca6255c675a0"),
						out var shader);
					var mat = new Material((Shader) shader);
					ObjectSerializer.Serialize(mat, @"Assets\\Materials\\testmat.mat".MakeProjectAbsolute());
				}

				ImGui.EndMenu();
			}

			if (ImGui.BeginMenu("Import"))
			{
				if (ImGui.MenuItem("File import"))
				{
					FileImporter.Import();
				}

				if (ImGui.MenuItem("Import All Files"))
				{
					if (!string.IsNullOrEmpty(ProjectManager.ActiveProject?.Directory))
					{
						FileImporter.ImportAllFromDirectory(ProjectManager.ActiveProject.Directory);
					}
				}

				if (ImGui.MenuItem("Import All Metadata"))
				{
					ResourceManager.LoadMetadata();
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