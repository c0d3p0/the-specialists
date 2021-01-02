using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class GameplayRankingData : Node
{
	public void UpdateRank()
	{
		Load();
		RankGameplayDataMap();
		jsonSerializer.Call(this.GetMethodSave(),
				rankedGameplayDataMap, GetFilePath());
	}

	private void SaveNewGameplayData()	// Save the last gameplay if there is one
	{
		Dictionary newDataMap = this.Call<Dictionary>(gameplayData,
				this.GetMethodGetGameplayDataMap(), gameMode);

		if(newDataMap != null)
		{
			Load();
			gameplayDataMap.Add(gameplayDataMap.Count.ToString(), newDataMap);
			RankGameplayDataMap();
			jsonSerializer.Call(this.GetMethodSave(),
					rankedGameplayDataMap, GetFilePath());
			newDataMap.Add("gameMode", gameMode);
			gameplayData.Call(this.GetMethodSave(), newDataMap);
			gameplayData.Call(this.GetMethodClear());
		}
	}

	private void RankGameplayDataMap()
	{
		Dictionary currentDataMap;
		string currentKey;
		long currentValue;
		string biggestRankKey = null;
		long biggestValue = long.MinValue;
		int iterations = gameplayDataMap.Count < 10 ? gameplayDataMap.Count : 10;
		SCG.HashSet<string> selectedMapSet = new SCG.HashSet<string>();
		rankedGameplayDataMap = new Dictionary();

		for(int i = 0; i < 3; i++)
		{
			for(int j = 0; j < iterations; j++)
			{
				currentKey = j.ToString();

				if(gameplayDataMap.Contains(currentKey) &&
						!selectedMapSet.Contains(currentKey))
				{
					currentDataMap = gameplayDataMap[currentKey] as Dictionary;

					if(this.Call<bool>(gameplayData, this.GetMethodIsValidDataMap(),
							currentDataMap, gameplayDataKeyList, gameMode))
					{
						currentValue = long.Parse(currentDataMap[rankKey] as string);

						if(currentValue > biggestValue)
						{
							biggestValue = currentValue;
							biggestRankKey = currentKey;
						}
					}
				}
			}

			if(gameplayDataMap.Contains(biggestRankKey) &&
					!selectedMapSet.Contains(biggestRankKey))
			{
				rankedGameplayDataMap.Add(i.ToString(), gameplayDataMap[biggestRankKey]);
				selectedMapSet.Add(biggestRankKey);
			}

			biggestValue = long.MinValue;
			biggestRankKey = null;
		}
	}

	private void Load()
	{
		gameplayDataMap = this.Call<Dictionary>(jsonSerializer,
				this.GetMethodLoad(), GetFilePath(), true);
	}

	private string GetFilePath()
	{
		return this.GetFilePath(rankingDataFileName, "json", rankingDataFolderName);
	}

	private void ObtainNodes()
	{
		gameplayData = GetNode(gameplayDataNP);
		jsonSerializer = GetNode(jsonSerializerNP);
	}

	private void Initialize()
	{
		this.CreateFolder(rankingDataFolderName);
		SaveNewGameplayData();

		if(rankedGameplayDataMap == null)
			UpdateRank();
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}

	public void GetGameplayRankingMap(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), rankedGameplayDataMap);
	}


	[Export]
	public NodePath gameplayDataNP;

	[Export]
	public NodePath jsonSerializerNP;

	[Export]
	public string rankingDataFolderName;

	[Export]
	public string rankingDataFileName;

	[Export]
	public Array gameplayDataKeyList;

	[Export]
	public string rankKey;

	[Export]
	public string gameMode;


	private Node gameplayData;
	private Node jsonSerializer;

	private Dictionary gameplayDataMap;
	private Dictionary rankedGameplayDataMap;
}
