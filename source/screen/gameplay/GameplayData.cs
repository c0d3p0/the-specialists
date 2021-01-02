using System.Text;
using SSC = System.Security.Cryptography;

using Godot;
using Godot.Collections;


public class GameplayData : Node
{
	public void GetLastGameplayDataMap(Godot.Object optional)
	{
		Dictionary dataMap = this.Call<Dictionary>(jsonSerializer,
				this.GetMethodLoad(), GetFilePath(), true);

		if(dataMap.Contains("gameMode") && dataMap.Contains("ac"))
		{
			string checkAC = GetGameplayAC(dataMap, dataMap["gameMode"] as string);

			if(checkAC.Equals(dataMap["ac"] as string))
				optional.Call(this.GetMethodSet(), dataMap);
		}
	}

	public void GetGameplayDataMap(string gameMode, Godot.Object optional)
	{
		Dictionary dataMap = Get1PGameplayDataMap(gameMode);

		if(dataMap == null)
			dataMap = Get2PGameplayDataMap(gameMode);

		if(dataMap != null)
			optional.Call(this.GetMethodSet(), dataMap);
	}

	public void IsValidDataMap(Dictionary dataMap,
			Array keyList, string gameMode, Godot.Object optional)
	{
		if(ContainsAllKeys(dataMap, keyList))
		{
			optional.Call(this.GetMethodSet(),
					GetGameplayAC(dataMap, gameMode).Equals(dataMap["ac"] as string));
		}
	}

	public void Save(Dictionary gameplayDataMap)
	{
		jsonSerializer.Call(this.GetMethodSave(), gameplayDataMap, GetFilePath());
	}

	public void Clear()
	{
		globalData.Call(this.GetMethodRemove(), "saveGameplayData");
		globalData.Call(this.GetMethodRemove(), "gameMode");
	}

	private Dictionary Get1PGameplayDataMap(string gameMode)
	{
		if(gameMode.Equals("1P Story Mode") && IsValidGameplayData(gameMode))
		{
			Dictionary dataMap = new Dictionary();
			
			dataMap.Add("totalTime", GetGlobal<string>("totalTime"));
			dataMap.Add("p1Deaths", GetGlobal<int>("p1Deaths").ToString());
			dataMap.Add("continues", GetGlobal<int>("continues").ToString());
			long score = CalculateScore(dataMap);
			dataMap.Add("score", score.ToString());
			dataMap.Add("grade", GetGrade(dataMap, score));
			dataMap.Add("ac", GetGameplayAC(dataMap, gameMode));
			return dataMap;
		}

		return null;
	}

	private Dictionary Get2PGameplayDataMap(string gameMode)
	{
		if(gameMode.Equals("2P Story Mode") && IsValidGameplayData(gameMode))
		{
			Dictionary dataMap = new Dictionary();
			dataMap.Add("totalTime", GetGlobal<string>("totalTime"));
			dataMap.Add("p1Deaths", GetGlobal<int>("p1Deaths").ToString());
			dataMap.Add("p2Deaths", GetGlobal<int>("p2Deaths").ToString());
			dataMap.Add("continues", GetGlobal<int>("continues").ToString());
			long score = CalculateScore(dataMap);
			dataMap.Add("score", score.ToString());
			dataMap.Add("grade", GetGrade(dataMap, score));
			dataMap.Add("ac", GetGameplayAC(dataMap, gameMode));
			return dataMap;
		}

		return null;
	}

	private string GetGrade(Dictionary dataMap, long score)
	{
		string[] grades = new string[] {"S", "A", "B", "C"};

		for(int i = 0; i < rankScores.Length; i++)
		{
			if(score >= rankScores[i])
				return grades[i];
		}

		return grades[grades.Length - 1];
	}

	private long CalculateScore(Dictionary dataMap)
	{
		long totalTime = System.Int64.Parse(dataMap["totalTime"] as string);
		int p1Deaths = System.Int32.Parse(dataMap["p1Deaths"] as string);
		int continues = System.Int32.Parse(dataMap["continues"] as string);
		int p2Deaths = dataMap.Contains("p2Deaths") ?
				System.Convert.ToInt32(dataMap["p2Deaths"] as string) : 0;
		long score = long.MaxValue - totalTime;

		// Half an hour decrease in the score if the players died.
		// One hour if the players continued.
		score -= p1Deaths > 0 || p2Deaths > 0 || continues > 0 ? 1800000 : 0;
		score -= continues > 0 ? 1800000 : 0;
		return score > -1 ? score : 0;
	}

	private string GetGameplayAC(Dictionary dataMap, string gameMode)
	{
		char[] reverseTime = (dataMap["totalTime"] as string).ToCharArray();
		System.Array.Reverse(reverseTime);
		return GetSha256HashString(new string(reverseTime),
				dataMap["score"] as string, GetSufix(dataMap, gameMode));
	}

	private string GetSha256HashString(string prefix, string data, string sufix)  
	{  
		using(SSC.SHA256 sha256Hash = SSC.SHA256.Create())  
		{ 
			StringBuilder hs = new StringBuilder(prefix).Append(data).Append(sufix);
			byte[] bytes = sha256Hash.ComputeHash(
					Encoding.UTF8.GetBytes(hs.ToString()));  
			hs = new StringBuilder();

			for(int i = 0; i < bytes.Length; i++)   
				hs.Append(bytes[i].ToString("x2"));  

			return hs.ToString();
		}
	}

	private string GetSha256HashString(string[] hashDataList)  
	{  
		return GetSha256HashString(hashDataList[0] as string,
				hashDataList[1] as string, hashDataList[2] as string);
	}
	
	private bool IsValidGameplayData(string gameMode)
	{
		if(GetGlobal<bool>("saveGameplayData"))
		{
			string cgm = GetGlobal<string>("gameMode");
			return cgm != null && gameMode.Equals(cgm);
		}

		return false;
	}

	private bool ContainsAllKeys(Dictionary dataMap, Array keyList)
	{
		for(int i = 0; i < keyList.Count; i++)
		{
			if(!dataMap.Contains(keyList[i]))
				return false;
		}
		
		return true;
	}

	private string GetSufix(Dictionary dataMap, string gameMode)
	{
		if(gameMode.Equals("1P Story Mode"))
		{
			return this.CreateString(dataMap["p1Deaths"],
				dataMap["continues"], dataMap["grade"]);
		}
		else if(gameMode.Equals("2P Story Mode"))
		{
			return this.CreateString(dataMap["p1Deaths"], dataMap["p2Deaths"],
					dataMap["continues"], dataMap["grade"]);
		}
		
		return "";
	}
	
	private string GetFilePath()
	{
		return this.GetFilePath(lastGameplayFileName, "json",
				lastGameplayFolderName);
	}

	private void Initialize()
	{
		rankScores = new long[]{9223372036849375807,
				9223372036846375807, 9223372036842775807, 0};
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		jsonSerializer = GetNode(jsonSerializerNP);
	}
	
	// NOTE: Rank test methods
	private void CreateMock1PGameplayData()
	{
		int deaths = 0;
		int continues = 0;
		globalData.Call(this.GetMethodPut(), "optionSection", 1);
		globalData.Call(this.GetMethodPut(), "saveGameplayData", true);
		globalData.Call(this.GetMethodPut(), "gameMode", "1P Story Mode");
		globalData.Call(this.GetMethodPut(), "p1Deaths", deaths);
		globalData.Call(this.GetMethodPut(), "continues", continues);
		globalData.Call(this.GetMethodPut(), "totalTime", "5399999");
	}

	private void CreateMock2PGameplayData()
	{
		int deaths = 0;
		int continues = 0;
		globalData.Call(this.GetMethodPut(), "optionSection", 1);
		globalData.Call(this.GetMethodPut(), "saveGameplayData", true);
		globalData.Call(this.GetMethodPut(), "gameMode", "2P Story Mode");
		globalData.Call(this.GetMethodPut(), "p1Deaths", deaths);
		globalData.Call(this.GetMethodPut(), "p2Deaths", deaths);
		globalData.Call(this.GetMethodPut(), "continues", continues);
		globalData.Call(this.GetMethodPut(), "totalTime", "5399999");
	}
	// NOTE: Rank test methods
	
	private T GetGlobal<T>(string key)
	{
		return this.Call<T>(globalData, this.GetMethodGet(), key);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
		// CreateMock2PGameplayData();
	}


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath jsonSerializerNP;

	[Export]
	public string lastGameplayFolderName;

	[Export]
	public string lastGameplayFileName;


	private Node globalData;
	private Node jsonSerializer;
	private long[] rankScores;
}
