using System.Diagnostics;
using System.Numerics;
using Editor.Controls;
using Engine;
using Engine.Logging;
using ImGuiNET;
using Silk.NET.Core;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using StbImageSharp;

namespace Editor
{
	public class EditorApplication
	{
		public const int WINDOW_SIZE_X = 3840;
		public const int WINDOW_SIZE_Y = 2160;
		private const string WINDOW_NAME = "Luna Engine - Stuart Heath - WIP";
		private IWindow? window;
		private Renderer? renderer;
		private static EditorApplication application;
		private EditorImGuiController? imGuiController;
		private EditorCamera? editorCamera;
		private static Vector2 LastMousePosition;
		private IKeyboard? primaryKeyboard;
		private InputController inputController;
		private FileWatcher fileWatcher;

		private EditorApplication()
		{
			Setup();
			renderer = new Renderer();
		}

		private void Setup()
		{
			Logger.Start();
			Logger.AddSink(new ConsoleLogSink());

			var options = WindowOptions.Default;
			options.Size = new Vector2D<int>(WINDOW_SIZE_X, WINDOW_SIZE_Y);
			options.Title = WINDOW_NAME;
			window = Window.Create(options);
			window.Load += OnLoad;
			window.Update += OnUpdate;
			window.Render += OnRender;
			window.Resize += OnWindowResize;
			window.Closing += OnClose;

			window.VSync = false;
		}

		private RawImage LoadIcon(string filePath)
		{
			byte[] imageData = File.ReadAllBytes(filePath);
			var imageResult = ImageResult.FromMemory(imageData, ColorComponents.RedGreenBlueAlpha);
			return new RawImage(imageResult.Width, imageResult.Height, imageResult.Data);
		}

		private void OnClose()
		{
			imGuiController.Close();
			renderer.Close();
		}

		public void Start()
		{
			EditorSettings.LoadSettings();

			if (window != null)
			{
				try
				{
					try
					{
						window.Run();
					}
					catch (Exception ex)
					{
						Logger.Error("An error occurred during window run: " + ex.Message);
					}

					if (imGuiController != null)
					{
						imGuiController.Dispose();
						imGuiController = null;
					}
				}
				catch (Exception ex)
				{
					Logger.Error("An error occurred during shutdown: " + ex.Message);
				}
				finally
				{
					try
					{
						window?.Dispose();
					}
					catch (Exception e)
					{
						Logger.Error("Failed to dispose");
					}
				}

				EditorSettings.SaveSettings();
			}
		}

		private void OnWindowResize(Vector2D<int> size)
		{
			renderer?.Resize(size);
			imGuiController.Resize(size);
		}

		private void OnLoad()
		{
			SceneController.ActiveScene = new Scene();

			if (window != null)
			{
				renderer?.Load(window);
				ResourceManager.Init(renderer.Gl);
				var inputContext = window.CreateInput();
				inputController = new InputController(inputContext);
				inputController.KeyPress += key =>
				{
					if (key == InputController.Key.Escape)
					{
						DecisionBox.Show("Are you sure you want to do close?",
							() => { window.Close(); });
					}
				};
				editorCamera = new MoveableEditorCamera(Vector3.UnitZ * 6, WINDOW_SIZE_X / (float) WINDOW_SIZE_Y);
				SceneController.ActiveScene.ActiveCamera = editorCamera;
				imGuiController = new EditorImGuiController(renderer.Gl, window, inputContext, renderer, editorCamera,
					inputController);

				// hack
				SceneController.OnActiveSceneChanged += scene =>
				{
					renderer.AddScene(scene, new Vector2D<uint>(0, 0), out _, true);
					scene.ActiveCamera = editorCamera;
					var size = imGuiController.CurrentSize;
					renderer.SetRenderTargetSize(SceneController.ActiveScene, new Vector2D<float>(size.X, size.Y));
				};

				try
				{
					window.SetWindowIcon(
						new ReadOnlySpan<RawImage>(LoadIcon(@"/resources/core/TransparentLunaSmall.png"
							.MakeAbsolute())));
				}
				catch (Exception e)
				{
					Logger.Error(e);
				}
			}
			else
			{
				throw new NullReferenceException($"{nameof(window)} cannot be null");
			}

			//PerformTest();
			fileWatcher = new FileWatcher(ProjectManager.ActiveProject.Directory);
		}

		// private void PerformTest()
		// {
		// 	var cube = new GameObject();
		// 	cube.Name = "Cube";
		//
		// 	cube.AddComponent<RotateComponent>();
		// 	cube.AddComponent<MeshFilter>()
		// 		?.AddMesh(ResourceManager.GetMesh(@"assets/models/TestCube.obj".MakeProjectAbsolute()));
		// 	cube.AddComponent<MeshRenderer>().Material = new Material(
		// 		ResourceManager.GetShader(
		// 			@"assets/shaders/PBRVertex.glsl".MakeProjectAbsolute(),
		// 			@"assets/shaders/PBRFragment.glsl".MakeProjectAbsolute()
		// 		));
		//
		// 	cube.GetComponent<MeshRenderer>().Material.Albedo =
		// 		ResourceManager.GetTexture(@"assets/textures/uvgrid.png".MakeProjectAbsolute());
		// 	cube.GetComponent<MeshRenderer>().Material.Normal =
		// 		ResourceManager.GetTexture(@"assets/textures/uvgrid.png".MakeProjectAbsolute());
		// 	cube.Transform.SetParent(SceneController.ActiveScene);
		//
		// 	var sphere = new GameObject();
		// 	sphere.Name = "Sphere";
		// 	
		// 	sphere.AddComponent<RotateComponent>();
		// 	sphere.AddComponent<MeshFilter>()
		// 		?.AddMesh(ResourceManager.GetMesh(@"assets/models/TestSphere.obj".MakeProjectAbsolute()));
		// 	sphere.Transform.Translate(new Vector3(1.5f, 0, 0));
		// 	sphere.AddComponent<MeshRenderer>().Material = new Material(
		// 		ResourceManager.GetShader(
		// 			@"assets/shaders/PBRVertex.glsl".MakeProjectAbsolute(),
		// 			@"assets/shaders/PBRFragment.glsl".MakeProjectAbsolute()
		// 		));
		// 	sphere.Transform.SetParent(SceneController.ActiveScene);
		// 	
		// 	var sphereChild = new GameObject();
		// 	sphereChild.Name = "SphereChild";
		// 	
		// 	sphereChild.AddComponent<RotateComponent>();
		// 	sphereChild.AddComponent<MeshFilter>()
		// 		?.AddMesh(ResourceManager.GetMesh(@"assets/models/TestSphere.obj".MakeProjectAbsolute()));
		// 	sphereChild.Transform.Translate(new Vector3(-2f, 1.5f, 0));
		// 	sphereChild.AddComponent<MeshRenderer>().Material = new Material(
		// 		ResourceManager.GetShader(
		// 			@"assets/shaders/PBRVertex.glsl".MakeProjectAbsolute(),
		// 			@"assets/shaders/PBRFragment.glsl".MakeProjectAbsolute()
		// 		));
		// 	
		// 	sphereChild.Transform.SetParent(sphere.Transform);
		// 	
		// 	var sphereChild2 = new GameObject();
		// 	sphereChild2.Name = "SphereChild2";
		// 	
		// 	sphereChild2.AddComponent<RotateComponent>();
		// 	sphereChild2.AddComponent<MeshFilter>()
		// 		?.AddMesh(ResourceManager.GetMesh(@"assets/models/TestSphere.obj".MakeProjectAbsolute()));
		// 	sphereChild2.Transform.Translate(new Vector3(-2f, 0, 0));
		// 	sphereChild2.AddComponent<MeshRenderer>().Material = new Material(
		// 		ResourceManager.GetShader(
		// 			@"assets/shaders/PBRVertex.glsl".MakeProjectAbsolute(),
		// 			@"assets/shaders/PBRFragment.glsl".MakeProjectAbsolute()
		// 		));
		// 	
		// 	sphereChild2.Transform.SetParent(sphereChild.Transform);
		// }

		private void OnRender(double deltaTime)
		{
			renderer?.RenderUpdate();
			imGuiController?.Render();
		}

		private void OnUpdate(double deltaTime)
		{
			Time.Update((float) window.Time);
			SceneController.ActiveScene?.Update();
			imGuiController?.ImGuiControllerUpdate((float) deltaTime);
			LateUpdate();
			PerformanceTracker.ReportAverages();
		}

		private void LateUpdate()
		{
			inputController.Update();
		}

		public static EditorApplication GetApplication() => application ??= new EditorApplication();
	}
}