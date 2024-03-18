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
	private static IPropertyDrawInterceptStrategy? interceptStrategy;

	public override void Draw(object component, IMemberAdapter? memberInfo, object propertyValue, IRenderer renderer, int depth)
	{
		TextureCustomEditor.propertyDrawer ??= new PropertyDrawer(renderer);
		TextureCustomEditor.interceptStrategy ??= new TexturePropertyDrawIntercept(component, memberInfo);
		if (propertyValue != null)
		{
			propertyDrawer.DrawObject(propertyValue, depth, interceptStrategy,
				CustomEditorBase.GenerateName<Texture>(memberInfo), () => DropTarget<Texture>(component, memberInfo));
		}
		else
		{
			interceptStrategy.DrawEmpty(++depth, CustomEditorBase.GenerateName<Texture>(memberInfo), propertyDrawer,
				memberInfo, component,() => DropTarget<Texture>(component, memberInfo));
		}
	}
}

public class TexturePropertyDrawIntercept : IPropertyDrawInterceptStrategy
{
	Vector2 imageSize = new(175, 175);
	private object owner;
	private IMemberAdapter ownerMemberInfo;

	public TexturePropertyDrawIntercept(object component, IMemberAdapter? memberInfo)
	{
		this.owner = component;
		this.ownerMemberInfo = memberInfo;
	}

	public bool Draw(object component, IMemberAdapter memberInfo, IRenderer renderer)
	{
		if (component is Texture tex)
		{
			if (memberInfo.Name == "textureHandle")
			{
				uint textureId = (uint) memberInfo.GetValue(component);
				if (textureId == 0)
				{
					return true;
				}

				IntPtr texturePtr = new IntPtr(textureId);

				DrawTexture(ownerMemberInfo, owner, texturePtr);

				ImGui.Text($"{tex.Width} x {tex.Height}");

				return true;
			}

			if (memberInfo.Name == nameof(Texture.Width) || memberInfo.Name == nameof(Texture.Height))
			{
				return true;
			}

			if (memberInfo.Name == nameof(Texture.Path))
			{
				ImGui.Text($"{memberInfo.Name} : {((string) memberInfo?.GetValue(component)).MakeProjectRelative()}");
				return true;
			}
		}

		return false;
	}

	private void DrawTexture(IMemberAdapter? memberInfo, object component, IntPtr texturePtr)
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