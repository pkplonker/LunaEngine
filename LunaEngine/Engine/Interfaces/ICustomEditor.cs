using System.Numerics;
using Editor.Properties;
using ImGuiNET;
using Silk.NET.OpenGL;

namespace Engine;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public class CustomEditorAttribute : Attribute
{
	public Type TargetType { get; }

	public CustomEditorAttribute(Type editorType)
	{
		TargetType = editorType;
	}
}

public interface ICustomEditor
{
	void Draw(object component, IMemberAdapter? memberInfo, object propertyValue, Renderer renderer, int depth);
}