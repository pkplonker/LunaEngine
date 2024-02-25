using System.Diagnostics;
using System.Numerics;
using Editor.Controls;
using Engine;
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
			Settings.Settings.LoadSettings();
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
					window?.Dispose();
				}

				Settings.Settings.SaveSettings();
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
						MessageBox.Show("Are you sure you want to do close?",
							() => { window.Close(); });
					}
				};
				editorCamera = new EditorCamera(Vector3.UnitZ * 6, WINDOW_SIZE_X / (float) WINDOW_SIZE_Y);
				imGuiController = new EditorImGuiController(renderer.Gl, window, inputContext, renderer, editorCamera);
				try
				{
					window.SetWindowIcon(
						new ReadOnlySpan<RawImage>(LoadIcon(@"/resources/core/TransparentLunaSmall.png"
							.MakeAbsolute())));
				}
				catch (Exception e)
				{
					Console.Write(e);
				}
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
			var test = new GameObject();
			test.AddComponent<TestComponent>();
			test.Name = "Test";
			
			var go = new GameObject();
			go.AddComponent<RotateComponent>();
			go.AddComponent<MeshFilter>()?.AddMesh(ResourceManager.GetMesh(@"/resources/models/TestSphere.obj"));
			go.AddComponent<MeshRenderer>().Material = new Material(ResourceManager.GetShader());
			go.Name = "Sphere";
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
			renderer?.RenderUpdate(editorCamera?.GetView(), editorCamera?.GetProjection());
			imGuiController?.Render();
		}

		private void OnUpdate(double deltaTime)
		{
			Time.Update((float) window.Time);
			editorCamera.Update(inputController);
			SceneController.ActiveScene?.Update();
			imGuiController?.ImGuiControllerUpdate((float) deltaTime);
			PerformanceTracker.ReportAverages();
		}

		public static EditorApplication GetApplication() => application ??= new EditorApplication();
	}
}