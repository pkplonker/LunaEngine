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
		private IInputContext? input;
		private Camera? camera;
		private static Vector2 LastMousePosition;
		private IKeyboard? primaryKeyboard;

		private EditorApplication()
		{
			Setup();
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
			renderer = new Renderer();
		}

		private void OnClose()
		{
			renderer.Close();
		}

		public void Start()
		{
			if (window != null)
			{
				//window.WindowState = WindowState.Maximized;
				window.Run();
				window.Dispose();
			}

			imGuiController?.Dispose();
		}

		private void OnWindowResize(Vector2D<int> size)
		{
			renderer?.Resize(size);
			imGuiController.Resize(size);
		}

		private void OnLoad()
		{
			if (window != null)
			{
				renderer?.Load(window);
				input = window.CreateInput();
				camera = new Camera(Vector3.UnitZ * 6, Vector3.UnitZ * -1, Vector3.UnitY,
					(float) WINDOW_SIZE_X / (float) WINDOW_SIZE_Y);
				imGuiController = new EditorImGuiController(renderer.Gl, window, input, renderer);
			}

			primaryKeyboard = input.Keyboards.FirstOrDefault();
			if (primaryKeyboard != null)
			{
				primaryKeyboard.KeyDown += KeyDown;
			}
			// for (int i = 0; i < input.Mice.Count; i++)
			// {
			// 	input.Mice[i].Cursor.CursorMode = CursorMode.Raw;
			// 	input.Mice[i].MouseMove += OnMouseMove;
			// 	input.Mice[i].Scroll += OnMouseWheel;
			// }
		}

		private void OnMouseMove(IMouse mouse, Vector2 position)
		{
			const float lookSensitivity = 0.1f;
			if (LastMousePosition == default)
			{
				LastMousePosition = position;
			}
			else
			{
				var xOffset = (position.X - LastMousePosition.X) * lookSensitivity;
				var yOffset = (position.Y - LastMousePosition.Y) * lookSensitivity;
				LastMousePosition = position;

				camera.ModifyDirection(xOffset, yOffset);
			}
		}

		private void OnMouseWheel(IMouse mouse, ScrollWheel scrollWheel)
		{
			camera.ModifyZoom(scrollWheel.Y);
		}

		private void OnRender(double deltaTime)
		{
			renderer?.Update(deltaTime, camera?.GetView(), camera?.GetProjection());
			imGuiController?.Render();
		}

		private void OnUpdate(double deltaTime)
		{
			var moveSpeed = 2.5f * (float) deltaTime;

			if (primaryKeyboard.IsKeyPressed(Key.W))
			{
				//Move forwards
				camera.Position += moveSpeed * camera.Front;
			}

			if (primaryKeyboard.IsKeyPressed(Key.S))
			{
				//Move backwards
				camera.Position -= moveSpeed * camera.Front;
			}

			if (primaryKeyboard.IsKeyPressed(Key.A))
			{
				//Move left
				camera.Position -= Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * moveSpeed;
			}

			if (primaryKeyboard.IsKeyPressed(Key.D))
			{
				//Move right
				camera.Position += Vector3.Normalize(Vector3.Cross(camera.Front, camera.Up)) * moveSpeed;
			}

			imGuiController?.Update((float) deltaTime);
		}

		private void KeyDown(IKeyboard keyboard, Key key, int arg3)
		{
			if (key == Key.Escape)
			{
				window.Close();
			}
		}

		public static EditorApplication GetApplication() => application ??= new EditorApplication();
	}
}