using Godot;
using Godot.Collections;


public class TitleScreen : Node
{
	public void OnStoryModeButtonPressed()
	{
		storyModeScreen.Call(this.GetMethodShowScreen(), contentControl);
	}

	public void OnBattleModeButtonPressed()
	{
		battleModeScreen.Call(this.GetMethodShowScreen(), contentControl);
	}

	public void OnLoadGameButtonPressed()
	{
		loadGameScreen.Call(this.GetMethodShowScreen(), contentControl);
	}

	public void OnOptionsButtonPressed()
	{
		optionScreen.Call(this.GetMethodShowScreen(), contentControl);
	}

	public void OnQuitGameButtonPressed()
	{
		GetTree().Quit();
	}

	public void OnToggleCheatCode(string dataKey, object value, bool active)
	{
		if(active)
			cheatDataMap.Add(dataKey, value);
		else
		{
			if(cheatDataMap.Contains(dataKey))
				cheatDataMap.Remove(dataKey);
		}
	}

	private void Initialize()
	{
		Input.SetMouseMode(Input.MouseMode.Visible);
		cheatDataMap = new Dictionary();
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		storyModeScreen = GetNode(storyModeScreenNP);
		loadGameScreen = GetNode(loadGameScreenNP);
		battleModeScreen = GetNode(battleModeScreenNP);
		optionScreen = GetNode(optionScreenNP);
		cheatCode = GetNode(cheatCodeNP);
		contentControl = GetNode<Control>(contentControlNP);
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).Get(
				"parameters/playback") as AnimationNodeStateMachinePlayback;
	}

	private bool DisplayOptionScreen()
	{
		int optionSection = TryToGetGlobal<int>("optionSection", -1);

		if(optionSection > -1)
		{
			animationStateMachine.Travel("active");
			optionScreen.Call(this.GetMethodShowScreen(), contentControl, optionSection);
			PutGlobal("optionSection", -1);
		}

		return optionSection > -1;
	}

	private bool DisplayBattleModeScreen()
	{
		bool battleResult = GetGlobal<bool>("battleResult");

		if(battleResult)
		{
			animationStateMachine.Travel("active");
			battleModeScreen.Call(this.GetMethodShowScreen(), contentControl, battleResult);
			PutGlobal("battleResult", false);
		}

		return battleResult;
	}

	private bool DisplayStoryModeScreen()
	{
		bool storyContinue = GetGlobal<bool>("storyContinue");

		if(storyContinue)
		{
			animationStateMachine.Travel("active");
			storyModeScreen.Call(this.GetMethodShowScreen(), contentControl, storyContinue);
			PutGlobal("storyContinue", false);
		}

		return storyContinue;
	}

	private void DisplaySubScreen()
	{
		bool ignore = DisplayOptionScreen();
		ignore = ignore || DisplayBattleModeScreen();
		ignore = ignore || DisplayStoryModeScreen();

		if(!ignore)
			animationStateMachine.Travel("intro");
	}

	protected void PutGlobal(string key, object value)
	{
		globalData.Call(this.GetMethodPut(), key, value);
	}

	protected T GetGlobal<T>(string key)
	{
		return this.Call<T>(globalData, this.GetMethodGet(), key);
	}

	protected T TryToGetGlobal<T>(string key, T defaultValue)
	{
		return this.TryToCall<T>(globalData, this.GetMethodGet(), defaultValue, key);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
		DisplaySubScreen();
	}

	public override void _ExitTree()
	{
		if(OS.IsDebugBuild())
			globalData.Call(this.GetMethodPut(), cheatDataMap);
	}

	public override void _Notification(int what)
	{
		if(what == MainLoop.NotificationWmFocusOut)
			GetTree().Paused = true;
		else if(what == MainLoop.NotificationWmFocusIn)
			GetTree().Paused = false;
	}


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath contentControlNP;

	[Export]
	public NodePath storyModeScreenNP;

	[Export]
	public NodePath loadGameScreenNP;

	[Export]
	public NodePath battleModeScreenNP;

	[Export]
	public NodePath optionScreenNP;

	[Export]
	public NodePath cheatCodeNP;

	[Export]
	public NodePath animationTreeNP;


	private Node globalData;
	private Control contentControl;
	private Node storyModeScreen;
	private Node loadGameScreen;
	private Node battleModeScreen;
	private Node optionScreen;
	private Node cheatCode;


	private AnimationNodeStateMachinePlayback animationStateMachine;
	private Dictionary cheatDataMap;
}
