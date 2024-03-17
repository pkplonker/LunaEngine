using System.Linq.Expressions;
using System.Numerics;
using System.Runtime.InteropServices;
using Editor.IMGUIControls;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;
using Silk.NET.OpenGL;
using Texture = Engine.Texture;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.Material))]
public class MaterialCustomEditor : BaseCustomEditor
{
	private static PropertyDrawer? propertyDrawer;
	private static IPropertyDrawInterceptStrategy? interceptStrategy;

	public override void Draw(object component, IMemberAdapter? memberInfo, object propertyValue, IRenderer renderer,
		int depth)
	{
		propertyDrawer ??= new PropertyDrawer(renderer);
		interceptStrategy ??= new MaterialPropertyDrawIntercept();
		if (propertyValue != null)
		{
			propertyDrawer.DrawObject(propertyValue, depth, interceptStrategy,
				CustomEditorBase.GenerateName<Material>(memberInfo), () => DropTarget<Material>(component, memberInfo));
		}
		else
		{
			interceptStrategy.DrawEmpty(++depth, CustomEditorBase.GenerateName<Material>(memberInfo), propertyDrawer,
				memberInfo, component);
		}
	}
}

public class MaterialPropertyDrawIntercept : IPropertyDrawInterceptStrategy
{
	public bool Draw(object component, IMemberAdapter memberInfo, IRenderer renderer)
	{
		return false;
	}

	public void DrawEmptyContent(IMemberAdapter? memberInfo, object component)
	{
		if (ImGui.Button("Select Material")) { }
	}
}