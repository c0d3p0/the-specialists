using Godot;
using Godot.Collections;


public class CreditsScreen : Node
{
	public void StartSection()	// Called by an animation
	{
		if(sectionKey < sectionMap.Count)
		{
			UpdateCurrentSectionType();
			itemsDisplayed = 0;
			sectionLabel.Text = GetText(sectionMap, sectionKey);
			animationStateMachine.Travel("section_start");
		}
		else
			animationStateMachine.Travel("thank_you");
	}

	public void TryToEndSecion()	// Called by an animation
	{
		if(IsSectionFinished())
		{
			animationStateMachine.Travel("section_end");
			sectionKey++;
		}
		else
			GoToSectionType();
	}

	public void LoadTitleScreen()	// Called by an animation
	{
		PutGlobal("sceneToLoad", this.GetScenePath(titleScreenScenePath));
		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
	}


	private void ShowPictureItem()	// Called by an animation
	{
		if(!IsSectionFinished())
		{
			pictureTextureRect.Texture = GetTexture(itemPictureList, itemKey);
			pictureLabel.Text = GetText(itemContentMap, itemKey);
			itemsDisplayed++;
			itemKey++;
		}
	}

	private void ShowTextItem()	// Called by an animation
	{
		for(int i = 0; i < textInfoLabels.Length; i++)
		{
			if(!IsSectionFinished())
			{
				textInfoLabels[i].Text = GetText(itemInfoMap, itemKey);
				textContentLabels[i].Text = GetText(itemContentMap, itemKey);
				itemsDisplayed++;
				itemKey++;
			}
			else
			{
				textInfoLabels[i].Text = "";
				textContentLabels[i].Text = "";
			}
		}
	}

	private void ShowSingleLineItem()	// Called by an animation
	{
		if(!IsSectionFinished())
		{
			lineLabel.Text = GetText(itemContentMap, itemKey);
			itemsDisplayed++;
			itemKey++;
		}
	}

	private bool IsSectionFinished()
	{
		return !itemAmountMap.ContainsKey(sectionKey) ||
				itemsDisplayed >= itemAmountMap[sectionKey];
	}
	
	private void GoToSectionType()
	{
		if(currentSectionType == 0)
			animationStateMachine.Travel("show_line_item");
		else if(currentSectionType == 1)
			animationStateMachine.Travel("show_text_item");
		else
			animationStateMachine.Travel("show_picture_item");
	}

	private void UpdateCurrentSectionType()
	{
		currentSectionType = 0;

		if(sectionTypeMap.ContainsKey(sectionKey))
		{
			if(sectionTypeMap[sectionKey].Equals("text"))
				currentSectionType = 1;
			else if(sectionTypeMap[sectionKey].Equals("picture"))
				currentSectionType = 2;
		}
	}

	private string GetText(Dictionary<int, string> map, int key)
	{
		return map.ContainsKey(key) ?
				map[key].Replace(lineBreakCharacter, "\n") : "";
	}

	private Texture GetTexture(Array<StreamTexture> list, int index)
	{
		return list[index < 0 ? 0 : index >= list.Count ? list.Count - 1 : index];
	}

	private void HandleInput(InputEventKey inputEventKey)
	{
		if(inputEventKey != null && inputEventKey.Pressed)
		{
			uint scancode = inputEventKey.Scancode;

			if(scancode == (uint) KeyList.End)
				GetTree().Paused = !GetTree().Paused;
		}
	}

	private void ObtainPictureControlNodes()
	{
		Node pictureControl = GetNode(pictureControlNP);
		pictureTextureRect = pictureControl.GetChild<TextureRect>(0);
		pictureLabel = pictureControl.GetChild<Label>(1);
	}

	private void ObtainNodes()
	{
		ObtainPictureControlNodes();
		globalData = GetNode(globalDataNodePath);
		sectionLabel = GetNode<Label>(sectionLabelNP);
		textInfoLabels = this.GetNodes<Label>(this, textInfoLabelNPList);
		textContentLabels = this.GetNodes<Label>(this, textContentLabelNPList);
		lineLabel = GetNode<Label>(lineLabelNP);
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).Get(
				"parameters/playback") as AnimationNodeStateMachinePlayback;
	}

	private void PutGlobal(string key, object value)
	{
		globalData.Call(this.GetMethodPut(), key, value);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}

	public override void _Ready()
	{
		SetProcessInput(OS.IsDebugBuild());
	}
	public override void _Input(InputEvent inputEvent)
	{
		HandleInput(inputEvent as InputEventKey);
	}

	[Export]
	public string loadScreenScenePath = "screen/load_screen";

	[Export]
	public string titleScreenScenePath = "screen/title_screen";

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath sectionLabelNP;

	[Export]
	public NodePath pictureControlNP;

	[Export]
	public Array<NodePath> textInfoLabelNPList;

	[Export]
	public Array<NodePath> textContentLabelNPList;

	[Export]
	public NodePath lineLabelNP;

	[Export]
	public NodePath animationTreeNP;

	[Export]
	public Dictionary<int, string> sectionMap;

	[Export]
	public Dictionary<int, string> sectionTypeMap;

	[Export]
	public Array<StreamTexture> itemPictureList;

	[Export]
	private Dictionary<int, string> itemInfoMap;

	[Export]
	public Dictionary<int, string> itemContentMap;

	[Export]
	public Dictionary<int, int> itemAmountMap;

	[Export]
	public string lineBreakCharacter = "¬";


	private Node globalData;
	private Label sectionLabel;
	private Label pictureLabel;
	private TextureRect pictureTextureRect;
	private Label[] textInfoLabels;
	private Label[] textContentLabels;
	private Label lineLabel;
	protected AnimationNodeStateMachinePlayback animationStateMachine;

	private int sectionKey;
	private int itemKey;
	private int itemsDisplayed;
	private int currentSectionType;
}


