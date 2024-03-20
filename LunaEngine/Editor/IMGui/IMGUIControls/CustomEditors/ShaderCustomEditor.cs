using System.Linq.Expressions;
using System.Numerics;
using Editor.IMGUIControls;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;
using Silk.NET.OpenGL;
using Shader = Engine.Shader;
using Texture = Engine.Texture;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.Shader))]
public class ShaderCustomEditor : BaseCustomEditor
{
	private static PropertyDrawer? propertyDrawer;

	public override void Draw(object component, object owningObject, IMemberAdapter memberInfoToSetObjectOnOwner,
		IRenderer renderer, int depth = 0)
	{
		if (component == owningObject)
		{
			Logger.Error("er");
		}

		propertyDrawer ??= new PropertyDrawer(renderer);
		if (component != null)
		{
			propertyDrawer.CreateNestedHeader(depth,
				CustomEditorBase.GenerateName<Shader>(memberInfoToSetObjectOnOwner),
				() => DrawInternal(component as Shader, owningObject, memberInfoToSetObjectOnOwner),
				() => DropTarget<Shader>(owningObject, memberInfoToSetObjectOnOwner));
		}
		else DrawEmpty();
	}

	private void DrawInternal(Shader? component, object owningObject, IMemberAdapter memberInfoToSetObjectOnOwner)
	{
		if (!string.IsNullOrEmpty(component.ShaderPath))
		{
			ImGui.Text(
				$"{nameof(component.ShaderPath).GetFormattedName()} : {component.ShaderPath.MakeProjectRelative()}");
			if (ImGui.Button("Edit"))
			{
				FileDialog.OpenFileWithDefaultApp(component.ShaderPath.MakeAbsolute());
			}

			ImGui.SameLine();

			if (ImGui.Button("Remove"))
			{
				memberInfoToSetObjectOnOwner.SetValue(owningObject,Guid.Empty);
			}

			ImGui.SameLine();
		}

		if (ImGui.Button("Reload"))
		{
			ResourceManager.Instance.ReleaseResource<Shader>(component.GUID);
		}
	}

	private void DrawEmpty()
	{
		//ImGui.Text("Empty Shader");
	}
}