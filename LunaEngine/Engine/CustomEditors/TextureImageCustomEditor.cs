using System.Numerics;
using System.Reflection;
using Engine;
using ImGuiNET;
using Silk.NET.OpenGL;

namespace Editor.Controls;

public class TextureImageCustomEditor : ICustomEditor
{
	private uint textureId;
	private Vector2 size;

	public TextureImageCustomEditor(uint textureId, int sizeX, int sizeY)
	{
		this.textureId = textureId;
		this.size = new Vector2(sizeX, sizeY);
	}

	public void Draw(Renderer renderer)
	{
		if (textureId == 0)
		{
			return;
		}

		IntPtr texturePtr = new IntPtr(textureId);

		renderer.Gl.BindTexture(TextureTarget.Texture2D, textureId);
		ImGui.Image(texturePtr, size);
		renderer.Gl.BindTexture(TextureTarget.Texture2D, 0);
	}
}

public class TextureCustomEditor : ICustomEditor
{
	private readonly MemberInfo memberInfo;

	public TextureCustomEditor(MemberInfo memberInfo)
	{
		this.memberInfo = memberInfo;
	}

	public void Draw(Renderer renderer)
	{
		ImGui.Text("TESTING");
	}
}