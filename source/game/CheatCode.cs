using System.Text;
using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class CheatCode : Node
{
	public void Get(Godot.Object signalData)
	{
		signalData.Call(this.GetMethodSet(), activeCheatCodeMap);
	}

	private void HandleKeyboardInput(InputEventKey inputEventKey)
	{
		if(inputEventKey != null && inputEventKey.Pressed)
		{
			uint scancode = inputEventKey.Scancode;
				
			if(scancode == (uint) KeyList.Space)
				ToggleCheatCode();
			else if(scancode == (uint) KeyList.Delete)
				DeactivateAllCheatCodes();
			else if(scancode == (uint) KeyList.Backspace)
				ClearCheatCodeInput();
			else
				cheatCodeInput.Append((char) scancode);
		}
	}

	private void ToggleCheatCode()
	{
		string cheatCodeString = cheatCodeInput.ToString().ToLower();

		if(cheatCodeDataMap.ContainsKey(cheatCodeString))
		{
			Array data = this.cheatCodeDataMap[cheatCodeString];
			object dataKey = data[0];
			object dataValue = data[1];

			if(activeCheatCodeMap.Contains(dataKey))
			{
				GD.PushWarning("Deactivating cheat code!");
				activeCheatCodeMap.Remove(dataKey);
				target.Call(this.GetMethodOnToggleCheatCode(),
						dataKey, dataValue, false);
			}
			else
			{
				GD.PushWarning("Activating cheat code!");
				activeCheatCodeMap.Add(dataKey, dataValue);
				target.Call(this.GetMethodOnToggleCheatCode(),
						dataKey, dataValue, true);
			}

			cheatCodeInput.Clear();
		}
	}

	private void ClearCheatCodeInput()
	{
		GD.PushWarning("Clearing cheat characters typed!");
		cheatCodeInput.Clear();
	}

	private void DeactivateAllCheatCodes()
	{
		GD.PushWarning("Deactivating all cheat codes!");
		Array dataList;
		object dataKey;
		object dataValue;
		SCG.IEnumerator<SCG.KeyValuePair<string, Array>> it
				= cheatCodeDataMap.GetEnumerator();

		while(it.MoveNext())
		{
			dataList = it.Current.Value;
			dataKey = dataList[0];
			dataValue = dataList[1];
			target.Call(this.GetMethodOnToggleCheatCode(),
					dataKey, dataValue, false);
		}
	}

	private void Initialize()
	{
		target = GetNode(targetNP);
		cheatCodeInput = new StringBuilder();
		activeCheatCodeMap = new Dictionary();
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		SetProcessInput(OS.IsDebugBuild());
	}

	public override void _Input(InputEvent inputEvent)
	{
		HandleKeyboardInput(inputEvent as InputEventKey);
	}


	[Export]
	public NodePath targetNP;

	[Export]
	public Dictionary<string, Array> cheatCodeDataMap;

	
	private Dictionary activeCheatCodeMap;
	private Node target;
	private StringBuilder cheatCodeInput;
}
