using Godot;
using Godot.Collections;


public class BattleLevelManager : Node
{
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
	
	private void Initialize()
	{
		levelBlockManager.SoftBlockPrefabList = softBlockPrefabList;
		levelBlockManager.ItemPrefabList = itemPrefabList;
		levelBlockManager.ItemAmountRangeList = itemAmountRangeList;
		levelBlockManager.SoftBlockAmountRange = softBlockAmountRange;

		battleLevelProgress.BgmAnimationName = bgmAnimationName;
		battleLevelProgress.SpecialistCharacters = specialistCharacters;
		battleLevelProgress.LaserDeviceAirStrike = laserDeviceAirStrike;
	}

	private void ObtainNodes()
	{
		specialistCharacters = this.GetNodes<Spatial>(this, specialistCharacterNPList);
		battleLevelProgress = GetNode<BattleLevelProgress>(battleLevelProgressNP);
		levelBlockManager = GetNode<BattleLevelBlockManager>(levelBlockManagerNP);
		laserDeviceAirStrike = GetNode(laserDeviceAirStrikeNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}


	[Export]
	public NodePath laserDeviceAirStrikeNP;

	[Export]
	public NodePath battleLevelProgressNP;

	[Export]
	public NodePath levelBlockManagerNP;

	[Export]
	public Array<NodePath> specialistCharacterNPList;

	[Export]
	public Array<PackedScene> softBlockPrefabList;

	[Export]
	public Array<PackedScene> itemPrefabList;

	[Export]
	public Array<Vector2> itemAmountRangeList;

	[Export]
	public Vector2 softBlockAmountRange = new Vector2(120f, 140f);

	[Export]
	public string bgmAnimationName;


	private Node laserDeviceAirStrike;
	private Spatial[] specialistCharacters;
	private BattleLevelProgress battleLevelProgress;
	private BattleLevelBlockManager levelBlockManager;
}
