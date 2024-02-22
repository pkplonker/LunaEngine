using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.Windowing;

namespace Engine
{
	class Application
	{
		private const int windowSizeX = 3840;
		private const int windowSizeY = 2160;
		private const string windowName = "Luna Engine";

		private IWindow window;
		private Renderer renderer;

		private Application()
		{
			renderer = new Renderer();
			SetupWindow();
		}

		private void SetupWindow()
		{
			var options = WindowOptions.Default;
			options.Size = new Vector2D<int>(windowSizeX, windowSizeY);
			options.Title = windowName;
			window = Window.Create(options);

			window.Load += OnLoad;
			window.Update += OnUpdate;
			window.Render += OnRender;
			window.Resize += OnWindowResize;
			window.Closing += OnClose;

		}

		private void OnClose()
		{
			renderer.Close();
		}

		public void Start()
		{
			window.Run();
			
			window.Dispose();
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
		}

		private void OnRender(double deltaTime)
		{
			renderer.Update(deltaTime);
		}

		private void OnUpdate(double deltaTime)
		{
		}

		private void KeyDown(IKeyboard keyboard, Key key, int arg3)
		{
			if (key == Key.Escape)
			{
				window.Close();
			}
		}

		public static Application GetApplication() => new();
	}
}