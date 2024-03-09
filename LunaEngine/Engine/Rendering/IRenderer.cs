using System.Numerics;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;

namespace Engine;

public interface IRenderer : IRenderStats
{
	GL Gl { get; }

	void AddScene(Scene scene, Vector2D<uint> size, out IRenderTarget renderTarget, bool toFrameBuffer);
	void RenderUpdate();
	void Resize(Vector2D<int> size);
	void Load(IWindow window);
	void SetRenderTargetSize(Scene scene, Vector2D<float> size);
	void Close();
	unsafe void DrawElements(PrimitiveType primativeType, uint indicesLength, DrawElementsType elementsTyp);
	void UseShader(Shader? shader);
	void UseMaterial(Material material, RenderPassData data, Matrix4x4 modelMatrix);
	IRenderTarget? GetSceneRenderTarget(Scene scene);
	void RemoveScene(Scene? oldScene);
	public Vector2D<int> WindowSize { get; set; }
}