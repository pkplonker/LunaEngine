using System.Numerics;
using Engine.Logging;
using Silk.NET.GLFW;
using Silk.NET.Input;
using MouseButton = Silk.NET.Input.MouseButton;

namespace Engine;

public class InputController : IInputController
{
	private readonly IInputContext input;
	private Vector2 lastMousePosition;
	private Vector2 currentMousePosition;
	private Vector2 mouseDelta;
	private HashSet<IInputController.Key> currentKeyPresses = new();

	private HashSet<IInputController.MouseButton> currentMousePresses = new();
	private HashSet<IInputController.MouseButton> previousMousePresses = new();

	private Stack<Func<IInputController.Key, IInputController.InputState, bool>> keyHandlers = new();
	private Stack<Func<IInputController.MouseButton, IInputController.InputState, bool>> mouseButtonHandlers = new();

	public event Action<float, float> MouseScroll;
	public event Action<float, float> MouseMove;

	public InputController(IInputContext input)
	{
		this.input = input;

		foreach (var keyboard in input.Keyboards)
		{
			keyboard.KeyDown += KeyDown;
			keyboard.KeyUp += KeyUp;
		}

		foreach (var mouse in input.Mice)
		{
			mouse.MouseMove += OnMouseMove;
			mouse.Scroll += OnMouseWheel;
			mouse.MouseDown += MouseButtonDown;
			mouse.MouseUp += MouseButtonUp;
			lastMousePosition = new Vector2(mouse.Position.X, mouse.Position.Y);
		}
	}

	public void SubscribeToKeyEvent(Func<IInputController.Key, IInputController.InputState, bool> handler) =>
		keyHandlers.Push(handler);

	public void UnsubscribeToKeyEvent(Func<IInputController.Key, IInputController.InputState, bool> handler)
	{
		var handlers = keyHandlers.ToList();
		handlers.Remove(handler);
		keyHandlers = new Stack<Func<IInputController.Key, IInputController.InputState, bool>>(handlers);
	}

	public void SubscribeToMouseButtonEvent(
		Func<IInputController.MouseButton, IInputController.InputState, bool> handler) =>
		mouseButtonHandlers.Push(handler);

	public void UnsubscribeToMouseButtonEvent(
		Func<IInputController.MouseButton, IInputController.InputState, bool> handler)
	{
		var handlers = mouseButtonHandlers.ToList();
		handlers.Remove(handler);
		mouseButtonHandlers =
			new Stack<Func<IInputController.MouseButton, IInputController.InputState, bool>>(handlers);
	}

	private void KeyDown(IKeyboard device, Silk.NET.Input.Key key, int arg3)
	{
		currentKeyPresses.Add((IInputController.Key) key);
		foreach (var handler in keyHandlers)
		{
			if (handler((IInputController.Key) key, IInputController.InputState.Pressed))
				return;
		}
	}

	private void KeyUp(IKeyboard device, Silk.NET.Input.Key key, int arg3)
	{
		currentKeyPresses.Remove((IInputController.Key) key);
		foreach (var handler in keyHandlers)
		{
			if (handler((IInputController.Key) key, IInputController.InputState.Released))
				return;
		}
	}

	private void MouseButtonDown(IMouse mouse, MouseButton button)
	{
		currentMousePresses.Add((IInputController.MouseButton) button);
		foreach (var handler in mouseButtonHandlers)
		{
			if (handler((IInputController.MouseButton) button, IInputController.InputState.Pressed))
				return;
		}
	}

	private void MouseButtonUp(IMouse mouse, MouseButton button)
	{
		currentMousePresses.Remove((IInputController.MouseButton) button);
		foreach (var handler in mouseButtonHandlers)
		{
			if (handler((IInputController.MouseButton) button, IInputController.InputState.Released))
				return;
		}
	}

	private void OnMouseWheel(IMouse device, ScrollWheel args)
	{
		MouseScroll?.Invoke(args.X, args.Y);
	}

	private void OnMouseMove(IMouse device, Vector2 args)
	{
		currentMousePosition = new Vector2(args.X, args.Y);
		mouseDelta = currentMousePosition - lastMousePosition;
		MouseMove?.Invoke(args.X, args.Y);
		lastMousePosition = currentMousePosition;
	}

	public void Update()
	{
		foreach (var key in currentKeyPresses)
		{
			foreach (var handler in keyHandlers)
			{
				if (handler(key, IInputController.InputState.Held))
					break;
			}
		}

		mouseDelta = Vector2.Zero;
	}

	public Vector2 GetMouseDelta() => mouseDelta;

	public bool IsKeyHeld(IInputController.Key key) => currentKeyPresses.Contains(key);
	public bool IsMousePressed(IInputController.MouseButton button) => currentMousePresses.Contains(button);
	public Vector2 GetMousePosition() => currentMousePosition;

	public bool IsKeyPressed(IInputController.Key key)
	{
		return currentKeyPresses.Contains(key);
	}

	public void UnsubscribeFromMouseButtonPress(Func<IInputController.MouseButton, bool, bool> handler)
	{
		throw new NotImplementedException();
	}
}