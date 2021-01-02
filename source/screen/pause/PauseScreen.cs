using Godot;


public class PauseScreen : Control
{
	public void OnResumeButtonPressed()
	{
		if(paused)
			Pause();
	}

	public void OnInputMappingButtonPressed()
	{
		inputMappingScreen.Call(this.GetMethodShowScreen(), contentControl);
	}

	public void OnSaveGameButtonPressed()
	{
		saveGameScreen.Call(this.GetMethodShowScreen(), contentControl);
	}

	public void OnQuitButtonPressed()
	{
		ReturnToTitleScreen();
	}

	private void Pause()
	{
		paused = !paused;
		Input.SetMouseMode(paused ? Input.MouseMode.Visible : Input.MouseMode.Captured);
		GetTree().Paused = paused;
		this.Visible = paused;
		this.ShowBehindParent = !paused;
		this.ShowOnTop = paused;
	}

	private void ReturnToTitleScreen()
	{
		PutGlobal("sceneToLoad", this.GetScenePath(titleScreenScenePath));
		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
		GetTree().Paused = false;
	}

	private void HandlePauseInput(InputEvent inputEvent)
	{
		for(int i = 1; i <= playerAmount; i++)
		{
			if(inputEvent.IsActionPressed(this.CreateString('p', i, "_pause")))
			{
				Pause();
				break;
			}
		}
	}

	public override void _Input(InputEvent inputEvent)
	{
		HandlePauseInput(inputEvent);
	}

	private void Initialize()
	{
		globalData = GetNode(globalDataNodePath);
		inputMappingScreen = GetNode<Control>(inputMappingScreenNP);
		saveGameScreen = GetNode<Control>(saveGameScreenNP);
		contentControl = GetNode<Control>(contentControlNP);
		manualPause = GetGlobal<bool>("manualPause");
		this.Visible = paused;
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
		Initialize();
	}

	public override void _Notification(int what)
	{
		if(!paused && !manualPause && what == MainLoop.NotificationWmFocusOut)
			Pause();
	}

	
	[Export]
	public NodePath contentControlNP;

	[Export]
	public NodePath inputMappingScreenNP;

	[Export]
	public NodePath saveGameScreenNP;

	[Export]
	public string loadScreenScenePath = "screen/load_screen";

	[Export]
	public string titleScreenScenePath = "screen/title_screen";

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public int playerAmount = 4;

	private Node globalData;

	private Control contentControl;
	private Control inputMappingScreen;
	private Control saveGameScreen;
	private bool paused;
	private bool manualPause;
}
