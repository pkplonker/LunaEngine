using System.Numerics;

namespace Engine;

public interface IMaterial : IResource
{
	void SetShader(Guid shader);
	void Use(Renderer renderer, RenderPassData data, Matrix4x4 modelMatrix);
	public Guid ShaderGUID { get; set; }
}