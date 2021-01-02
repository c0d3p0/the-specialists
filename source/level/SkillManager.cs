using Godot;
using Godot.Collections;

using SCG = System.Collections.Generic;


public class SkillManager : Node
{
	public void IsAvailable(string type, int amount, Godot.Object optional)
	{
		Array<Spatial> projectileList = availableProjectileMap[type];
		optional.Call(this.GetMethodSet(), projectileList.Count >= amount);
	}

	public void Request(Node requester, string type, Godot.Object optional)
	{
		Array<Spatial> projectileList = availableProjectileMap[type];
		projectileDataList.Clear();

		if(projectileList.Count > 0)
		{
			Spatial projectile = projectileList[0];
			projectile.Call(this.GetMethodSetCharacter(), requester);
			projectileList.Remove(projectile);
			projectileDataList.Add(projectile);
		}

		optional.Call(this.GetMethodSet(), projectileDataList);
	}

	public void AddAsAvailable(string type, Spatial projectile, Vector3 translation)
	{
		projectile.Translation = inactiveTranslation;
		availableProjectileMap[type].Add(projectile);
	}

	public void AddEmptyBlockSlot(Vector3 translation)
	{
		levelManager.Call(this.GetMethodAddEmptyBlockSlot(), translation);
	}

	private void Initialize()
	{
		availableProjectileMap = new Dictionary<string, Array<Spatial>>();
		projectileDataList = new Array();
	}

	private void ObtainNodes()
	{
		if(levelManagerNP != null)
			levelManager = GetNode(levelManagerNP);
	}

	private void CreateSpellInstances()
	{
		Array<Spatial> projectileList;
		string currentType;
		Spatial container;	
		Spatial projectile = null;
		SCG.IEnumerator<PackedScene> it = projectilePrefabList.GetEnumerator();
		int index = 0;
		int amount;

		while(it.MoveNext())
		{
			projectileList = new Array<Spatial>();
			container = GetChild<Spatial>(index);
			amount = projectileAmountList[index++];

			for(int i = 0; i < amount; i++)
			{
				projectile = it.Current.Instance() as Spatial;
				projectile.Translation = inactiveTranslation;
				projectile.Name = this.CreateUniqueNodeName(projectile);
				projectile.Call(this.GetMethodSetManager(), this);
				projectileList.Add(projectile);
				container.CallDeferred(this.GetGDMethodAddChild(), projectile);
			}

			if(amount > 0)
			{
				currentType = this.Call<string>(projectile, this.GetMethodGetNodeType());
				availableProjectileMap.Add(currentType, projectileList);
			}
		}
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
		CreateSpellInstances();
	}


	[Export]
	public NodePath levelManagerNP;

	[Export]
	public Array<PackedScene> projectilePrefabList;

	[Export]
	public Array<int> projectileAmountList;

	[Export]
	public Vector3 inactiveTranslation = new Vector3(-19f, 10f, -14f);


	private Node levelManager;

	private Dictionary<string, Array<Spatial>> availableProjectileMap;
	private Array projectileDataList;
}
