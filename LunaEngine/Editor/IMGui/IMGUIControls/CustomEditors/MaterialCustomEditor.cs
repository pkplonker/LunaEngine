using Editor.IMGUIControls;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.Material))]
public class MaterialCustomEditor : BaseCustomEditor
{
	private static IPropertyDrawer? propertyDrawer;

	public override void Draw(object component, object owningObject, IMemberAdapter memberInfoToSetObjectOnOwner,
		IRenderer renderer, int depth = 0)
	{
		propertyDrawer ??= new PropertyDrawer(renderer);
		if (component == owningObject)
		{
			Logger.Error("er");
		}

		if (component != null)
		{
			propertyDrawer.CreateNestedHeader(depth,
				CustomEditorBase.GenerateName<Silk.NET.Assimp.Material>(memberInfoToSetObjectOnOwner),
				() => DrawInternal(component as Material, depth, memberInfoToSetObjectOnOwner, owningObject),
				() => DropTarget<Material>(owningObject, memberInfoToSetObjectOnOwner));
		}
		else
		{
			propertyDrawer.CreateNestedHeader(depth,
				CustomEditorBase.GenerateName<Silk.NET.Assimp.Material>(memberInfoToSetObjectOnOwner),
				() => DrawEmpty(),
				() => DropTarget<Material>(owningObject, memberInfoToSetObjectOnOwner));
			
		}
	}

	private static void DrawInternal(Material mat, int depth, IMemberAdapter memberInfoToSetObjectOnOwner,
		object owningObject)
	{
		try
		{
			if (ResourceManager.Instance.GetMetadata(mat.GUID, out var metadata))
			{
				ImGui.Text($"Material Path : {metadata.Path}");
			}
		}
		catch (Exception e)
		{
			Logger.Warning($"Failed to convert guid for UI {e}");
		}

		if (ImGui.Button("Remove"))
		{
			memberInfoToSetObjectOnOwner.SetValue(owningObject, Guid.Empty);
		}

		propertyDrawer.ProcessProps(mat, ++depth);
	}

	private void DrawEmpty()
	{
		//if (ImGui.Button("Select Material")) { }
	}
}