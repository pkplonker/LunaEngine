﻿using Editor.IMGUIControls;
using Editor.Properties;
using Engine;
using Engine.Logging;
using ImGuiNET;
using Shader = Engine.Shader;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.Mesh))]
public class MeshCustomEditor : BaseCustomEditor
{
	public override void Draw(object component, object owningObject, IMemberAdapter memberInfoToSetObjectOnOwner,
		IRenderer renderer, int depth = 0)
	{
		Draw<Mesh>(component, owningObject, memberInfoToSetObjectOnOwner,
			renderer, depth);
	}

	protected override void DropProps(object component, IMemberAdapter memberInfoToSetObjectOnOwner,
		object owningObject,
		int depth = 0)
	{
		if (component is not Mesh mesh) return;

		ImGui.Text(
			$"{nameof(mesh.GUID)}");
	}
}