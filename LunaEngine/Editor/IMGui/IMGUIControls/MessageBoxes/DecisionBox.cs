namespace Editor.Controls;

using ImGuiNET;
using System;

public class DecisionBox
{
	private static bool isVisible = false;
	private static string message = string.Empty;
	private static Action onConfirm = () => { };
	private static Action onCancel = () => { };

	public static void Show(string message, Action onConfirm, Action onCancel)
	{
		DecisionBox.message = message;
		DecisionBox.onConfirm = onConfirm;
		DecisionBox.onCancel = onCancel;
		isVisible = true;
	}
	public static void Show(string message, Action onConfirm)
	{
		DecisionBox.message = message;
		DecisionBox.onConfirm = onConfirm;
		isVisible = true;
	}
	public static void Show(string message)
	{
		DecisionBox.message = message;
		isVisible = true;
	}

	public static void Render()
	{
		if (!isVisible) return;

		if (ImGui.BeginPopupModal("Confirmation", ref isVisible, ImGuiWindowFlags.AlwaysAutoResize))
		{
			ImGui.Text(message);

			if (ImGui.Button("Yes"))
			{
				onConfirm?.Invoke();
				isVisible = false;
				ImGui.CloseCurrentPopup();
			}

			ImGui.SameLine();

			if (ImGui.Button("No"))
			{
				onCancel?.Invoke();
				isVisible = false;
				Close();
			}

			ImGui.EndPopup();
		}

		if (isVisible)
		{
			ImGui.OpenPopup("Confirmation");
		}
	}

	private static void Close()
	{
		ImGui.CloseCurrentPopup();
		onConfirm = () => { };
		onCancel = () => { };
	}
}