using System.Numerics;
using Engine;
using ImGuiNET;
using Silk.NET.Maths;

namespace Editor.Controls;

public class EditorViewport
{
	private Vector2 currentSize;

	private Vector2D<float> CalculateSizeForAspectRatio(Vector2D<float> currentSize, float aspectRatio)
	{
		float currentAspectRatio = currentSize.X / currentSize.Y;

		float newWidth, newHeight;

		if (currentAspectRatio > aspectRatio)
		{
			newHeight = currentSize.Y;
			newWidth = newHeight * aspectRatio;
		}
		else
		{
			newWidth = currentSize.X;
			newHeight = newWidth / aspectRatio;
		}

		return new Vector2D<float>(newWidth, newHeight);
	}

	public void Update(string panelName, IEditorCamera camera, IScene? scene, IInputController inputController,
		IRenderer renderer)
	{
		ImGui.Begin(panelName,
			ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse);
		Vector2 size = ImGui.GetContentRegionAvail();
		if (ImGui.IsWindowFocused() || (ImGui.IsWindowHovered() && ImGui.IsMouseClicked(ImGuiMouseButton.Right)))
		{
			ImGui.SetWindowFocus();
			camera.SetActive(true,inputController);
		}
		else
		{
			camera.SetActive(false,inputController);
		}

		if (scene != null)
		{
			float aspectRatio = 16f / 9f;
			Vector2D<float> aspectSize =
				CalculateSizeForAspectRatio(new Vector2D<float>(size.X, size.Y), aspectRatio);

			if (size != currentSize)
			{
				renderer.SetRenderTargetSize(scene, aspectSize);
				camera.AspectRatio = aspectRatio;
				currentSize = size;
			}

			IRenderTarget? rt = renderer.GetSceneRenderTarget(scene);
			if (rt != null && rt is FrameBufferRenderTarget fbrtt)
			{
				Vector2 offset = new Vector2((size.X - aspectSize.X) * 0.5f, (size.Y - aspectSize.Y) * 0.5f);

				var drawList = ImGui.GetWindowDrawList();
				var windowPos = ImGui.GetWindowPos();
				var windowSize = ImGui.GetWindowSize();
				var barColor = new Vector4(0, 0, 0, 1);
				drawList.AddRectFilled(windowPos, windowPos + new Vector2(windowSize.X, windowSize.Y),
					ImGui.ColorConvertFloat4ToU32(barColor));

				ImGui.SetCursorPos(offset);

				ImGui.Image(fbrtt.GetTextureHandlePtr(),
					(Vector2) aspectSize, Vector2.Zero,
					Vector2.One,
					Vector4.One,
					Vector4.Zero);
			}
		}

		ImGui.End();
	}
}