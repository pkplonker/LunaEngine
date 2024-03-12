using Silk.NET.OpenGL;

namespace Engine;

public class ResourceManager : IAssetManager
{
	private static readonly Lazy<ResourceManager> instance = new(() => new ResourceManager());
	private IAssetManager assetManager;

	public void Init(GL gl)
	{
		assetManager = new UserResourceManager(gl);
	}

	public static ResourceManager Instance => instance.Value;

	public void LoadMetadata() => assetManager?.LoadMetadata();

	public IEnumerable<string> GetFilesFromFolder(string path, IEnumerable<string> ext = null) =>
		assetManager.GetFilesFromFolder(path, ext);

	public bool TryGetResourceByGuid<T>(Guid guid, out T? result) where T : class
	{
		T? r = null;
		var res = assetManager?.TryGetResourceByGuid<T>(guid, out r) ?? false;
		result = r ?? null;
		return res;
	}

	public bool AddMetaData(Metadata metadata) => assetManager?.AddMetaData(metadata) ?? false;

	public IEnumerable<Metadata> GetMetadata(MetadataType? filterType = null) =>
		assetManager?.GetMetadata(filterType) ?? Enumerable.Empty<Metadata>();

	public bool MetadataExistsWithPath(string path) => assetManager.MetadataExistsWithPath(path);

	public void ClearMetadatas() => assetManager?.ClearMetadatas();
}