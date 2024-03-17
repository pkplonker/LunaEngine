using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.InteropServices;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;
using Silk.NET.Core;
using Silk.NET.OpenGL;
using StbImageSharp;
using Texture = Engine.Texture;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.Texture))]
public class TextureCustomEditor : ICustomEditor
{
	private static PropertyDrawer? propertyDrawer;
	private static IPropertyDrawInterceptStrategy? interceptStrategy;

	public void Draw(object component, IMemberAdapter? memberInfo, object propertyValue, IRenderer renderer, int depth)
	{
		TextureCustomEditor.propertyDrawer ??= new PropertyDrawer(renderer);
		TextureCustomEditor.interceptStrategy ??= new TexturePropertyDrawIntercept();
		if (propertyValue != null)
		{
			TextureCustomEditor.propertyDrawer.DrawObject(propertyValue, depth, TextureCustomEditor.interceptStrategy,
				CustomEditorBase.GenerateName<Texture>(memberInfo));
		}
		else
		{
			interceptStrategy.DrawEmpty(++depth, CustomEditorBase.GenerateName<Texture>(memberInfo), propertyDrawer,
				memberInfo, component);
		}
	}
}

public class TexturePropertyDrawIntercept : IPropertyDrawInterceptStrategy
{
	Vector2 imageSize = new(175, 175);

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

				ImGui.Image(texturePtr, imageSize);

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

	public void DrawEmptyContent(IMemberAdapter? memberInfo, object component)
	{
		if (ImGui.ImageButton("Select Texture",
			    IconLoader.LoadIcon(@"resources/icons/AddTexture.png".MakeAbsolute()),
			    imageSize))
		{
			Logger.Info("Pressed select texture");
		}

		if (ImGui.BeginDragDropTarget())
		{
			unsafe
			{
				var payload = ImGui.AcceptDragDropPayload("Metadata");
				if (payload.NativePtr != (void*) IntPtr.Zero)
				{
					var guidPtr = ImGui.AcceptDragDropPayload("Metadata").Data;
					Guid guid = Marshal.PtrToStructure<Guid>(guidPtr);
					if(ResourceManager.Instance.GuidIsType<Texture>(guid))
					{
						memberInfo?.SetValue(component,guid);
					}
				}
			}

			ImGui.EndDragDropTarget();
		}
	}
}