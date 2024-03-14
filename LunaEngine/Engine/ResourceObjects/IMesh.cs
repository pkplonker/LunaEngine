namespace Engine;

public interface IMesh : IResource
{
	unsafe void SetupMesh();
	void Render(Renderer renderer, RenderPassData data);
}