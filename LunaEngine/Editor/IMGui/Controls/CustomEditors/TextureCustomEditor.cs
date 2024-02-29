using System.Linq.Expressions;
using System.Numerics;
using Editor.Properties;
using Engine;
using ImGuiNET;
using Silk.NET.OpenGL;
using Texture = Engine.Texture;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.Texture))]
public class TextureCustomEditor : ICustomEditor
{
	private static PropertyDrawer? propertyDrawer;
	private static TexturePropertyDrawIntercept? interceptStrategy;

	public void Draw(IMemberAdapter? memberInfo, object propertyValue, Renderer renderer, int depth)
	{
		propertyDrawer ??= new PropertyDrawer(renderer);
		interceptStrategy ??= new TexturePropertyDrawIntercept();
		var memberType = memberInfo.MemberType;
		var upperName = CamelCaseRenamer.GetFormattedName(memberInfo.Name);
		var shownName = upperName == memberType.Name ? upperName : $"{memberType.Name} - {upperName}";
		propertyDrawer.DrawObject(propertyValue, depth, interceptStrategy, shownName);
	}
}

public class TexturePropertyDrawIntercept : IPropertyDrawInterceptStrategy
{
	public bool Draw(object component, IMemberAdapter member, Renderer renderer)
	{
		if (component is Texture tex)
		{
			if (member.Name == "textureHandle")
			{
				uint textureId = (uint) member.GetValue(component);
				if (textureId == 0)
				{
					return true;
				}

				IntPtr texturePtr = new IntPtr(textureId);

				renderer.Gl.BindTexture(TextureTarget.Texture2D, textureId);
				var size = ImGui.GetWindowSize();
				var floatSize = Math.Min(size.X, size.Y) / 2;
				size = new Vector2(floatSize, floatSize);
				ImGui.Image(texturePtr, size);
				renderer.Gl.BindTexture(TextureTarget.Texture2D, 0);
				ImGui.Text($"{tex.Width} x {tex.Height}");

				return true;
			}

			if (member.Name == nameof(Texture.Width) || member.Name == nameof(Texture.Height))
			{
				return true;
			}

			if (member.Name == nameof(Texture.Path))
			{
				ImGui.Text($"{member.Name} : {(string) member?.GetValue(component)}");
				return true;
			}
		}

		return false;
	}
}