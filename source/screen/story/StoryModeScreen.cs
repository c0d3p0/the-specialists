using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class StoryModeScreen : Control
{
	public void ShowScreen(Control hideControl)
	{
		ShowScreen(hideControl, false);
	}

	public void ShowScreen(Control hideControl, bool loadGame)
	{
		if(loadGame)
			PrepareLoadStoryMode();
		else
			PrepareNewStoryMode();

		this.hideControl = hideControl;
		ShowScreen(true);
		UpdateGUI();
		UpdateSpecialistControls();
	}

	public void OnSpecialistAmountButtonPressed(int amount)
	{
		specialistAmountIndex = Mathf.Max(specialistAmountIndex + amount, 0);
		specialistAmountIndex = Mathf.Min(specialistAmountIndex,
				specialistAmountTextList.Count - 1);
		specialistAmountLabel.Text = specialistAmountTextList[specialistAmountIndex];
		UpdateSpecialistControls();
	}

	public void OnSpecialistSelectButtonPressed(int index, int amount)
	{
		specialistColorIndexes[index] = Mathf.Max(
				specialistColorIndexes[index] + amount, 0);
		specialistColorIndexes[index] = Mathf.Min(
				specialistColorIndexes[index], specialistTextureList.Count - 1);
		specialistTextureRects[index].Texture =
				specialistTextureList[specialistColorIndexes[index]];
	}

	public void OnStartButtonPressed()
	{	
		globalData.Call(this.GetMethodClear()); // NOTE: This clears all entries inside GlobalData.
		PutGlobal("totalTime", totalTime.ToString());
		PutGlobal("p1Deaths", p1Deaths);
		PutGlobal("p2Deaths", p2Deaths);
		PutGlobal("continues", continues);
		PutGlobal("levelScenePathList", locationScenePathList);
		PutGlobal("locationIndex", locationIndex);
		PutGlobal("specialistAmount", specialistAmountIndex + 1);
		PutGlobal("gameMode", (specialistAmountIndex + 1) + "P Story Mode");
		PutGlobal("sceneToLoad", this.GetScenePath(dialogueScreenScenePath));

		SCG.HashSet<int> selectedSpecialistKeySet = new SCG.HashSet<int>();
		Array slotList;
		int specialistColorIndex;
		int autoSelect;

		for(int i = 0; i < specialistAmountIndex + 1; i++)
		{
			specialistColorIndex = specialistColorIndexes[i];
			autoSelect = 0;
			slotList = new Array();
			slotList.Add("S");
			slotList.Add("S");

			while(selectedSpecialistKeySet.Contains(specialistColorIndex))
				specialistColorIndex = autoSelect++;

			selectedSpecialistKeySet.Add(specialistColorIndex);

			PutGlobal("livesSpecialistIndex" + i, 4);
			PutGlobal("healthSpecialistIndex" + i, 100);
			PutGlobal("speedLevelSpecialistIndex" + i, 1);
			PutGlobal("laserRayLevelSpecialistIndex" + i, 1);
			PutGlobal("laserDeviceAmountSpecialistIndex" + i, 1);
			PutGlobal("detonateTimeLevelSpecialistIndex" + i, 1);
			PutGlobal("slotListSpecialistIndex" + i, slotList);
			PutGlobal("selectedSlotSpecialistIndex" + i, 0);
			PutGlobal("skillSpecialistIndex" + i, "");
			PutGlobal("colorSpecialistIndex" + i, specialistColorIndex);
		}

		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
	}

	private void UpdateGUI()
	{
		specialistAmountLabel.Text = specialistAmountTextList[specialistAmountIndex];
		locationLabel.Text = locationNameList[locationIndex];

		for(int i = 0; i < specialistTextureRects.Length; i++)
		{
			specialistTextureRects[i].Texture =
					specialistTextureList[specialistColorIndexes[i]];
		}
	}

	private void UpdateSpecialistControls()
	{
		for(int i = 0; i < specialistControls.Length; i++)
			specialistControls[i].Visible = i <= specialistAmountIndex;
	}

	private void Initialize()
	{
		specialistColorIndexes = new int[specialistTextureRects.Length];
		locationIndex = GetGlobal<int>("locationIndex");
		
		for(int i = 0; i < specialistTextureRects.Length; i++)
			specialistColorIndexes[i] = i;
	}

	private void ShowScreen(bool show)
	{
		hideControl.Visible = !show;
		ShowBehindParent = !show;
		Visible = show;
		SetProcessInput(show);
	}

	private void HandleExit(InputEventKey inputEventKey)
	{
		if(inputEventKey != null && inputEventKey.Pressed)
		{
			uint keycode = inputEventKey.Scancode;

			if((uint) KeyList.Escape == keycode)
				ShowScreen(false);
		}
	}

	// NOTE: Unused
	private void OnLocationButtonPressed(int amount)
	{
		locationIndex = Mathf.Max(locationIndex + amount, 0);
		locationIndex = Mathf.Min(locationIndex, 3);
		locationLabel.Text = locationNameList[locationIndex];
	}
	// NOTE: Unused

	private void InitializeSpecialistsColor()
	{
		int specialistColor;
		int autoSelect;
		SCG.HashSet<int> selectedSpecialistKeySet = new SCG.HashSet<int>();

		for(int i = 0; i < specialistControls.Length; i++)
		{
			specialistColor = GetGlobal<int>("colorSpecialistIndex" + i);
			autoSelect = 0;

			while(selectedSpecialistKeySet.Contains(specialistColor))
				specialistColor = autoSelect++;

			selectedSpecialistKeySet.Add(specialistColor);
			specialistColorIndexes[i] = specialistColor;
		}
	}

	private void ObtainNodesFromSpecialistControls()
	{
		specialistTextureRects = new TextureRect[specialistControls.Length];

		for(int i = 0; i < specialistControls.Length; i++)
			specialistTextureRects[i] = specialistControls[i].GetChild<TextureRect>(0);
	}
	
	private void PrepareNewStoryMode()
	{
		totalTime = 0;
		p1Deaths = 0;
		p2Deaths = 0;
		continues = 0;
		locationIndex = 0;
		specialistAmountActionControl.Visible = true;
	}

	private void PrepareLoadStoryMode()
	{
		// Each segmented gameplay is considered a continue lost.
		continues = GetGlobal<int>("continues") + 1;
		locationIndex = GetGlobal<int>("locationIndex");
		totalTime = System.Convert.ToInt64(GetGlobal<string>("totalTime"));
		p1Deaths = GetGlobal<int>("p1Deaths");
		p2Deaths = GetGlobal<int>("p2Deaths");
		string gm = GetGlobal<string>("gameMode");
		specialistAmountIndex = gm.Equals("2P Story Mode") ? 1 : 0;
		specialistAmountActionControl.Visible = false;
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		specialistControls = this.GetNodes<Control>(this, specialistControlNPList);
		specialistAmountLabel = GetNode<Label>(specialistAmountLabelNP);
		locationLabel = GetNode<Label>(locationLabelNP);
		specialistAmountActionControl = GetNode<Control>(
				specialistAmountActionControlNP);
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
		ObtainNodesFromSpecialistControls();
		Initialize();
		InitializeSpecialistsColor();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if(Visible)
			HandleExit(inputEvent as InputEventKey);
	}


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath specialistAmountLabelNP;

	[Export]
	public NodePath locationLabelNP;

	[Export]
	public NodePath specialistAmountActionControlNP;

	[Export]
	public Array<NodePath> specialistControlNPList;

	[Export]
	public string loadScreenScenePath = "screen/load_screen";

	[Export]
	public string dialogueScreenScenePath = "screen/dialogue_screen";

	[Export]
	public Array<StreamTexture> specialistTextureList;
	
	[Export]
	public Array locationScenePathList;

	[Export]
	public Array<string> locationNameList;

	[Export]
	public Array<string> specialistAmountTextList;


	private Control hideControl;

	private Node globalData;
	private Control specialistAmountActionControl;
	private Control[] specialistControls;
	private Label specialistAmountLabel;
	private Label locationLabel;
	private TextureRect[] specialistTextureRects;

	private int specialistAmountIndex;
	private int locationIndex;
	private int[] specialistColorIndexes;
	private long totalTime;
	private int p1Deaths;
	private int p2Deaths;
	private int continues;
}
