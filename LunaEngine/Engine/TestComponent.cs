namespace Engine;

public class TestComponent : Component
{
	//visable

	public float PropGetSet { get; set; } = 20;
	public Material Mat { get; set; } = new Material(null);
	[Serializable] public int PropInspectable = 40;
	[Serializable] public bool boollt = true;

	[Serializable] public double FieldInspectable = 60;

	[Serializable(CustomName = "Custom Name")]
	public string oldName = "Steve";

	public string PropGetSetString { get; set; } = null;

	// not visable
	public int prop;
	public int field;
	[Serializable(false)] public int propGetSetNotInspectable { get; set; }

	public TestComponent(GameObject gameObject) : base(gameObject) { }
}