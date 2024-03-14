namespace Engine;

public static class SceneController
{
	static SceneController()
	{
		ProjectManager.ProjectChanged += OnProjectChanged;
	}

	private static void OnProjectChanged(IProject? obj)
	{
		ActiveScene = null;
	}

	private static IScene? activeScene;

	public static IScene? ActiveScene
	{
		get => activeScene;
		set
		{
			if (value != activeScene)
			{
				var oldScene = activeScene;
				activeScene = value;
				OnActiveSceneChanged?.Invoke(activeScene,oldScene);
			}
		} }
	public static Action<IScene?, IScene?> OnActiveSceneChanged { get; set; }
}