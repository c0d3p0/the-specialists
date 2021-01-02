using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class GameplayRankingGUI : Node
{
	private void UpdateRankingLabels(int index)
	{
		string indexText = index.ToString();

		if(rankingMap.Contains(indexText))
		{
			rankingControls[index].Visible = true;
			Label[] labels = labelsMap[index];
			Dictionary dataMap = rankingMap[indexText] as Dictionary;
			SCG.IEnumerator<string> it = rankingKeyList.GetEnumerator();
			int labelIndex = 0;
			
			while(it.MoveNext())
				labels[labelIndex++].Text = GetFixedLabelText(dataMap, it.Current);
		}
		else
			rankingControls[index].Visible = false;
	}

	private void UpdateRankingControls()
	{
		for(int i = 0; i < rankingControls.Length; i++)
			UpdateRankingLabels(i);

		noDataControl.Visible = false;
	}

	private void UpdateNoDataControl()
	{
		for(int i = 0; i < rankingControls.Length; i++)
			rankingControls[i].Visible = false;

		noDataControl.Visible = true;
	}

	private void UpdateGUI()
	{
		rankingMap = this.Call<Dictionary>(gameplayRankingData,
				this.GetMethodGetGameplayRankingMap());
		
		if(rankingMap.Count > 0)
			UpdateRankingControls();
		else
			UpdateNoDataControl();
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

			if(allHours < 96 && time > 0)
			{
				return labelPrefixMap[key] + string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
						allHours, allMinutes % 60, allSeconds % 60, time % 1000);
			}
			else
				return labelPrefixMap[key] + "4+ days";
		}
		else
			return labelPrefixMap[key] + dataMap[key];
	}

	private void ObtainLabelNodesFromControl(int index)
	{
		Label[] labels = new Label[rankingControls[index].GetChildCount() - 1];

		for(int i = 1; i < rankingControls[index].GetChildCount(); i++)
			labels[i - 1] = rankingControls[index].GetChild<Label>(i);

		labelsMap.Add(index, labels);
	}

	private void ObtainAllLabelNodes()
	{
		labelsMap = new SCG.Dictionary<int, Label[]>();

		for(int i = 0; i < rankingControls.Length; i++)
			ObtainLabelNodesFromControl(i);
	}

	private void ObtainNodes()
	{
		rankingControls = this.GetNodes<Control>(this, rankingControlNPList);
		noDataControl = GetNode<Control>(noDataControlNP);
		gameplayRankingData = GetNode(gameplayRankingDataNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		ObtainAllLabelNodes();
	}

	public override void _Ready()
	{
		UpdateGUI();
	}


	[Export]
	public NodePath gameplayRankingDataNP;

	[Export]
	public Array<NodePath> rankingControlNPList;

	[Export]
	public NodePath noDataControlNP;

	[Export]
	public Array<string> rankingKeyList;

	[Export]
	public Dictionary<string, string> labelPrefixMap;


	private Node gameplayRankingData;
	private Control[] rankingControls;
	private Control noDataControl;
	private SCG.Dictionary<int, Label[]> labelsMap;

	private Dictionary rankingMap;
}
