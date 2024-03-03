namespace Engine;

public class TestComponent : Component
{
	//visable

	public float PropGetSet { get; set; } = 20;
	public Material Mat { get; set; } = new Material(null);
	[Inspectable] public int PropInspectable = 40;
	[Inspectable] public bool boollt = true;
	
	[Inspectable] public double FieldInspectable = 60;
	
	[Inspectable(CustomName = "Custom Name")]
	public string oldName = "Steve";
	
	public string PropGetSetString { get; set; } = null;
	
	// not visable
	public int prop;
	public int field;
	[Inspectable(false)] public int propGetSetNotInspectable { get; set; }

	public TestComponent(GameObject gameObject) : base(gameObject)
	{
	}
}