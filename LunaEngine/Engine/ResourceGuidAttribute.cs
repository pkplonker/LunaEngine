namespace Engine;

public class ResourceGuidAttribute : Attribute
{
	public readonly Type Type;

	public ResourceGuidAttribute(Type type)
	{
		this.Type = type;
	}
}