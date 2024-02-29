using System.Numerics;
using System.Reflection;
using Editor.Properties;
using Engine;
using ImGuiNET;
using Silk.NET.OpenGL;

namespace Editor.Controls;


[CustomEditor(typeof(Engine.Texture))]
public class TextureCustomEditor : ICustomEditor
{
	private static PropertyDrawer propertyDrawer = null;

	public void Draw(object propertyValue, Renderer renderer, int depth)
	{
		propertyDrawer ??= new PropertyDrawer(renderer);
		propertyDrawer.DrawObject(propertyValue,depth);
	}
}