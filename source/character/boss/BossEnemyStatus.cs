using Godot;


public class BossEnemyStatus : DefaultEnemyStatus
{
	public override void IncreaseHealth(int amount)
	{
		base.IncreaseHealth(amount);
		PutGlobal("bossHealth", health);
	}

	private void Initialize()
	{
		health = IncreaseValue(0, startingHealth, healthRange);
		PutGlobal("bossHealth", health);
		PutGlobal("bossId", bossId);
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
	}

	private void PutGlobal(string key, object value)
	{
		globalData.Call(this.GetMethodPut(), key, value);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}


	[Export]
	public int bossId = 0;

	[Export]
	public string globalDataNodePath = "/root/GlobalData";


	private Node globalData;
}
