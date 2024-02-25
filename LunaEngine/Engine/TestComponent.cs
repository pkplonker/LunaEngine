namespace Engine;

public class TestComponent : Component
{
	//visable
	
	public int PropGetSet { get; set; }
	public string PropGetSetString { get; set; } = null;
	[Serializable]
	public int PropInspectable;
	
	[Serializable]
	public int FieldInspectable;
	
	[Serializable(CustomName = "Custom Name")]
	public int oldName;
	
	// not visable
	public int prop;
	public int field;
	[Serializable(false)]
	public int propGetSetNotInspectable { get; set; }
	
	public TestComponent(GameObject gameObject) : base(gameObject) { }
}