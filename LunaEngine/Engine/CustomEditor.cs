using System.Numerics;
using ImGuiNET;
using Silk.NET.OpenGL;

namespace Engine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class | AttributeTargets.Struct)]
public class CustomEditorAttribute : Attribute
{
	public Type EditorType { get; }
	public bool ShowName { get; set; }

	public CustomEditorAttribute(Type editorType, bool showName = true, params object[] args)
	{
		this.ShowName = showName;
		if (!typeof(ICustomEditor).IsAssignableFrom(editorType))
		{
			throw new ArgumentException("Editor type must implement iCustomEditor", nameof(editorType));
		}

		EditorType = editorType;
	}
}

public interface ICustomEditor
{
	void Draw(Renderer renderer);
	bool ShowName() => true;
}