using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class LastGameplayGUI : Node
{
	private void UpdateLabels()
	{
		SCG.IEnumerator<string> it = gameplayKeyList.GetEnumerator();
		int index = 0;

		while(it.MoveNext())
		{
			if(lastGameplayDataMap.Contains(it.Current))
				labels[index++].Text = GetFixedLabelText(it.Current);
		}

		for(int i = index; i < labels.Length; i++)
			labels[i].Text = "";
	}

	private void UpdateControls(bool validData)
	{
		lastGameplayControl.Visible = validData;
		noDataControl.Visible = !validData;
	}

	private void UpdateGUI()
	{
		lastGameplayDataMap = this.Call<Dictionary>(gameplayData,
				this.GetMethodGetLastGameplayDataMap());
		
		if(lastGameplayDataMap != null)
		{
			UpdateControls(true);
			UpdateLabels();
		}
		else
			UpdateControls(false);
	}

	private void ObtainNodes()
	{
		gameplayData = GetNode(gameplayDataNP);
		lastGameplayControl = GetNode<Control>(lastGameplayControlNP);
		noDataControl = GetNode<Control>(noDataControlNP);
		labels = this.GetChildren<Label>(lastGameplayControl);
	}

	private string GetFixedLabelText(string key)
	{
		string data = lastGameplayDataMap[key] as string;

		if(key.Equals("totalTime"))
		{
			long time = System.Int64.Parse(data);
			int allSeconds = (int) (time / 1000L);
			int allMinutes = (int) (allSeconds / 60);
			int allHours = (int) (allMinutes / 60);

			if(allHours < 96 && time > 0)
			{
				return labelPrefixMap[key] + string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
						allHours, allMinutes % 60, allSeconds % 60, time % 1000);
			}
			else
				return labelPrefixMap[key] + "4+ days";
		}
		else
			return labelPrefixMap[key] + lastGameplayDataMap[key];
	}

	public override void _Ready()
	{
		UpdateGUI();
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}



	[Export]
	public NodePath gameplayDataNP;

	[Export]
	public NodePath lastGameplayControlNP;

	[Export]
	public NodePath noDataControlNP;

	[Export]
	public Array<string> gameplayKeyList;

	[Export]
	public Dictionary<string, string> labelPrefixMap;


	private Node gameplayData;
	private Control lastGameplayControl;
	private Control noDataControl;
	private Label[] labels;

	private Dictionary lastGameplayDataMap;
}
