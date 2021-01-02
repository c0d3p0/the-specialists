using Godot;
using Godot.Collections;


public class OptionScreen : Control
{
	public void ShowScreen(Control hideControl)
	{
		ShowScreen(hideControl, 0);
	}

	public void ShowScreen(Control hideControl, int option)
	{
		this.hideControl = hideControl;
		ShowScreen(true);
		ShowOption(option);
		currentOptionIndex = option;
	}

	public void OnOptionChangeButtonPressed(int amount)
	{
		currentOptionIndex = Mathf.Max(currentOptionIndex + amount, 0);
		currentOptionIndex = Mathf.Min(currentOptionIndex, options.Length - 1);
		ShowOption(currentOptionIndex);
	}

	public void OnSubScreenButtonPressed(string key)
	{
		if(subScreenMap.ContainsKey(key))
			subScreenMap[key].Call(this.GetMethodShowScreen(), screenControl);
	}

	public void OnNewScreenButtonPressed(string key)
	{
		if(newScreenMethodMap.ContainsKey(key))
			optionNewScreen.Call(newScreenMethodMap[key]);
	}

	private void ShowOption(int option)
	{
		optionLabel.Text = optionTitleList[option];

		for(int i = 0; i < options.Length; i++)
			options[i].Visible = i == option;
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

	private void ObtainNodes()
	{
		optionNewScreen = GetNode(optionNewScreenNP);
		screenControl = GetNode<Control>(screenControlNP);
		optionLabel = GetNode<Label>(optionLabelNP);
		options = this.GetNodes<Control>(this, optionNPList);
		subScreenMap = this.GetNodeMap<string, Control>(this, subScreenNPMap);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}

	public override void _Input(InputEvent inputEvent)
	{
		if(Visible)
			HandleExit(inputEvent as InputEventKey);
	}


	[Export]
	private NodePath optionNewScreenNP;

	[Export]
	private NodePath screenControlNP;

	[Export]
	private NodePath optionLabelNP;

	[Export]
	private Array<NodePath> optionNPList;

	[Export]
	private Dictionary<string, NodePath> subScreenNPMap;

	[Export]
	private Array<string> optionTitleList;

	[Export]
	private Dictionary<string, string> newScreenMethodMap;


	private Node optionNewScreen;
	private Control screenControl;
	private Control[] options;
	private Dictionary<string, Control> subScreenMap;
	private Label optionLabel;

	private Control hideControl;
	private int currentOptionIndex;
}
