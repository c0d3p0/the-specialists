using Godot;
using Godot.Collections;


public class BossHUD : Node
{
	private void UpdateHealthLabel()
	{
		bossHealth = TryToGetGlobal<int>("bossHealth", 0);
		bossHealth = Mathf.RoundToInt(bossHealth * 100f / bossStartingHealth);
		healthLabel.Text = bossHealth + "%";
	}

	private void UpdateProfileTextureRect()
	{
		bossId = TryToGetGlobal<int>("bossId", 3);

		if(bossId > -1 && bossPictureList != null && bossPictureList.Count > bossId)
			profileTextureRect.Texture = bossPictureList[bossId];
	}

	private void Initialize()
	{
		bossStartingHealth = TryToGetGlobal<int>("bossHealth", 0);
		bossHealth = bossStartingHealth;
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		profileTextureRect = GetNode<TextureRect>(profileTextureRectNP);
		healthLabel = GetNode<Label>(healthLabelNP);
	}

	private T TryToGetGlobal<T>(string key, T defaultValue)
	{
		return this.TryToCall<T>(globalData, this.GetMethodGet(), defaultValue, key);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}

	public override void _Ready()
	{
		Initialize();
		UpdateProfileTextureRect();
		UpdateHealthLabel();
	}

	public override void _Process(float delta)
	{
		UpdateHealthLabel();
	}


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath profileTextureRectNP;

	[Export]
	public NodePath healthLabelNP;

	[Export]
	public Array<StreamTexture> bossPictureList;


	private Node globalData;
	private TextureRect profileTextureRect;
	private Label healthLabel;

	private int bossStartingHealth;
	private int bossHealth;
	private int bossId;
}
