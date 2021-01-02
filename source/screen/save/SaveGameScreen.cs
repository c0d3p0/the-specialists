using Godot;
using Godot.Collections;


public class SaveGameScreen : Control
{
	public void ShowScreen(Control hideControl)
	{
		this.hideControl = hideControl;
		LoadData();
		ShowScreen(true);
		UpdateGUI();
	}

	public void OnLoadButtonPressed()
	{
		Dictionary dataMap = saveGameDataMap[saveGameDataIndex] as Dictionary;

		if(dataMap.Contains("p2Deaths"))
			PutGlobal("p2Deaths", System.Convert.ToInt32(dataMap["p2Deaths"]));

		PutGlobal("continues", System.Convert.ToInt32(dataMap["continues"]));
		PutGlobal("locationIndex", System.Convert.ToInt32(dataMap["locationIndex"]));
		PutGlobal("totalTime", dataMap["totalTime"] as string);
		PutGlobal("p1Deaths", System.Convert.ToInt32(dataMap["p1Deaths"]));
		PutGlobal("gameMode", dataMap["gameMode"]);
		ShowScreen(false);
		storyModeScreen.Call(this.GetMethodShowScreen(), hideControl, true);
	}

	public void OnSaveButtonPressed(bool overwrite)
	{
		Dictionary saveDataMap = new Dictionary();		
		saveDataMap.Add("continues", GetGlobal<int>("continues").ToString());
		saveDataMap.Add("locationIndex", GetGlobal<int>("locationIndex").ToString());
		saveDataMap.Add("p1Deaths", GetGlobal<int>("p1Deaths").ToString());
		saveDataMap.Add("p2Deaths", GetGlobal<int>("p2Deaths").ToString());
		saveDataMap.Add("gameMode", GetGlobal<string>("gameMode"));
		saveDataMap.Add("totalTime",
				System.Convert.ToInt64(GetGlobal<string>("totalTime")).ToString());

		if(overwrite)
		{
			saveGameData.Call(this.GetMethodSave(), saveDataMap,
					saveGameDataIndex.ToString());
		}
		else
			saveGameData.Call(this.GetMethodSave(), saveDataMap);
		
		LoadData();
		UpdateGUI();
	}

	public void OnLoadGameDataChangeButtonPressed(int amount)
	{
		saveGameDataIndex = Mathf.Min(saveGameDataIndex + amount, saveGameDataMap.Count - 1);
		saveGameDataIndex = Mathf.Max(0, saveGameDataIndex);
		UpdateGUI();
	}

	public void OnRemoveButtonPressed()
	{
		saveGameDataMap.Remove(saveGameDataIndex);
		saveGameData.Call(this.GetMethodSave(), saveGameDataMap);
		LoadData();
		UpdateGUI();
	}

	private void UpdateGUI()
	{
		if(saveGameDataMap.Count > 0)
		{
			Dictionary dataMap = saveGameDataMap[saveGameDataIndex] as Dictionary;
			int index = 0;

			for(int i = 0; i < orderedKeys.Length; i++)
			{
				if(dataMap.Contains(orderedKeys[i]))
					dataLabels[index++].Text = GetFixedLabelText(dataMap, orderedKeys[i]);
			}
		}

		dataControl.Visible = saveGameDataMap.Count > 0;
		noDataControl.Visible = !dataControl.Visible;
	}

	private string GetFixedLabelText(Dictionary dataMap, string key)
	{
		string data = dataMap[key] as string;

		if(key.Equals("totalTime"))
		{
			long time = System.Int64.Parse(data);
			int allSeconds = (int) (time / 1000L);
			int allMinutes = (int) (allSeconds / 60);
			int allHours = (int) (allMinutes / 60);

			if(allHours < 96 && time >= 0)
			{
				return labelPrefixMap[key] + string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
						allHours, allMinutes % 60, allSeconds % 60, time % 1000);
			}
			else
				return labelPrefixMap[key] + "4+ days";
		}
		else if(key.Equals("locationIndex"))
		{
			int index = System.Convert.ToInt32(data);
			return labelPrefixMap[key] + locationNameList[index];
		}
		else
			return labelPrefixMap[key] + data;
	}

	private void LoadData()
	{
		saveGameDataMap = this.Call<Dictionary>(saveGameData, this.GetMethodLoad());
		saveGameDataIndex = Mathf.Max(0, saveGameDataMap.Count - 1);
	}

	private void ShowScreen(bool show)
	{
		hideControl.Visible = !show;
		this.ShowBehindParent = !show;
		this.Visible = show;
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

	public void Initialize()
	{
		orderedKeys = new string[]{"gameMode", "locationIndex",
				"p1Deaths", "p2Deaths", "continues", "totalTime"};
	}

	public void ObtainLabels()
	{
		dataLabels = new Label[dataControl.GetChildCount() - 1];

		for(int i = 0; i < dataLabels.Length; i++)
			dataLabels[i] = dataControl.GetChild<Label>(i);
	}

	public void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		saveGameData = GetNode(saveGameDataNP);
		dataControl = GetNode<Control>(dataControlNP);
		noDataControl = GetNode<Control>(noDataControlNP);

		if(shouldLoadGame)
			storyModeScreen = GetNode(storyModeScreenNP);
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
		ObtainLabels();
		Initialize();
	}

	public override void _Ready()
	{
		Initialize();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if(this.Visible)
			HandleExit(inputEvent as InputEventKey);
	}


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath storyModeScreenNP;

	[Export]
	public NodePath saveGameDataNP;

	[Export]
	public NodePath dataControlNP;

	[Export]
	public NodePath noDataControlNP;

	[Export]
	public Array<string> locationNameList;

	[Export]
	public Dictionary<string, string> labelPrefixMap;

	[Export]
	public bool shouldLoadGame;


	private Control hideControl;

	private Node globalData;
	private Node storyModeScreen;
	private Node saveGameData;
	private Control dataControl;
	private Control noDataControl;
	private Label[] dataLabels;

	private string[] orderedKeys;
	private Dictionary saveGameDataMap;
	private int saveGameDataIndex;
}
