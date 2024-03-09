namespace Engine;

public interface IMesh
{
	Guid GUID { get; set; }
	unsafe void SetupMesh();
	void Render(Renderer renderer, RenderPassData data);
}