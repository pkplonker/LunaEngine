using System.Linq.Expressions;
using System.Numerics;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;
using Silk.NET.OpenGL;
using Texture = Engine.Texture;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.Texture))]
public class TextureCustomEditor : ICustomEditor
{
	private static PropertyDrawer? propertyDrawer;
	private static IPropertyDrawInterceptStrategy? interceptStrategy;

	public void Draw(IMemberAdapter? memberInfo, object propertyValue, Renderer renderer, int depth)
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
			interceptStrategy.DrawEmpty(++depth, CustomEditorBase.GenerateName<Texture>(memberInfo), propertyDrawer,memberInfo);
		}
	}
}

public class TexturePropertyDrawIntercept : IPropertyDrawInterceptStrategy
{
	Vector2 imageSize = new(175, 175);

	public bool Draw(object component, IMemberAdapter memberInfo, Renderer renderer)
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

				renderer.Gl.BindTexture(TextureTarget.Texture2D, textureId);

				ImGui.Image(texturePtr, imageSize);
				renderer.Gl.BindTexture(TextureTarget.Texture2D, 0);
				ImGui.Text($"{tex.Width} x {tex.Height}");

				return true;
			}

			if (memberInfo.Name == nameof(Texture.Width) || memberInfo.Name == nameof(Texture.Height))
			{
				return true;
			}

			if (memberInfo.Name == nameof(Texture.Path))
			{
				ImGui.Text($"{memberInfo.Name} : {(string) memberInfo?.GetValue(component)}");
				return true;
			}
		}

		return false;
	}

	public void DrawEmptyContent(IMemberAdapter? memberInfo)
	{
		if (ImGui.Button("Select Texture"))
		{
			Logger.Info("Pressed");
		}
	}
}