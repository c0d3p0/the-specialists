using Godot;
using Godot.Collections;


public class BattleNextScene : Node
{
	public void GoToNextRound() // Called by an animation
	{
		string levelScenePath;

		if(battleLevelIndex >= levelScenePathList.Count)
		{
			levelScenePath = levelScenePathList[
					this.RandiRange(rng, 0, levelScenePathList.Count - 1)] as string;
		}
		else
			levelScenePath = levelScenePathList[battleLevelIndex] as string;

		PutGlobal("battleRound", battleRound + 1);
		LoadScene(levelScenePath);
	}

	public void FinishBattle() // Called by an animation
	{
		PutGlobal("battleResult", true);
		LoadScene(titleScreenScenePath);
	}

	private void LoadScene(string scenePath)
	{
		PutGlobal("sceneToLoad", this.GetScenePath(scenePath));
		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
	}

	private void Initialize()
	{
		rng = new RandomNumberGenerator();
		battleRound = GetGlobal<int>("battleRound");
		levelScenePathList = GetGlobal<Array>("levelScenePathList");
		battleLevelIndex = GetGlobal<int>("levelIndex");
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


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public string loadScreenScenePath = "screen/load_screen";

	[Export]
	public string titleScreenScenePath = "screen/title_screen";


	private Node globalData;

	private RandomNumberGenerator rng;
	private Array levelScenePathList;
	private int battleLevelIndex;
	private int battleRound;
}
