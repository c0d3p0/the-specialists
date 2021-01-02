using Godot;
using Godot.Collections;


public class LevelNextScene : Node
{
	public void LoadContinueTitleScreen()	// Called by an animation
	{
		PutGlobal("storyContinue", true);
		GetTree().Paused = false;
		LoadScene(titleScreenScenePath);
	}

	public void ReloadCurrentLevel() // Called by an animation
	{
		LoadScene(currentLevelScenePath);
	}

	public void LoadNextLevel() // Called by an animation
	{
		HandleNextLocation();
		LoadScene(nextLevelScenePath);
	}

	private void LoadScene(string scenePath)
	{
		PutGlobal("sceneToLoad", this.GetScenePath(scenePath));
		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
	}

	private void HandleNextLocation()
	{
		if(lastLevel)
		{
			int locationIndex = Mathf.Max(0, GetGlobal<int>("locationIndex")) + 1;
			PutGlobal("locationIndex", locationIndex);
		}
	}

	private void Initialize()
	{
		continues = GetGlobal<int>("continues");
		levelScenePathList = this.GetGlobal<Array>("storyLevelScenePathList");
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
	}

	private void PutGlobal(string key, object value)
	{
		globalData.Call(this.GetMethodPut(), key, value);
	}

	private T GetGlobal<T>(string key)
	{
		return this.Call<T>(globalData, this.GetMethodGet(), key);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}

	public string CurrentLevelScenePath
	{
		set
		{
			currentLevelScenePath = value;
		}
	}

	public string NextLevelScenePath
	{
		set
		{
			nextLevelScenePath = value;
		}
	}

	public bool LastLevel
	{
		set
		{
			lastLevel = value;
		}
	}


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public string loadScreenScenePath = "screen/load_screen";

	[Export]
	public string titleScreenScenePath = "screen/title_screen";


	private Node globalData;
	private string currentLevelScenePath;
	private string nextLevelScenePath;

	private int continues;
	private Array levelScenePathList;
	private bool lastLevel;
}
