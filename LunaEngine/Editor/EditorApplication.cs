using Engine;
using ImGuiNET;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;
using Silk.NET.OpenGL.Extensions.ImGui;
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
		private ImGuiController imGuiController;

		private EditorApplication()
		{
			SetupWindow();
		}

		private void SetupWindow()
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
			window.Run();

			window.Dispose();
			imGuiController.Dispose();
		}

		private void OnWindowResize(Vector2D<int> size)
		{
			renderer.Resize(size);
		}

		private void OnLoad()
		{
			IInputContext input = window.CreateInput();
			foreach (var keyboard in input.Keyboards)
			{
				keyboard.KeyDown += KeyDown;
			}

			renderer.Load(window);
			imGuiController = new ImGuiController(renderer.Gl, window, input);
		}

		private void OnRender(double deltaTime)
		{
			renderer.Update(deltaTime);
			ImGui.ShowDemoWindow();
			ImGui.ShowAboutWindow();
			ImGui.ShowDebugLogWindow();

			imGuiController.Render();
		}

		private void OnUpdate(double deltaTime)
		{
			imGuiController.Update((float)deltaTime);
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