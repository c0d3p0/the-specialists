using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class BattleModeScreen : Control
{
	public void ShowScreen(Control hideControl)
	{
		this.hideControl = hideControl;
		ShowScreen(true);
		UpdateGUI(false);
	}

	public void ShowScreen(Control hideControl, bool result)
	{
		this.hideControl = hideControl;
		ShowScreen(true);
		UpdateGUI(result);
	}

	public void OnSpecialistAmountButtonPressed(int amount)
	{
		specialistAmountIndex = Mathf.Max(specialistAmountIndex + amount, 0);
		specialistAmountIndex = Mathf.Min(specialistAmountIndex,
				specialistAmountTextList.Count - 1);
		configurationLabels[0].Text = specialistAmountTextList[specialistAmountIndex];
		UpdateSpecialistControls();
	}

	public void OnFirstToButtonPressed(int amount)
	{
		firstToIndex = Mathf.Max(firstToIndex + amount, 0);
		firstToIndex = Mathf.Min(firstToIndex, firstToTextList.Count - 1);
		configurationLabels[1].Text = firstToTextList[firstToIndex];
	}

	public void OnLocationButtonPressed(int amount)
	{
		locationIndex = Mathf.Max(locationIndex + amount, 0);
		locationIndex = Mathf.Min(locationIndex, locationTextList.Count - 1);
		configurationLabels[2].Text = locationTextList[locationIndex];
	}

	public void OnRounTimeButtonPressed(int amount)
	{
		roundTimeIndex = Mathf.Max(roundTimeIndex + amount, 0);
		roundTimeIndex = Mathf.Min(roundTimeIndex, roundTimeTextList.Count - 1);
		configurationLabels[3].Text = roundTimeTextList[roundTimeIndex];
	}

	public void OnSkillButtonPressed(int amount)
	{
		skillIndex = Mathf.Max(skillIndex + amount, 0);
		skillIndex = Mathf.Min(skillIndex, skillTextList.Count - 1);
		configurationLabels[4].Text = skillTextList[skillIndex];
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
		SCG.HashSet<int> selectedSpecialistKeySet = new SCG.HashSet<int>();
		Array slotList;
		int specialistColorIndex;
		int autoSelect;
		string[] skills = new string[]{"", "P", "L", "S"};
		
		globalData.Call(this.GetMethodClear()); // NOTE: This clears all entries inside GlobalData.
		PutGlobal("specialistAmountIndex", specialistAmountIndex);
		PutGlobal("firstToIndex", firstToIndex);
		PutGlobal("levelIndex", locationIndex);
		PutGlobal("roundTimeIndex", roundTimeIndex);
		PutGlobal("skillIndex", skillIndex);

		PutGlobal("sceneToLoad", GetLevelScenePath());
		PutGlobal("levelScenePathList", levelScenePathList);
		PutGlobal("firstTo", firstToIndex + 1);
		PutGlobal("levelIndex", locationIndex);
		PutGlobal("battleRound", 1);
		PutGlobal("specialistAmount", specialistAmountIndex + 2);
		PutGlobal("battleRoundTime", 180f + (roundTimeIndex * 60f));
		PutGlobal("battleStartingSkill", skills[skillIndex]);

		for(int i = 0; i < specialistControls.Length; i++)
		{
			specialistColorIndex = specialistColorIndexes[i];
			autoSelect = 0;
			slotList = new Array();
			slotList.Add("S");
			slotList.Add("S");

			while(selectedSpecialistKeySet.Contains(specialistColorIndex))
				specialistColorIndex = autoSelect++;

			selectedSpecialistKeySet.Add(specialistColorIndex);
			PutGlobal("winsSpecialistIndex" + i, 0);
			PutGlobal("livesSpecialistIndex" + i, 0);
			PutGlobal("healthSpecialistIndex" + i, 100);
			PutGlobal("speedLevelSpecialistIndex" + i, 1);
			PutGlobal("laserRayLevelSpecialistIndex" + i, 2);
			PutGlobal("laserDeviceAmountSpecialistIndex" + i, 1);
			PutGlobal("detonateTimeLevelSpecialistIndex" + i, 1);
			PutGlobal("slotListSpecialistIndex" + i, slotList);
			PutGlobal("selectedSlotSpecialistIndex" + i, 0);
			PutGlobal("skillSpecialistIndex" + i, skills[skillIndex]);
			PutGlobal("colorSpecialistIndex" + i, specialistColorIndex);
		}

		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
	}

	public void OnNewBattleButtonPressed()
	{
		configurationControl.Visible = true;
		resultControl.Visible = false;
	}

	private void UpdateConfigurationControl()
	{
		configurationLabels[0].Text = specialistAmountTextList[specialistAmountIndex];
		configurationLabels[1].Text = firstToTextList[firstToIndex];
		configurationLabels[2].Text = locationTextList[locationIndex];
		configurationLabels[3].Text = roundTimeTextList[roundTimeIndex];
		configurationLabels[4].Text = skillTextList[skillIndex];

		for(int i = 0; i < specialistControls.Length; i++)
		{
			specialistTextureRects[i].Texture =
					specialistTextureList[specialistColorIndexes[i]];
		}
	}

	private void UpdateResultGUI()
	{
		resultLabels[0].Text = locationTextList[locationIndex];
		resultLabels[1].Text = roundTimeTextList[roundTimeIndex];
		resultLabels[2].Text = firstToTextList[firstToIndex];
		resultLabels[3].Text = skillTextList[skillIndex];
		string[] specialistWinTexts = GetSortedSpecialistWinTexts();

		for(int i = 4; i < resultLabels.Length; i++)
		{
			resultLabels[i].Visible = (i - 4) < specialistAmountIndex + 2;

			if((i - 4) < specialistWinTexts.Length)
				resultLabels[i].Text = specialistWinTexts[i - 4];
		}
	}

	private void UpdateGUI(bool result)
	{
		if(result)
			UpdateResultGUI();

		UpdateConfigurationControl();
		UpdateSpecialistControls();
		configurationControl.Visible = !result;
		resultControl.Visible = result;
	}

	private void UpdateSpecialistControls()
	{
		for(int i = 0; i < specialistControls.Length; i++)
			specialistControls[i].Visible = i < specialistAmountIndex + 2;
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

	private string GetLevelScenePath()
	{
		if(locationIndex >= levelScenePathList.Count)
		{
			int randomIndex = this.RandiRange(rng, 0, levelScenePathList.Count - 1);
			return this.GetScenePath(levelScenePathList[randomIndex].ToString());
		}
		
		return this.GetScenePath(levelScenePathList[locationIndex].ToString());
	}

	private string[] GetSortedSpecialistWinTexts()
	{
		string[] swt = new string[specialistAmountIndex + 2];
		int w;

		for(int i = 0; i < swt.Length; i++)
		{
			w = GetGlobal<int>("winsSpecialistIndex" + i);
			swt[i] = this.CreateString(w, " Rounds, Player ", i + 1);
		}

		System.Array.Sort(swt);
		System.Array.Reverse(swt);
		return swt;
	}

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

	private void Initialize()
	{
		rng = new RandomNumberGenerator();
		specialistColorIndexes = new int[specialistTextureRects.Length];
		firstToIndex = GetGlobal<int>("firstToIndex");
		locationIndex = GetGlobal<int>("levelIndex");
		roundTimeIndex = GetGlobal<int>("roundTimeIndex");
		skillIndex = GetGlobal<int>("skillIndex");
		specialistAmountIndex = GetGlobal<int>("specialistAmountIndex");
		InitializeSpecialistsColor();
	}

	private void ObtainNodesFromSpecialistControls()
	{
		specialistTextureRects = new TextureRect[specialistControls.Length];

		for(int i = 0; i < specialistControls.Length; i++)
			specialistTextureRects[i] = specialistControls[i].GetChild<TextureRect>(0);
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		configurationControl = GetNode<Control>(configurationControlNP);
		resultControl = GetNode<Control>(resultControlNP);
		specialistControls = this.GetNodes<Control>(this, specialistControlNPList);
		configurationLabels = this.GetNodes<Label>(this, configurationLabelNPList);
		resultLabels = this.GetNodes<Label>(this, resultLabelNPList);
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
	}

	public override void _Input(InputEvent inputEvent)
	{
		if(Visible)
			HandleExit(inputEvent as InputEventKey);
	}


	[Export]
	public string loadScreenScenePath = "screen/load_screen";

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath configurationControlNP;

	[Export]
	public NodePath resultControlNP;

	[Export]
	public Array<NodePath> specialistControlNPList;

	[Export]
	public Array<NodePath> configurationLabelNPList;

	[Export]
	public Array<NodePath> resultLabelNPList;

	[Export]
	public Array<StreamTexture> specialistTextureList;

	[Export]
	public Array levelScenePathList;

	[Export]
	public Array<string> specialistAmountTextList;

	[Export]
	public Array<string> firstToTextList;

	[Export]
	public Array<string> locationTextList;

	[Export]
	public Array<string> roundTimeTextList;

	[Export]
	public Array<string> skillTextList;


	private Control hideControl;

	private Node globalData;
	private Control configurationControl;
	private Control resultControl;
	private Control[] specialistControls;
	private Label[] configurationLabels;
	private Label[] resultLabels;
	private TextureRect[] specialistTextureRects;

	private RandomNumberGenerator rng;
	private int specialistAmountIndex;
	private int firstToIndex;
	private int locationIndex;
	private int roundTimeIndex;
	private int skillIndex;
	private int[] specialistColorIndexes;
}
