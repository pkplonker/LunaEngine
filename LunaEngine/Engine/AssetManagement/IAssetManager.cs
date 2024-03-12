﻿namespace Engine;

public interface IAssetManager {void LoadMetadata();
	IEnumerable<string> GetFilesFromFolder(string path, IEnumerable<string> ext = null);
	bool TryGetResourceByGuid<T>(Guid guid, out T? result) where T : class;
	bool AddMetaData(Metadata metadata);
	IEnumerable<Metadata> GetMetadata(MetadataType? filterType = null);
	bool MetadataExistsWithPath(string path);
	void ClearMetadatas(); }