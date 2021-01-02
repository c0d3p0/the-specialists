using Godot;
using Godot.Collections;


public class DialogueScreen : Node
{
	public void GoToLevelScene()
	{
		PutGlobal("sceneToLoad", GetNextScenePath());
		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
	}

	public void OnStartLevelButtonPressed()
	{
		GoToLevelScene();
	}

	private void PrepareEndGame()
	{
		if(locationIndex == 4)
		{
			PutGlobal("optionSection", 1);	
			PutGlobal("saveGameplayData", true);
		}
	}

	private void UpdateLabel()
	{
		if(locationIndex == 0)
			dialogueLabel.Text = GetDialogue1();
		else if(locationIndex == 1)
			dialogueLabel.Text = GetDialogue2();
		else if(locationIndex == 2)
			dialogueLabel.Text = GetDialogue3();
		else if(locationIndex == 3)
			dialogueLabel.Text = GetDialogue4();
		else if(locationIndex == 4)
			dialogueLabel.Text = GetDialogue5();
	}

	private void UpdateBackgroundPicture()
	{
		if(locationIndex > -1 && locationIndex < backgroundPictureList.Count)
			backgroundTextureRect.Texture = backgroundPictureList[locationIndex];
		else
			backgroundTextureRect.Texture = backgroundPictureList[0];
	}

	private string GetNextScenePath()
	{
		return locationIndex < locationScenePathList.Count ?
				this.GetScenePath(locationScenePathList[locationIndex]) :
				this.GetScenePath(creditsScreenScenePath);
	}

	private string GetDialogue1()
	{
		return
				"Apparently a crazy scientist is creating and releasing " +
				"biological weapons around the world. We are doing an " +
				"investigation to find out where he is so we can stop him. " +
				"Our team is specialized in dealing with the most critical " +
				"issues, so your task for now is to help clear areas that " +
				"are very affected by those things, and as soon we find " +
				"out where the scientist is, you will be responsible to " +
				"deal with him.";
	}

	private string GetDialogue2()
	{
		return
				"You did a fast and clean work, we are still gathering " + 
				"information of where the scientist might be, so the best " +
				"thing you can do right now is to help clear another area. " + 
				"These mutants invaded a very important mine, our country may " +
				"lose a lot of money if we cannot recover all the precious " + 
				"material there. We are all counting on you to do that.";
	}

	private string GetDialogue3()
	{
		return
				"We are almost there, we already found out that this crazy " +
				"scientist has a laboratory that he is using to create his " +
				"biological weapons, the only thing left now is to find out the " +
				"exact location of his hidden laboratory and his identity. " +
				"Meanwhile, there is a city that is badly affected, people " +
				"are dying and we request you to help them, we hope that when " + 
				"you come back, we will have the laboratory location, so you " + 
				"can secretly go there and end it all.";
	}

	private string GetDialogue4()
	{
		return
				"We finally have the identity of the scientist and the location " +
				"of his laboratory. As incredible as it sounds, it is not even " +
				"a human who is creating all those things, it is a robot. His " +
				"name is Cybotron and your objective now is to go there and " +
				"literally melt its circuits.";
	}

	private string GetDialogue5()
	{
		return
				"Well, I didn't buy that story of a robot doing all that " +
				"craziness, don't be surprised if it start all over again. " +
				"Your hard work saved a lot of people, and we can keep living " +
				"our lives in peace, at least for now. As expected, we are " +
				"rewarding you with a Medal of Honor for everything you've done, " +
				"it is a pleasure to have specialists like you in our team.";
	}

	private void Initialize()
	{
		Input.SetMouseMode(Input.MouseMode.Visible);
		int max = locationScenePathList != null ? locationScenePathList.Count : 0;
		locationIndex = Mathf.Max(0, GetGlobal<int>("locationIndex"));
		locationIndex = Mathf.Min(locationIndex, max);
		animationPlayer.Play("dialogue_" + (locationIndex + 1));
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		dialogueLabel = GetNode<Label>(dialogueLabelNP);
		backgroundTextureRect = GetNode<TextureRect>(backgroundTextureRectNP);
		animationPlayer = GetNode<AnimationPlayer>(animationPlayerNP);
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
		UpdateLabel();
		PrepareEndGame();
		UpdateBackgroundPicture();
	}


	[Export]
	public string loadScreenScenePath = "screen/load_screen";

	[Export]
	public string creditsScreenScenePath = "screen/credits_screen";

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	private NodePath backgroundTextureRectNP;

	[Export]
	private NodePath dialogueLabelNP;

	[Export]
	private NodePath animationPlayerNP;

	[Export]
	private Array<StreamTexture> backgroundPictureList;

	[Export]
	private Array<string> locationScenePathList;


	private Node globalData;
	private TextureRect backgroundTextureRect;
	private Label dialogueLabel;
	private AnimationPlayer animationPlayer;

	private int locationIndex;
}
