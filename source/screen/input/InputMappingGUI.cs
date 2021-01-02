using SC = System.Collections;
using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class InputMappingGUI : Node
{
	public void ShowScreen(Control hideControl)
	{
		this.hideControl = hideControl;
		this.CreateFolder(inputMappingFolderName);
		LoadConfiguredMappings();
		playerId = 1;
		playerIdLabel.Text = this.CreateString("Player ", playerId);
		OnDeviceTypeButtonPressed(true);
		currentInputName = null;
		ShowScreen(true);
	}

	public void OnPlayerIdButtonPressed(int shift)
	{
		int newId = playerId + shift;
		newId = newId < 1 ? 1 : newId > playerAmount ? playerAmount : newId;
		playerId = (byte) newId;
		playerIdLabel.Text = this.CreateString("Player ", playerId);
		UpdateCurrentInputMapping();
		UpdateButtonsTextToCurrentMapping();
	}

	public void OnDeviceTypeButtonPressed(bool keyboard)
	{
		keyboardMappingActive = keyboard;
		UpdateCurrentInputMapping();
		UpdateButtonsTextToCurrentMapping();
		deviceTypeLabel.Text = keyboardMappingActive ? "Keyboard" : "Controller";
	}

	public void OnInputButtonPressed(string inputName)
	{
		if(currentInputName != null)
			SetButtonText(currentInputName);

		currentInputName = inputName;
		buttonMap[currentInputName].Text = "Press an input";
	}

	public void OnSaveButtonPressed()
	{
		EmitSignal(this.GetSignalSave(), keyboardInputMapping,
				GetFilePath(keyboardMappingFileName));
		EmitSignal(this.GetSignalSave(), controllerInputMapping,
				GetFilePath(controllerMappingFileName));
		ShowScreen(false);
		ApplyCurrentMappings();
	}

	public void OnIgnoreButtonPressed()
	{
		ShowScreen(false);
		keyboardInputMapping.Dispose();
		controllerInputMapping.Dispose();
	}

	private void AddControllerInputsToGameMapping()
	{
		string playerPrefix;
		string action;
		InputEventJoypadMotion iejm;
		InputEventJoypadButton iejb;
		SC.IDictionaryEnumerator it = controllerInputMapping.GetEnumerator();
		Dictionary mapping;

		for(int i = 1; i <= playerAmount; i++)
		{
			mapping = controllerInputMapping[i.ToString()] as Dictionary;

			if(mapping != null && mapping.Contains("deviceId"))
			{
				playerPrefix = this.CreateString("p", i, "_");
				int deviceId = int.Parse(mapping["deviceId"] as string);
				it = mapping.GetEnumerator();

				while(it.MoveNext())
				{
					if(!it.Entry.Key.Equals("deviceId"))
					{
						action = this.CreateString(playerPrefix, it.Entry.Key);
						string inputValue = it.Entry.Value as string;

						if(IsDirectionInput(inputValue))
						{
							iejm = new InputEventJoypadMotion();
							float[] axisData = ConvertInputcodeToJoystickAxis(inputValue);
							iejm.Axis = System.Convert.ToInt32(axisData[0]);
							iejm.AxisValue = axisData[1];
							iejm.Device = deviceId;
							InputMap.ActionAddEvent(action, iejm);
						}
						else
						{
							iejb = new InputEventJoypadButton();
							iejb.ButtonIndex = int.Parse(it.Entry.Value as string);
							iejb.Device = deviceId;
							InputMap.ActionAddEvent(action, iejb);
						}
					}
				}
			}
		}
	}

	private void AddKeyboardInputsToGameMapping()
	{
		string playerPrefix;
		string action;
		InputEventKey iek;
		SC.IDictionaryEnumerator it;
		Dictionary mapping;

		for(int i = 1; i <= playerAmount; i++)
		{
			playerPrefix = this.CreateString("p", i, "_");
			mapping = keyboardInputMapping[i.ToString()] as Dictionary;

			if(mapping != null)
			{
				it = mapping.GetEnumerator();

				while(it.MoveNext())
				{
					action = this.CreateString(playerPrefix, it.Entry.Key);
					InputMap.ActionEraseEvents(action);
					iek = new InputEventKey();
					iek.Scancode = uint.Parse(it.Entry.Value as string);
					InputMap.ActionAddEvent(action, iek);
				}
			}
		}
	}

	private void ConfigureInputKey(InputEventKey inputEventKey)
	{
		if(inputEventKey != null && inputEventKey.Pressed && 
				currentInputName != null && keyboardMappingActive)
		{
			uint keycode = inputEventKey.Scancode;

			if(!ignoreKeyMap.ContainsKey((int) keycode))
				AddInput(currentInputName, keycode.ToString());

			SetButtonText(currentInputName);
			currentInputName = null;
		}	
	}

	private void ConfigureInputJoypadButton(InputEventJoypadButton inputEventJoyButton)
	{
		if(inputEventJoyButton != null && inputEventJoyButton.Pressed && 
				currentInputName != null && !keyboardMappingActive)
		{
			int buttonIndex = (int) inputEventJoyButton.ButtonIndex;
			AddInput(currentInputName, buttonIndex.ToString());
			AddInput("deviceId", inputEventJoyButton.Device);
			SetButtonText(currentInputName);
			currentInputName = null;
			GetTree().SetInputAsHandled();
		}	
	}

	private void ConfigureInputJoypadMotion(InputEventJoypadMotion inputEventJoyMotion)
	{
		if(inputEventJoyMotion != null && inputEventJoyMotion.AxisValue != 0 && 
				currentInputName != null && !keyboardMappingActive)
		{
			string inputcode = ConvertJoystickAxisInputcode(inputEventJoyMotion.Axis,
					inputEventJoyMotion.AxisValue);
			AddInput(currentInputName, inputcode);
			AddInput("deviceId", inputEventJoyMotion.Device);
			SetButtonText(currentInputName);
			currentInputName = null;
		}	
	}

	private void UpdateButtonsTextToCurrentMapping()
	{
		SCG.IEnumerator<SCG.KeyValuePair<object, Button>> it =
				buttonMap.GetEnumerator();

		while(it.MoveNext())
			SetButtonText(it.Current.Key);
	}
	
	private void SetButtonText(object buttonKey)
	{
		if(currentInputMapping.Contains(buttonKey))
		{
			object inputValue = currentInputMapping[buttonKey];

			if(!keyboardMappingActive)
			{
				if(IsDirectionInput(inputValue))
				{
					float[] axisData = ConvertInputcodeToJoystickAxis(inputValue);
					buttonMap[buttonKey].Text = "Axis " + axisData[0] + ", " + axisData[1];
				}
				else
					buttonMap[buttonKey].Text = "Button " + inputValue;
			}
			else
			{
				buttonMap[buttonKey].Text =
						((KeyList) uint.Parse(inputValue as string)).ToString();
			}
		}
		else
			buttonMap[buttonKey].Text = unmappedText;
	}

	private void FixInputMappings()
	{
		for(int i = 1; i <= playerAmount; i++)
		{
			if(!keyboardInputMapping.Contains(i.ToString()))
				keyboardInputMapping.Add(i.ToString(), new Dictionary());

			if(!controllerInputMapping.Contains(i.ToString()))
				controllerInputMapping.Add(i.ToString(), new Dictionary());
		}	
	}

	private void UpdateCurrentInputMapping()
	{
		if(keyboardMappingActive)
			currentInputMapping = keyboardInputMapping[playerId.ToString()] as Dictionary;
		else
			currentInputMapping = controllerInputMapping[playerId.ToString()] as Dictionary;
	}

	private void AddInput(object inputName, object value)
	{
		if(currentInputMapping.Contains(inputName))
			currentInputMapping[inputName] = value.ToString();
		else
			currentInputMapping.Add(inputName, value.ToString());
	}

	private void ApplyCurrentMappings()
	{
		AddKeyboardInputsToGameMapping();
		AddControllerInputsToGameMapping();
		keyboardInputMapping.Dispose();
		controllerInputMapping.Dispose();
	}

	private void LoadConfiguredMappings()
	{
		keyboardInputMapping = this.EmitSignal<Dictionary>(this,
				this.GetSignalLoad(), GetFilePath(keyboardMappingFileName), true);
		controllerInputMapping = this.EmitSignal<Dictionary>(this,
				this.GetSignalLoad(), GetFilePath(controllerMappingFileName), true);	
		FixInputMappings();
	}

	private void ShowScreen(bool show)
	{
		hideControl.Visible = !show;
		inputMappingScreen.ShowBehindParent = !show;
		inputMappingScreen.Visible = show;
		this.SetProcessInput(show);
	}

	private string ConvertJoystickAxisInputcode(int axis, float axisValue)
	{
		return axis + "_" + axisValue;
	}

	private float[] ConvertInputcodeToJoystickAxis(object value)
	{
		string[] data = (value as string).Split("_");
		int axis = int.Parse(data[0]);
		float axisValue = float.Parse(data[1]);
		return new float[]{axis, axisValue};
	}

	private bool IsDirectionInput(object value)
	{
		return (value as string).Contains("_");
	}

	private string GetFilePath(string fileName)
	{
		return this.GetFilePath(fileName, "json", inputMappingFolderName);
	}
	
	private void Initialize()
	{
		LoadConfiguredMappings();
		ApplyCurrentMappings();
	}

	private void ObtainNodes()
	{
		inputMappingScreen = GetNode<Control>(inputMappingScreenNP);
		buttonMap = this.GetNodeMap<object, Button>(this, buttonNPMap);
		playerIdLabel = GetNode<Label>(playerIdLabelNP);
		deviceTypeLabel = GetNode<Label>(deviceTypeLabelNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}

	public override void _Ready()
	{
		Initialize();
		this.SetProcessInput(false);
	}

	public override void _Input(InputEvent inputEvent)
	{
		ConfigureInputKey(inputEvent as InputEventKey);
		ConfigureInputJoypadMotion(inputEvent as InputEventJoypadMotion);
		ConfigureInputJoypadButton(inputEvent as InputEventJoypadButton);
	}


	[Export]
	public NodePath inputMappingScreenNP;

	[Export]
	public NodePath playerIdLabelNP;

	[Export]
	public NodePath deviceTypeLabelNP;

	[Export]
	public Dictionary<object, NodePath> buttonNPMap;

	[Export]
	public Dictionary<int, object> ignoreKeyMap;

	[Export]
	public string unmappedText = "None";

	[Export]
	public int playerAmount = 4;

	[Export]
	public string inputMappingFolderName;

	[Export]
	public string keyboardMappingFileName;

	[Export]
	public string controllerMappingFileName;


	private Control inputMappingScreen;
	private Control hideControl;
	private Label playerIdLabel;
	private Label deviceTypeLabel;
	private Dictionary<object, Button> buttonMap;

	private byte playerId;
	private bool keyboardMappingActive;
	private Dictionary keyboardInputMapping;
	private Dictionary controllerInputMapping;
	private Dictionary currentInputMapping;
	private string currentInputName;
}
