using Engine;
using ImGuiNET;
using System;

namespace Editor.Controls;

public class Stats : IControl
{
	public string PanelName { get; set; } = "Stats";

	public static event Action<IControl> RegisterPanel;

	private double lastUpdateTime = 0;
	private string formattedTotalTime = "00:00:00";

	public Stats()
	{
		RegisterPanel?.Invoke(this);
	}

	private readonly float[] fpsBuffer = new float[100];
	private int bufferIndex = 0;

	private void UpdateFpsBuffer(int fps)
	{
		fpsBuffer[bufferIndex] = fps;
		bufferIndex = (bufferIndex + 1) % fpsBuffer.Length;
	}

	public void Draw(Renderer renderer)
	{
		if (Time.TotalTime - lastUpdateTime >= 1.0)
		{
			TimeSpan timeSpan = TimeSpan.FromSeconds(Time.TotalTime);
			formattedTotalTime = timeSpan.ToString(@"hh\:mm\:ss");
			UpdateFpsBuffer(Time.FPS);
			lastUpdateTime = Time.TotalTime;
		}

		ImGui.Begin(PanelName);
		ImGui.Text($"Uptime: {formattedTotalTime}");
		ImGui.Text($"DeltaTime: {Time.DeltaTime * 1000:F2}ms");
		ImGui.Text($"FPS: {Time.FPS}");
		ImGui.Separator();
		ImGui.Text($"Draw Calls: {renderer.DrawCalls}");
		ImGui.Text($"Materials: {renderer.MaterialsUsed}");
		ImGui.Text($"Shaders: {renderer.ShadersUsed}");
		ImGui.Separator();
		ImGui.Text($"Window Size: {renderer.WindowSize.X} x {renderer.WindowSize.Y}");
		ImGui.Text($"Viewport Size: {(int)renderer.ViewportSize.X} x {(int)renderer.ViewportSize.Y}");

		ImGui.PlotLines(string.Empty, ref fpsBuffer[0], fpsBuffer.Length, 0, null, 0, fpsBuffer.Max(),
			new System.Numerics.Vector2(-1, 80));

		ImGui.End();
	}
}