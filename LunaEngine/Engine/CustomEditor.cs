using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;

namespace Engine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class CustomEditorAttribute : Attribute
{
	public Type EditorType { get; }
	public bool ShowName { get; set; }

	public CustomEditorAttribute(Type editorType, bool showName = true, params object[] args)
	{
		this.ShowName = showName;
		if (!typeof(iCustomEditor).IsAssignableFrom(editorType))
		{
			throw new ArgumentException("Editor type must implement iCustomEditor", nameof(editorType));
		}

		EditorType = editorType;
	}

}

public interface iCustomEditor
{
	void Draw(Renderer renderer);
	bool ShowName() => true;
}

public class TextureImageCustomEditor : iCustomEditor
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