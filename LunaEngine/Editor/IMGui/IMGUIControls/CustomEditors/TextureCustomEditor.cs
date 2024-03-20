using System.Numerics;
using System.Runtime.InteropServices;
using Editor.IMGUIControls;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;
using Texture = Engine.Texture;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.Texture))]
public class TextureCustomEditor : BaseCustomEditor
{
	private static PropertyDrawer? propertyDrawer;
	private static Vector2 imageSize = new(175, 175);

	public override void Draw(object component, object owningObject, IMemberAdapter memberInfoToSetObjectOnOwner,
		IRenderer renderer, int depth = 0)
	{
		propertyDrawer ??= new PropertyDrawer(renderer);
		if (component != null)
		{
			propertyDrawer.CreateNestedHeader(depth,
				CustomEditorBase.GenerateName<Texture>(memberInfoToSetObjectOnOwner),
				() => DrawInternal(component as Texture, memberInfoToSetObjectOnOwner, owningObject),
				() => DropTarget<Texture>(owningObject, memberInfoToSetObjectOnOwner));
		}
		else
		{
			propertyDrawer.CreateNestedHeader(depth,
				CustomEditorBase.GenerateName<Texture>(memberInfoToSetObjectOnOwner),
				() => DrawEmptyContent(memberInfoToSetObjectOnOwner, owningObject),
				() => DropTarget<Texture>(owningObject, memberInfoToSetObjectOnOwner));
		}
	}

	private static void DrawInternal(Texture texture, IMemberAdapter memberInfoToSetObjectOnOwner, object owningObject)
	{
		if (ImGui.Button("Reload"))
		{
			ResourceManager.Instance.ReleaseResource<Texture>(texture.GUID);
		}

		var textureId = texture.TextureId;
		if (textureId == 0)
		{
			return;
		}

		IntPtr texturePtr = new IntPtr(textureId);

		DrawTexture(memberInfoToSetObjectOnOwner, owningObject, texturePtr);

		ImGui.Text($"{texture.Width} x {texture.Height}");
	}

	private static void DrawTexture(IMemberAdapter? memberInfo, object component, IntPtr texturePtr)
	{
		if (ImGui.ImageButton("Select Texture", texturePtr, imageSize))
		{
			Logger.Info("Pressed select texture");
		}

		HandleDragDrop(memberInfo, component);
	}

	private static unsafe void HandleDragDrop(IMemberAdapter? memberInfo, object component)
	{
		if (ImGui.BeginDragDropTarget())
		{
			var payload = ImGui.AcceptDragDropPayload("Metadata");
			if (payload.NativePtr != (void*) IntPtr.Zero)
			{
				var guidPtr = ImGui.AcceptDragDropPayload("Metadata").Data;
				Guid guid = Marshal.PtrToStructure<Guid>(guidPtr);
				if (ResourceManager.Instance.GuidIsType<Texture>(guid))
				{
					memberInfo?.SetValue(component, guid);
				}
			}

			ImGui.EndDragDropTarget();
		}
	}

	public void DrawEmptyContent(IMemberAdapter? memberInfo, object component)
	{
		DrawTexture(memberInfo, component, IconLoader.LoadIcon(@"resources/icons/AddTexture.png".MakeAbsolute()));
	}
}