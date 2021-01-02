using Godot;
using Godot.Collections;


public class Boss4Skill : BaseSkill
{
	public override bool CanExecuteSkill()
	{
		FetchAvailableProjectile();
		RequestProjectiles();
		return projectileList.Count > 0;
	}

	public override void ExecuteSkill()
	{
		if(projectileList.Count > 0)
		{
			Spatial projectile = projectileList[0];
			projectile.Call(this.GetMethodPlant());
			string type = this.Call<string>(projectile, this.GetMethodGetNodeType());
			TryToRemoveEmptyBlockSlot(projectile, type);
			TryToMoveProjectile(projectile, type);
			projectileList.RemoveAt(0);
		}
	}

	private void FetchAvailableProjectile()
	{
		string type;
		availableProjectileTypeList.Clear();

		for(int i = 0; i < projectileTypeList.Count; i++)
		{
			type = projectileTypeList[i];
			
			if(this.Call<bool>(skillManager, this.GetMethodIsAvailable(),
					type, projectileTypeAmountMap[type]))
			{
				availableProjectileTypeList.Add(type);
			}
		}
	}

	private void RequestProjectiles()
	{
		projectileList.Clear();
		
		if(availableProjectileTypeList.Count > 0)
		{
			int i = this.RandiRange(rng, 0, availableProjectileTypeList.Count - 1);
			string type = availableProjectileTypeList[i];
			int amount = projectileTypeAmountMap[type];
			Vector2 levelRange = projectileTypeLevelRangeMap[type];
			Spatial projectile;

			for(int j = 0; j < amount; j++)
			{
				projectile = this.Call<Array>(skillManager,
						this.GetMethodRequest(), character, type)[0] as Spatial;
				projectile.Call(this.GetMethodSetLaserRayLevel(),
						this.RandiRange(rng, System.Convert.ToInt32(levelRange.x),
						System.Convert.ToInt32(levelRange.y)));
				projectile.Translation = GetProjectilePosition(type);
				projectile.Call(this.GetMethodSetCharacter(), character);
				projectileList.Add(projectile);
			}
		}
	}

	private Vector3 GetProjectilePosition(string type)
	{
		if(removeEmptyBlockProjectileTypeMap.ContainsKey(type))
		{
			return this.Call<Vector3>(levelManager,
					this.GetMethodGetPositionFromRandomEmptyBlockSlot()) +
					projectileTypeOffsetMap[type];
		}
		
		return this.Call<Vector3>(levelManager,
					this.GetMethodGetPositionFromRandomDefaultBlockSlot()) +
					projectileTypeOffsetMap[type];
	}

	private void TryToRemoveEmptyBlockSlot(Spatial projectile, string type)
	{
		if(removeEmptyBlockProjectileTypeMap.ContainsKey(type))
		{
			levelManager.Call(this.GetMethodRemoveEmptyBlockSlot(),
					projectile.Translation);
		}
	}

	private void TryToMoveProjectile(Spatial projectile, string type)
	{
		if(projectileTypeMoveDirectionMap.ContainsKey(type))
		{
			Vector3 direction = projectileTypeMoveDirectionMap[type];
			projectile.Call(this.GetMethodMove(), skillManager, direction);
		}
	}

	private void Initialize()
	{
		rng = new RandomNumberGenerator();
		availableProjectileTypeList = new HashList<string>();
		projectileList = new Array<Spatial>();
	}

	private void ObtainNodes()
	{
		character = GetNode<Spatial>(characterNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}
	

	[Export]
	public NodePath characterNP;

	[Export]
	public Array<string> projectileTypeList;

	[Export]
	public Dictionary<string, int> projectileTypeAmountMap;

	[Export]
	public Dictionary<string, Vector3> projectileTypeOffsetMap;

	[Export]
	public Dictionary<string, bool> removeEmptyBlockProjectileTypeMap;

	[Export]
	public Dictionary<string, Vector2> projectileTypeLevelRangeMap;

	[Export]
	public Dictionary<string, Vector3> projectileTypeMoveDirectionMap;


	private Spatial character;

	private RandomNumberGenerator rng;
	private HashList<string> availableProjectileTypeList;
	private Array<Spatial> projectileList;
}
