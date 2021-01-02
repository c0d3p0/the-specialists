using Godot;
using Godot.Collections;


public class LevelManager : Node
{
	public void GetPositionFromRandomEmptyBlockSlot(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(),
				levelBlockManager.GetPositionFromRandomEmptyBlockSlot());
	}

	public void GetPositionFromRandomDefaultBlockSlot(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(),
				levelBlockManager.GetPositionFromRandomDefaultBlockSlot());
	}

	public void AddEmptyBlockSlot(Vector3 translation)
	{
		levelBlockManager.AddEmptyBlockSlot(translation);
	}

	public void RemoveEmptyBlockSlot(Vector3 translation)
	{
		levelBlockManager.RemoveEmptyBlockSlot(translation);
	}

	public void ContainsEmptyBlockSlot(Vector3 translation, Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(),
				levelBlockManager.ContainsEmptyBlockSlot(translation));
	}
	
	public void OnObjectiveCompleted()
	{
		levelProgress.Call(this.GetMethodOnObjectiveCompleted());
	}

	private void Initialize()
	{
		levelProgress.LevelName = levelName;
		levelProgress.BgmAnimationName = bgmAnimationName;
		levelProgress.LevelTime = levelTime;
		levelProgress.InfiniteTime = infiniteTime;
		levelProgress.SpecialistCharacters = specialistCharacters;
		levelProgress.EnemyCharacters = enemyCharacters;
		levelProgress.BossCharacters = bossCharacters;
		levelProgress.ExitGate = exitGate;

		levelNextScene.CurrentLevelScenePath = currentLevelScenePath;
		levelNextScene.NextLevelScenePath = nextLevelScenePath;
		levelNextScene.LastLevel = lastLevel;

		levelBlockManager.EnemyCharacters = enemyCharacters;
		levelBlockManager.SoftBlockPrefabList = softBlockPrefabList;
		levelBlockManager.ItemPrefabList = itemPrefabList;
		levelBlockManager.SoftBlockAmountRange = softBlockAmountRange;
		levelBlockManager.ItemAmountList = itemAmountList;
	}

	private void ObtainNodes()
	{
		specialistCharacters = this.GetNodes<Spatial>(this, specialistCharacterNPList);
		pauseScreen = GetNode(pauseScreenNP);
		exitGate = GetNode(exitGateNP);
		levelProgress = GetNode<LevelProgress>(levelProgressNP);
		levelBlockManager = GetNode<LevelBlockManager>(levelBlockManagerNP);
		levelNextScene = GetNode<LevelNextScene>(levelNextSceneNP);

		if(hasEnemy)
			enemyCharacters = this.GetNodes<Spatial>(this, enemyCharacterNPList);

		if(hasBoss)
			bossCharacters = this.GetNodes<Spatial>(this, bossCharacterNPList);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}


	[Export]
	public NodePath pauseScreenNP;

	[Export]
	public NodePath exitGateNP;

	[Export]
	public Array<NodePath> specialistCharacterNPList;

	[Export]
	public Array<NodePath> enemyCharacterNPList;

	[Export]
	public Array<NodePath> bossCharacterNPList;

	[Export]
	public NodePath levelProgressNP;

	[Export]
	public NodePath levelBlockManagerNP;

	[Export]
	public NodePath levelNextSceneNP;

	[Export]
	public string currentLevelScenePath;

	[Export]
	public string nextLevelScenePath;

	[Export]
	public Array<PackedScene> softBlockPrefabList;

	[Export]
	public Array<PackedScene> itemPrefabList;

	[Export]
	public string bgmAnimationName;

	[Export]
	public string levelName;

	[Export]
	public float levelTime;

	[Export]
	public Vector2 softBlockAmountRange = new Vector2(80f, 100f);

	[Export]
	public Array<int> itemAmountList;

	[Export]
	public bool infiniteTime;

	[Export]
	public bool hasEnemy;

	[Export]
	public bool hasBoss;

	[Export]
	public bool lastLevel;


	private Node pauseScreen;
	private Node exitGate;
	private Spatial[] specialistCharacters;
	private Spatial[] enemyCharacters;
	private Spatial[] bossCharacters;
	private LevelProgress levelProgress;
	private LevelBlockManager levelBlockManager;
	private LevelNextScene levelNextScene;	
}
