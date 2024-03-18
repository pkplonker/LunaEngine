using System.Runtime.InteropServices;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;

namespace Editor.IMGUIControls;

public abstract class BaseCustomEditor : ICustomEditor
{
	public abstract void Draw(object component, IMemberAdapter? memberInfo, object propertyValue, IRenderer renderer,
		int depth);

	protected unsafe void DropTarget<T>(object component, IMemberAdapter? memberInfo) where T : class, IResource
	{
		if (component == null || memberInfo == null) return;
		if (ImGui.BeginDragDropTarget())
		{
			Logger.Info($"hit {component}");
			var payload = ImGui.AcceptDragDropPayload("Metadata");
			if (payload.NativePtr != (void*) IntPtr.Zero)
			{
				var guidPtr = ImGui.AcceptDragDropPayload("Metadata").Data;
				Guid guid = Marshal.PtrToStructure<Guid>(guidPtr);
				if (ResourceManager.Instance.GuidIsType<T>(guid))
				{
					memberInfo.SetValue(component, guid);
				}
			}

			ImGui.EndDragDropTarget();
		}
	}
}