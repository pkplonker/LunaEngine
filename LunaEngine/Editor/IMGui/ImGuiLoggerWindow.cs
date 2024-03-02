using System.Collections.Concurrent;
using Editor.Controls;
using Engine;
using Engine.Logging;
using ImGuiNET;

namespace Editor;

public class ImGuiLoggerWindow : ILogSink, IPanel
{
	private static ConcurrentQueue<LogMessage> logQueue = new ConcurrentQueue<LogMessage>();

	public void Emit(LogMessage message)
	{
		logQueue.Enqueue(message);
	}

	public string PanelName { get; set; } = "Console";

	public void Draw(Renderer renderer)
	{
		ImGui.Begin(PanelName);
		ImGui.End();
	}
}