using Godot;
using Godot.Collections;


public class SpecialistBattleHUD : Control
{
	private void UpdateGUI()
	{
		dataControl.Visible = inBattle;
		notInBattleControl.Visible = !inBattle;
		winLabel.Text = specialistWins.ToString();
		profileTextureRect.Texture = specialistTextureList[specialistColor];
	}

	private void ObtainData()
	{
		inBattle = specialistIndex < GetGlobal<int>("specialistAmount");
		specialistColor = GetGlobal<int>("colorSpecialistIndex" + specialistIndex);
		specialistWins = GetGlobal<int>("winsSpecialistIndex" + specialistIndex);
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		dataControl = GetNode<Control>(dataControlNP);
		notInBattleControl = GetNode<Control>(notInBattleControlNP);
		profileTextureRect = GetNode<TextureRect>(profileTextureRectNP);
		winLabel = GetNode<Label>(winLabelNP);
	}

	private T GetGlobal<T>(string key)
	{
		return this.Call<T>(globalData, this.GetMethodGet(), key);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		ObtainData();
		UpdateGUI();
	}


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath dataControlNP;

	[Export]
	public NodePath notInBattleControlNP;

	[Export]
	public NodePath profileTextureRectNP;

	[Export]
	public NodePath winLabelNP;

	[Export]
	public Array<StreamTexture> specialistTextureList;

	[Export]
	public int specialistIndex;


	private Node globalData;
	private Control dataControl;
	private Control notInBattleControl;
	private TextureRect profileTextureRect;
	private Label winLabel;

	private bool inBattle;
	private int specialistColor;
	private int specialistWins;
}
