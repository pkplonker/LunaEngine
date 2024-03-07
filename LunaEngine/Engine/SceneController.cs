namespace Engine;

public static class SceneController
{
	private static Scene? activeScene;

	public static Scene? ActiveScene
	{
		get => activeScene;
		set
		{
			if (value != activeScene)
			{
				activeScene = value;
				OnActiveSceneChanged?.Invoke(activeScene);
			}
		} }
	public static Action<Scene?> OnActiveSceneChanged { get; set; }
}