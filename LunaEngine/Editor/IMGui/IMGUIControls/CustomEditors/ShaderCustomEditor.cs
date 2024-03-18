using System.Linq.Expressions;
using System.Numerics;
using Editor.IMGUIControls;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;
using Silk.NET.OpenGL;
using Shader = Engine.Shader;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.Shader))]
public class ShaderCustomEditor : BaseCustomEditor
{
	private static PropertyDrawer? propertyDrawer;
	private static IPropertyDrawInterceptStrategy? interceptStrategy;

	public override void Draw(object component, IMemberAdapter? memberInfo, object propertyValue, IRenderer renderer, int depth)
	{
		propertyDrawer ??= new PropertyDrawer(renderer);
		interceptStrategy ??= new ShaderPropertyDrawIntercept();
		if (propertyValue != null)
		{
			propertyDrawer.DrawObject(propertyValue, depth, interceptStrategy,
				CustomEditorBase.GenerateName<Shader>(memberInfo), () => DropTarget<Shader>(component, memberInfo));
		}
		else
		{
			interceptStrategy.DrawEmpty(++depth, CustomEditorBase.GenerateName<Shader>(memberInfo),
				propertyDrawer, memberInfo,component);
		}
	}

	public class ShaderPropertyDrawIntercept : IPropertyDrawInterceptStrategy
	{
		public bool Draw(object component, IMemberAdapter memberInfo, IRenderer renderer)
		{
			if (memberInfo.Name == "ShaderPath")
			{
				ImGui.Text($"{memberInfo.Name.GetFormattedName()} : {((string) memberInfo?.GetValue(component)).MakeProjectRelative()}");
				return true;
			}

			return false;
		}

		public void DrawEmptyContent(IMemberAdapter? memberInfo, object component)
		{
			if (ImGui.Button($"Select Shader##{memberInfo.Name}"))
			{
				ReplaceShader(memberInfo);
			}
		}

		private static void ReplaceShader(IMemberAdapter memberInfo)
		{
			Logger.Info("Pressed");
		}
	}
}