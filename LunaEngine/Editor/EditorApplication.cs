using System.Diagnostics;
using System.Numerics;
using Engine;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;

namespace Editor
{
	public class EditorApplication
	{
		public const int WINDOW_SIZE_X = 3840;
		public const int WINDOW_SIZE_Y = 2160;
		private const string WINDOW_NAME = "Luna Engine";
		private IWindow? window;
		private Renderer? renderer;
		private static EditorApplication application;
		private EditorImGuiController? imGuiController;
		private Camera? camera;
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

		private void OnClose()
		{
			renderer.Close();
		}

		public void Start()
		{
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
						Console.WriteLine("An error occurred during window run: " + ex.Message);
					}

					if (imGuiController != null)
					{
						imGuiController.Dispose();
						imGuiController = null;
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("An error occurred during shutdown: " + ex.Message);
				}
				finally
				{
					window.Dispose();
				}
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
				var inputContext = window.CreateInput();
				inputController = new InputController(inputContext);
				inputController.KeyPress += key =>
				{
					if (key == InputController.Key.Escape)
					{
						window.Close();
					}
				};
				camera = new Camera(Vector3.UnitZ * 6, Vector3.UnitZ * -1, Vector3.UnitY,
					(float) WINDOW_SIZE_X / (float) WINDOW_SIZE_Y);
				imGuiController = new EditorImGuiController(renderer.Gl, window, inputContext, renderer);
			}
			else
			{
				throw new NullReferenceException($"{nameof(window)} cannot be null");
			}

			ResourceManager.Init(renderer.Gl);

			PerformTest();
		}

		private void PerformTest()
		{
			var go = new GameObject();
			go.AddComponent<RotateComponent>();
			go.AddComponent<MeshFilter>()?.AddMesh(ResourceManager.GetMesh(@"/resources/models/TestSphere.obj"));
			go.AddComponent<MeshRenderer>().Shader = ResourceManager.GetShader();

			var go2 = new GameObject();
			go2.AddComponent<RotateComponent>();
			go2.AddComponent<MeshFilter>()?.AddMesh(ResourceManager.GetMesh(@"/resources/models/TestCube.obj"));
			go2.AddComponent<MeshRenderer>().Shader = ResourceManager.GetShader();
			go2.Transform.Position += new Vector3(1, 0, 0);
		}

		private void OnRender(double deltaTime)
		{
			renderer?.RenderUpdate(camera?.GetView(), camera?.GetProjection());
			imGuiController?.Render();
		}

		private void OnUpdate(double deltaTime)
		{
			Time.Update((float) window.Time);
			SceneController.ActiveScene?.Update();
			imGuiController?.ImGuiControllerUpdate((float) deltaTime);
			PerformanceTracker.ReportAverages();
		}

		public static EditorApplication GetApplication() => application ??= new EditorApplication();
	}
}