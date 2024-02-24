using System.Numerics;
using Silk.NET.OpenGL;

namespace Engine;

public interface IRenderable
{
	public void Render(GL gl, RenderPassData data);
}