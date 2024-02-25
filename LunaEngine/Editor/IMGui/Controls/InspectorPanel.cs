using System.Numerics;
using Engine;
using ImGuiNET;

namespace Editor.Controls;

public class InspectorPanel : IPanel
{
	private readonly EditorImGuiController controller;
	public static event Action<IPanel> RegisterPanel;

	public InspectorPanel(EditorImGuiController controller)
	{
		RegisterPanel?.Invoke(this);
		this.controller = controller;
	}

	public string PanelName { get; set; } = "Inspector";

	public void Draw(Renderer renderer)
	{
		ImGui.Begin(PanelName);
		if (controller.SelectedGameObject != null)
		{
			var go = controller.SelectedGameObject;
			ImGui.Text(go.Name);
			ImGui.Text(go.Guid.ToString());
			DrawTransform(go.Transform);
		}

		ImGui.End();
	}

	public static void DrawTransform(Transform trans)
	{
		UndoableDrag(
			() => trans.Position.X,
			value => trans.Position = new Vector3(value, trans.Position.Y, trans.Position.Z),
			$"##PosX{trans.GetHashCode()}",
			"Change Position X");

		UndoableDrag(
			() => trans.Position.Y,
			value => trans.Position = new Vector3(trans.Position.X, value, trans.Position.Z),
			$"##PosY{trans.GetHashCode()}",
			"Change Position Y");
		UndoableDrag(
			() => trans.Position.Z,
			value => trans.Position = new Vector3(trans.Position.X, value, trans.Position.Z),
			$"##PosZ{trans.GetHashCode()}",
			"Change Position Z");
		// UndoableDrag(
		// 	() => trans.Rotation.X,
		// 	value => trans.Rotation = new Vector3(value, trans.Rotation.Y, trans.Rotation.Z),
		// 	$"##RotX{trans.GetHashCode()}",
		// 	"Change Rotation X");
		//
		// UndoableDrag(
		// 	() => trans.Rotation.Y,
		// 	value => trans.Rotation = new Vector3(trans.Rotation.X, value, trans.Rotation.Z),
		// 	$"##RotY{trans.GetHashCode()}",
		// 	"Change Rotation Y");
		// UndoableDrag(
		// 	() => trans.Rotation.Z,
		// 	value => trans.Rotation = new Vector3(trans.Rotation.X, value, trans.Rotation.Z),
		// 	$"##RotZ{trans.GetHashCode()}",
		// 	"Change Rotation Z");
		UndoableDrag(
			() => trans.Scale.X,
			value => trans.Scale = new Vector3(value, trans.Scale.Y, trans.Scale.Z),
			$"##ScaleX{trans.GetHashCode()}",
			"Change Scale X");

		UndoableDrag(
			() => trans.Scale.Y,
			value => trans.Scale = new Vector3(trans.Scale.X, value, trans.Scale.Z),
			$"##ScaleY{trans.GetHashCode()}",
			"Change Scale Y");
		UndoableDrag(
			() => trans.Scale.Z,
			value => trans.Scale = new Vector3(trans.Scale.X, trans.Scale.Y, value),
			$"##ScaleZ{trans.GetHashCode()}",
			"Change Scale Z");
	}
	
	

	private static Dictionary<string, float> sliderStates = new Dictionary<string, float>();

	// public static bool DrawVector(string vectorName, ref Vector3 vector, int hashCode)
	// {
	// 	bool dirty = false;
	// 	float labelWidth = ImGui.CalcTextSize("Rotation").X + 20.0f;
	// 	float totalWidth = ImGui.GetContentRegionAvail().X - labelWidth;
	// 	float fieldWidth = totalWidth / 3.0f - ImGui.GetStyle().ItemInnerSpacing.X;
	//
	// 	ImGui.Text(vectorName);
	// 	ImGui.SameLine(labelWidth);
	//
	// 	ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(1, 0, 0, 1));
	// 	ImGui.Text("X");
	// 	ImGui.SameLine();
	// 	ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("X").X);
	// 	Vector3 localVector = vector;
	//
	// 	UndoableDrag(
	// 		() => localVector.X,
	// 		value =>
	// 		{
	// 			localVector.X = value;
	// 			dirty = true;
	// 		},
	// 		$"##{vectorName}x{hashCode}",
	// 		"Change X Value");
	// 	ImGui.PopStyleColor();
	// 	ImGui.SameLine();
	//
	// 	ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 1, 0, 1));
	// 	ImGui.Text("Y");
	// 	ImGui.SameLine();
	// 	ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Y").X);
	// 	UndoableDrag(
	// 		() => localVector.Y,
	// 		value =>
	// 		{
	// 			localVector.Y = value;
	// 			dirty = true;
	// 		},
	// 		$"##{vectorName}Y{hashCode}",
	// 		"Change Y Value");
	//
	// 	ImGui.PopStyleColor();
	// 	ImGui.SameLine();
	//
	// 	ImGui.PushStyleColor(ImGuiCol.FrameBg, new Vector4(0, 0, 1, 1));
	// 	ImGui.Text("Z");
	// 	ImGui.SameLine();
	// 	ImGui.SetNextItemWidth(fieldWidth - ImGui.CalcTextSize("Z").X);
	// 	UndoableDrag(
	// 		() => localVector.Z,
	// 		value =>
	// 		{
	// 			localVector.Z = value;
	// 			dirty = true;
	// 		},
	// 		$"##{vectorName}z{hashCode}",
	// 		"Change Z Value");
	// 	ImGui.PopStyleColor();
	//
	// 	ImGui.PopStyleColor();
	//
	// 	if (dirty)
	// 	{
	// 		vector = localVector;
	// 	}
	//
	// 	return dirty;
	// }

	public static void UndoableDrag(
		Func<float> getValue,
		Action<float> setValue,
		string label,
		string actionDescription,
		float min = float.MinValue,
		float max = float.MaxValue,
		float speed = 1.0f)
	{
		float value = getValue();

		bool valueChanged = ImGui.DragFloat(label, ref value, speed, min, max);
		if (ImGui.IsItemActivated())
		{
			sliderStates[label] = value;
		}

		if (valueChanged)
		{
			setValue(value);
		}

		if (ImGui.IsItemDeactivatedAfterEdit())
		{
			float originalVal = sliderStates.ContainsKey(label) ? sliderStates[label] : value;

			if (value != originalVal)
			{
				UndoManager.RecordAndPerform(new Memento(
					() => setValue(value),
					() => setValue(originalVal),
					actionDescription));
			}
		}
	}
}