using Godot;


public class LoadScreen : Node
{
	private void HandleChangeScene()
	{
		if(loadedScene != null)
		{
			taskRunner.Call(this.GetMethodSetActive(), false);
			taskRunner.Call(this.GetMethodClear());
			GetTree().ChangeSceneTo(loadedScene);
		}
	}

	private void LoadScene()
	{
		loadedScene = ResourceLoader.Load(scenePath) as PackedScene;
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		taskRunner = GetNode(taskRunnerNodePath);
	}

	private T GetGlobal<T>(string key)
	{
		return this.Call<T>(globalData, this.GetMethodGet(), key);
	}

	public void Initialize()
	{
		scenePath = GetGlobal<string>("sceneToLoad");
		taskRunner.Call(this.GetMethodPut(), this, nameof(LoadScene));
		taskRunner.Call(this.GetMethodSetActive(), true);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}

	public override void _Ready()
	{
		SetProcess(process);
	}

	public override void _Process(float delta)
	{
		HandleChangeScene();
	}


	[Export]
	public bool process = true;

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public string taskRunnerNodePath = "/root/TaskRunner";


	private Node globalData;
	private Node taskRunner;
	private string scenePath;
	private PackedScene loadedScene;
}
