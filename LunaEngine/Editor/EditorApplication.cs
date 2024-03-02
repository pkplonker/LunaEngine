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
using Debug = Engine.Logging.Debug;
using MessageBox = Editor.Controls.MessageBox;

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

		private EditorApplication()
		{
			Setup();
			renderer = new Renderer();
		}

		private void Setup()
		{
			Debug.Start();
			Debug.AddSink(new ConsoleLogSink());
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
						Debug.Error("An error occurred during window run: " + ex.Message);
					}

					if (imGuiController != null)
					{
						imGuiController.Dispose();
						imGuiController = null;
					}
				}
				catch (Exception ex)
				{
					Debug.Error("An error occurred during shutdown: " + ex.Message);
				}
				finally
				{
					try
					{
						window?.Dispose();
					}
					catch (Exception e)
					{
						Debug.Error("Failed to dispose");
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
						MessageBox.Show("Are you sure you want to do close?",
							() => { window.Close(); });
					}
				};
				editorCamera = new MoveableEditorCamera(Vector3.UnitZ * 6, WINDOW_SIZE_X / (float) WINDOW_SIZE_Y);
				SceneController.ActiveScene.ActiveCamera = editorCamera;
				imGuiController = new EditorImGuiController(renderer.Gl, window, inputContext, renderer, editorCamera,
					inputController);
				renderer.AddScene(SceneController.ActiveScene, new Vector2D<uint>(0, 0), out _);
				try
				{
					window.SetWindowIcon(
						new ReadOnlySpan<RawImage>(LoadIcon(@"/resources/core/TransparentLunaSmall.png"
							.MakeAbsolute())));
				}
				catch (Exception e)
				{
					Debug.Error(e);
				}
			}
			else
			{
				throw new NullReferenceException($"{nameof(window)} cannot be null");
			}

			// for (int i = 0; i < 50; i++)
			// {
			// 	Debug.Log($"Testing{i}");
			// }
			// for (int i = 0; i < 50; i++)
			// {
			// 	Debug.Warning($"Testing{i}");
			// }
			// for (int i = 0; i < 50; i++)
			// {
			// 	Debug.Info($"Testing{i}");
			// }
			// for (int i = 0; i < 50; i++)
			// {
			// 	Debug.Error($"Testing");
			// }
			Debug.Warning("Testing22");
			Debug.Log("Testing33");

			PerformTest();
		}

		private void PerformTest()
		{
			// var test = new GameObject();
			// test.AddComponent<TestComponent>();
			// test.Name = "Test";

			var cube = new GameObject();
			cube.Name = "Cube";

			// SceneController.ActiveScene.AddGameObject(cube);
			// cube.AddComponent<RotateComponent>();
			// cube.AddComponent<MeshFilter>()
			// 	?.AddMesh(ResourceManager.GetMesh(@"models/TestCube.obj".MakeProjectAbsolute()));
			// cube.AddComponent<MeshRenderer>().Material = new Material(
			// 	ResourceManager.GetShader(
			// 		@"/shaders/PBRVertex.glsl".MakeProjectAbsolute(),
			// 		@"shaders/PBRFragment.glsl".MakeProjectAbsolute()
			// 	));
			//
			// cube.GetComponent<MeshRenderer>().Material.Albedo =
			// 	ResourceManager.GetTexture(@"textures/uvgrid.png".MakeProjectAbsolute());
			// cube.GetComponent<MeshRenderer>().Material.Normal =
			// 	ResourceManager.GetTexture(@"textures/uvgrid.png".MakeProjectAbsolute());

			var sphere = new GameObject();
			sphere.Name = "Sphere";

			SceneController.ActiveScene.AddGameObject(sphere);
			sphere.AddComponent<RotateComponent>();
			sphere.AddComponent<MeshFilter>()
				?.AddMesh(ResourceManager.GetMesh(@"/models/TestSphere.obj".MakeProjectAbsolute()));
			sphere.Transform.Translate(new Vector3(1.5f, 0, 0));
			sphere.AddComponent<MeshRenderer>().Material = new Material(
				ResourceManager.GetShader(
					@"/shaders/PBRVertex.glsl".MakeProjectAbsolute(),
					@"shaders/PBRFragment.glsl".MakeProjectAbsolute()
				));

			// sphere.GetComponent<MeshRenderer>().Material.Albedo =
			// 	ResourceManager.GetTexture(@"textures/uvgrid.png".MakeProjectAbsolute());
			// sphere.GetComponent<MeshRenderer>().Material.Normal =
			// 	ResourceManager.GetTexture(@"/textures/uvgrid.png".MakeProjectAbsolute());
			// go.GetComponent<MeshRenderer>().Material.Metallic =
			// 	ResourceManager.GetTexture(@"/resources/textures/uvgrid.png");
			// go.GetComponent<MeshRenderer>().Material.Roughness =
			// 	ResourceManager.GetTexture(@"/resources/textures/uvgrid.png");
			// go.GetComponent<MeshRenderer>().Material.AO =
			// 	ResourceManager.GetTexture(@"/resources/textures/uvgrid.png");
			// go.Name = "Sphere";

			//
			// var go2 = new GameObject();
			// go2.AddComponent<RotateComponent>();
			// go2.AddComponent<MeshFilter>()?.AddMesh(ResourceManager.GetMesh(@"/resources/models/TestCube.obj"));
			// go2.AddComponent<MeshRenderer>().Material = new Material(ResourceManager.GetShader());
			// go2.Transform.Position += new Vector3(1, 0, 0);
			// go2.Name = "Cube";
			//
			// var plane = new GameObject();
			// plane.AddComponent<MeshFilter>()?.AddMesh(ResourceManager.GetMesh(@"/resources/models/plane.fbx"));
			// plane.AddComponent<MeshRenderer>().Material = new Material(ResourceManager.GetShader());
			// plane.Transform.Position += new Vector3(0, 3, 0);
			// plane.Name = "Plane";
		}

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