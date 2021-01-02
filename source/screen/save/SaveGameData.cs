using System.Text;
using SSC = System.Security.Cryptography;

using Godot;
using Godot.Collections;


public class SaveGameData : Node
{
	public void Load(Godot.Object optional)
	{
		Load();
		optional.Call(this.GetMethodSet(), saveGameDataMap);
	}

	public void Load()
	{
		string checkAC;
		Dictionary cdm;
		int saveIndex = 0;
		saveGameDataMap = new Dictionary();
		Dictionary dataMap = this.Call<Dictionary>(jsonSerializer,
				this.GetMethodLoad(), GetFilePath(), true);

		for(int i = 0; i < dataMap.Count; i++)
		{
			string key = i.ToString();

			if(dataMap.Contains(key))
			{
				cdm = dataMap[key] as Dictionary;

				if(cdm != null && IsValidDataMap(cdm))
				{
					checkAC = GetGameplayAC(cdm, cdm["gameMode"] as string);

					if(checkAC.Equals(cdm["ac"] as string))
						saveGameDataMap.Add(saveIndex++, cdm);
				}
			}
		}
	}

	public void Save(Dictionary dataMap)
	{
		Save(dataMap, saveGameDataMap.Count.ToString());
	}

	public void Save(Dictionary dataMap, string key)
	{
		if(dataMap != null && dataMap.Contains("gameMode"))
		{
			if(CanSave(dataMap))
			{
				string gm = dataMap["gameMode"] as string;
				dataMap.Add("ac", GetGameplayAC(dataMap, gm));
				saveGameDataMap.Add(key, dataMap);
				jsonSerializer.Call(this.GetMethodSave(), saveGameDataMap, GetFilePath());
			}
		}
	}

	private bool IsValidDataMap(Dictionary dataMap)
	{
		if(dataMap.Contains("ac") && dataMap.Contains("gameMode"))
		{
			string gm = dataMap["gameMode"] as string;

			if(gm.Equals("1P Story Mode"))
			{
				return ContainsAllKeys(dataMap, keyLists[0]) &&
						GetGameplayAC(dataMap, gm).Equals(dataMap["ac"] as string);
			}
			else
			{
				return ContainsAllKeys(dataMap, keyLists[1]) &&
						GetGameplayAC(dataMap, gm).Equals(dataMap["ac"] as string);
			}
		}

		return false;
	}

	private string GetGameplayAC(Dictionary dataMap, string gameMode)
	{
		char[] reverseTime = (dataMap["totalTime"] as string).ToCharArray();
		System.Array.Reverse(reverseTime);
		return GetSha256HashString(new string(reverseTime),
				gameMode, GetSufix(dataMap, gameMode));
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

	private string GetSufix(Dictionary dataMap, string gameMode)
	{
		if(gameMode.Equals("1P Story Mode"))
		{
			return this.CreateString(dataMap["locationIndex"],
				dataMap["p1Deaths"], dataMap["continues"]);
		}
		else if(gameMode.Equals("2P Story Mode"))
		{
			return this.CreateString(dataMap["locationIndex"],
					dataMap["p1Deaths"], dataMap["p2Deaths"], dataMap["continues"]);
		}
		
		return "";
	}

	private bool ContainsAllKeys(Dictionary dataMap, Array<string> keyList)
	{
		for(int i = 0; i < keyList.Count; i++)
		{
			if(!dataMap.Contains(keyList[i]))
				return false;
		}
		
		return true;
	}

	private bool CanSave(Dictionary dataMap)
	{
		string gm = dataMap["gameMode"] as string;
		return (gm.Equals("1P Story Mode") &&
				ContainsAllKeys(dataMap, keyLists[0])) ||
				(gm.Equals("2P Story Mode") &&
				ContainsAllKeys(dataMap, keyLists[1]));
	}

	private string GetFilePath()
	{
		return this.GetFilePath(saveGameFileName, "json", saveGameFolderName);
	}

	private void Initialize()
	{
		keyLists = new Array<Array<string>>();
		Array<string> aKeyList = new Array<string>();
		Array<string> bKeyList = new Array<string>();
		string[] keys = new string[]{
				"totalTime", "locationIndex", "p1Deaths",
				"continues", "gameMode", "p2Deaths"};

		for(int i = 0; i < keys.Length - 1; i++)
		{
			aKeyList.Add(keys[i]);
			bKeyList.Add(keys[i]);
		}

		bKeyList.Add(keys[keys.Length - 1]);
		keyLists.Add(aKeyList);
		keyLists.Add(bKeyList);
	}

	private void ObtainNodes()
	{
		jsonSerializer = GetNode(jsonSerializerNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}


	[Export]
	public NodePath jsonSerializerNP;

	[Export]
	public string saveGameFolderName;

	[Export]
	public string saveGameFileName;


	private Node jsonSerializer;

	private Dictionary saveGameDataMap;
	private Array<Array<string>> keyLists;
}
