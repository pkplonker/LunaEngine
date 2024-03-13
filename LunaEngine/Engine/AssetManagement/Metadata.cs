using Engine.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Engine;

public enum MetadataType
{
	Texture,
	Material,
	Shader,
	Mesh
}

public abstract class Metadata
{
	public Metadata() { }

	public static string MetadataFileExtension { get; } = "metadata";
	public string Path { get; set; } = string.Empty;
	public Guid GUID { get; set; } = Guid.NewGuid();
	public int MetaDataVersion { get; set; } = 1;
	public MetadataType MetadataType { get; set; }

	public static Metadata Create(Type? type, string path)
	{
		ArgumentNullException.ThrowIfNull(type);

		if (type == typeof(Texture))
		{
			return new TextureMetadata {Path = path, GUID = Guid.NewGuid()};
		}

		if (type == typeof(Material))
		{
			var metadata = new MaterialMetadata {Path = path, GUID = Guid.NewGuid()};
			try
			{
				var material = (Material) ObjectSerializer.Deserialize(path.MakeProjectAbsolute());
				metadata.GUID = material.GUID;
			}
			catch (Exception e)
			{
				Logger.Warning($"Failed to deserialize material to extract guid: {e}");
			}

			return metadata;
		}

		if (type == typeof(Shader))
		{
			return new ShaderMetadata {Path = path, GUID = Guid.NewGuid()};
		}

		if (type == typeof(Mesh))
		{
			return new MeshMetadata {Path = path, GUID = Guid.NewGuid()};
		}

		throw new ArgumentException("Unsupported type for metadata creation", nameof(type));
	}

	public override bool Equals(object obj) => obj is Metadata other && this.GUID.Equals(other.GUID);

	public override int GetHashCode() => GUID.GetHashCode();

	public virtual void Serialize()
	{
		try
		{
			if (string.IsNullOrWhiteSpace(Path))
			{
				throw new InvalidOperationException("Path is not set for the metadata object.");
			}

			File.WriteAllText(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Path),
					$"{GUID.ToString()}.{MetadataFileExtension}").MakeProjectAbsolute(),
				JsonConvert.SerializeObject(this, Formatting.Indented));
		}
		catch (Exception ex)
		{
			Logger.Error($"Error during serialization: {ex.Message}");
		}
	}

	public static Metadata? CreateMetadataFromMetadataFile(string path)
	{
		if (string.IsNullOrEmpty(path) || !File.Exists(path))
		{
			return null;
		}

		var json = File.ReadAllText(path);
		var jObject = JObject.Parse(json);

		// Assuming "Type" is the enum property name in JSON
		if (!Enum.TryParse<MetadataType>(jObject["MetadataType"]?.ToString(), out var metadataType))
		{
			return null; // Or handle unknown/invalid type
		}

		switch (metadataType)
		{
			case MetadataType.Texture:
				return JsonConvert.DeserializeObject<TextureMetadata>(json);
			case MetadataType.Material:
				return JsonConvert.DeserializeObject<MaterialMetadata>(json);
			case MetadataType.Shader:
				return JsonConvert.DeserializeObject<ShaderMetadata>(json);
			case MetadataType.Mesh:
				return JsonConvert.DeserializeObject<MeshMetadata>(json);
			default:
				return null; // Or handle unknown type
		}
	}
}

public class TextureMetadata : Metadata
{
	public TextureMetadata() : base()
	{
		MetadataType = MetadataType.Texture;
	}
}

public class MaterialMetadata : Metadata
{
	public MaterialMetadata() : base()
	{
		MetadataType = MetadataType.Material;
	}
}

public class ShaderMetadata : Metadata
{
	public ShaderMetadata() : base()
	{
		MetadataType = MetadataType.Shader;
	}
}

public class MeshMetadata : Metadata
{
	public MeshMetadata() : base()
	{
		MetadataType = MetadataType.Mesh;
	}
}