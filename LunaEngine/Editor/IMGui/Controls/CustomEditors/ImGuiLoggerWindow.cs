using System.Collections.Concurrent;
using System.Numerics;
using Editor.Controls;
using Engine;
using Engine.Logging;
using ImGuiNET;

namespace Editor;

public class ImGuiLoggerWindow : ILogSink, IPanel
{
	private static ConcurrentQueue<LogMessage> logQueue = new();
	private const int MaxLogSize = 3000;
	private bool first;
	public string PanelName { get; set; } = "Console";

	private int scrollWidth = 30;
	private int lineHeight = 20;
	private float val = 0.0f;
	private float maxSlider = 1.0f;

	private Dictionary<LogLevel, Vector4> logLevelColors = new()
	{
		{LogLevel.Debug, new Vector4(0.5f, 0.5f, 0.5f, 1.0f)},
		{LogLevel.Info, new Vector4(1.0f, 1.0f, 1.0f, 1.0f)},
		{LogLevel.Warning, new Vector4(1.0f, 1.0f, 0.0f, 1.0f)},
		{LogLevel.Error, new Vector4(1.0f, 0.0f, 0.0f, 1.0f)},
	};

	public ImGuiLoggerWindow()
	{
		Debug.AddSink(this);
	}

	public void Emit(LogMessage message)
	{
		while (logQueue.Count >= MaxLogSize)
		{
			logQueue.TryDequeue(out var _);
		}

		logQueue.Enqueue(message);
	}

	public void Draw(Renderer renderer)
	{
		ImGui.Begin(PanelName, ImGuiWindowFlags.NoScrollbar);
		if (ImGui.Button("Scroll to bottom"))
		{
			val = 0;
		}

		ImGui.SameLine();
		if (ImGui.Button("GenerateTestMessages"))
		{
			for (var i = 0; i < 5; i++)
			{
				Debug.Log("l");
				Debug.Info("i");
				Debug.Warning("w");
				Debug.Error("e");
			}
		}
		ImGui.SameLine();
		ImGui.Text($"Count: {logQueue.Count}");
		
		
		ImGui.BeginChild("LogScrollingRegion", new Vector2(-scrollWidth, -ImGui.GetFrameHeightWithSpacing()), false,
			ImGuiWindowFlags.NoScrollbar);
		var height = GetLogMessageHeight("Test");
		var availableHeight = ImGui.GetContentRegionAvail().Y;
		var elementsToShow = (int) (availableHeight / height);
		var maxStartIndex = Math.Max(logQueue.Count - elementsToShow, 0);

		var startIndex = maxStartIndex - (int) (val * maxStartIndex);
		startIndex = Math.Max(startIndex, 0);

		var endIndex = Math.Min(startIndex + elementsToShow, logQueue.Count - 1);

		LogMessage[] logArray = logQueue.Skip(startIndex).Take(endIndex - startIndex).ToArray();
		for (var i = 0; i < logArray.Length; i++)
		{
			DrawLog(logArray[i]);
		}

		ImGui.EndChild();

		ImGui.SameLine();

		ImGui.VSliderFloat("##v", new Vector2(scrollWidth, ImGui.GetWindowHeight() - ImGui.GetFrameHeightWithSpacing()),
			ref val, 0.0f, maxSlider, "");

		ImGui.End();
	}

	private void DrawLog(LogMessage log)
	{
		Vector4 color;
		if (!logLevelColors.TryGetValue(log.Level, out color))
		{
			color = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
		}

		ImGui.PushStyleColor(ImGuiCol.Text, color);
		ImGui.TextUnformatted($"[{log.Timestamp.ToShortTimeString()}] {log.Message}");
		ImGui.PopStyleColor();
	}

	private float GetLogMessageHeight(string log)
	{
		string message = log;
		float wrapWidth = ImGui.GetWindowWidth();
		Vector2 textSize = ImGui.CalcTextSize(message, wrapWidth);
		return textSize.Y;
	}
}