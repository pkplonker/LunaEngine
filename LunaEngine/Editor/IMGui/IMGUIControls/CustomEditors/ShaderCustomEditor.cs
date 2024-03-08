using System.Linq.Expressions;
using System.Numerics;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;
using Silk.NET.OpenGL;
using Shader = Engine.Shader;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.Shader))]
public class ShaderCustomEditor : ICustomEditor
{
	private static PropertyDrawer? propertyDrawer;
	private static IPropertyDrawInterceptStrategy? interceptStrategy;

	public void Draw(IMemberAdapter? memberInfo, object propertyValue, Renderer renderer, int depth)
	{
		propertyDrawer ??= new PropertyDrawer(renderer);
		interceptStrategy ??= new ShaderPropertyDrawIntercept();
		if (propertyValue != null)
		{
			propertyDrawer.DrawObject(propertyValue, depth, interceptStrategy,
				CustomEditorBase.GenerateName<Shader>(memberInfo));
		}
		else
		{
			interceptStrategy.DrawEmpty(++depth, CustomEditorBase.GenerateName<Shader>(memberInfo),
				propertyDrawer, memberInfo);
		}
	}

	public class ShaderPropertyDrawIntercept : IPropertyDrawInterceptStrategy
	{
		public bool Draw(object component, IMemberAdapter memberInfo, Renderer renderer)
		{
			var memberName = CustomEditorBase.GenerateName<Shader>(memberInfo);
			if (memberInfo.Name == "shaderPath")
			{
				ImGui.Text($"{memberName} : {((string) memberInfo?.GetValue(component)).MakeProjectRelative()}");
				ImGui.SameLine();
				if (ImGui.Button($"Replace##{memberInfo.Name}"))
				{
					ReplaceShader(memberInfo);
				}

				return true;
			}

			return false;
		}

		public void DrawEmptyContent(IMemberAdapter? memberInfo)
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