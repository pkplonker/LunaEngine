using System.Linq.Expressions;
using System.Numerics;
using Editor.Properties;
using Engine;
using ImGuiNET;
using Silk.NET.OpenGL;
using Texture = Engine.Texture;

namespace Editor.Controls;

[CustomEditor(typeof(Engine.Material))]
public class MaterialCustomEditor : ICustomEditor
{
	private static PropertyDrawer? propertyDrawer;
	private static IPropertyDrawInterceptStrategy? interceptStrategy;

	public void Draw(IMemberAdapter? memberInfo, object propertyValue, Renderer renderer, int depth)
	{
		propertyDrawer ??= new PropertyDrawer(renderer);
		interceptStrategy ??= new MaterialPropertyDrawIntercept();
		var memberType = memberInfo.MemberType;
		var upperName = CamelCaseRenamer.GetFormattedName(memberInfo.Name);
		var typeName = typeof(Material).Name;
		var shownName = upperName == typeName ? upperName : $"{typeName} - {upperName}";
		propertyDrawer.DrawObject(propertyValue, depth, interceptStrategy, shownName);
	}
}

public class MaterialPropertyDrawIntercept : IPropertyDrawInterceptStrategy
{
	public bool Draw(object component, IMemberAdapter member, Renderer renderer)
	{
		return false;
	// 	if (component is Texture tex)
	// 	{
	// 		if (member.Name == "textureHandle")
	// 		{
	// 			uint textureId = (uint) member.GetValue(component);
	// 			if (textureId == 0)
	// 			{
	// 				return true;
	// 			}
	//
	// 			IntPtr texturePtr = new IntPtr(textureId);
	//
	// 			renderer.Gl.BindTexture(TextureTarget.Texture2D, textureId);
	// 			var size = ImGui.GetWindowSize();
	// 			var floatSize = Math.Min(size.X, size.Y) / 2;
	// 			size = new Vector2(floatSize, floatSize);
	// 			ImGui.Image(texturePtr, size);
	// 			renderer.Gl.BindTexture(TextureTarget.Texture2D, 0);
	// 			ImGui.Text($"{tex.Width} x {tex.Height}");
	//
	// 			return true;
	// 		}
	//
	// 		if (member.Name == nameof(Texture.Width) || member.Name == nameof(Texture.Height))
	// 		{
	// 			return true;
	// 		}
	//
	// 		if (member.Name == nameof(Texture.Path))
	// 		{
	// 			ImGui.Text($"{member.Name} : {(string) member?.GetValue(component)}");
	// 			return true;
	// 		}
	// 		ImGui.Text("BLASfojhdsjknfljdshnfsnjdf");
	// 	}
	//
	// 	return false;
	 }
}