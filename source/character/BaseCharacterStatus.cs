using Godot;


public class BaseCharacterStatus : Node
{
	public virtual void IncreaseHealth(int amount)
	{
		if(!Dead)
			health = IncreaseValue(health, amount, healthRange);
	}

	public virtual void IncreaseSpeedLevel(int amount)
	{
		speedLevel = IncreaseValue(speedLevel, amount, speedLevelRange);
	}
	
	protected int IncreaseValue(int value, int amount, Vector2 range)
	{
		int v = Mathf.Max(value + amount, System.Convert.ToInt32(range.x));
		v = Mathf.Min(v, System.Convert.ToInt32(range.y));
		return v;
	}

	protected int GetValue(int value, Vector2 range)
	{
		int v = Mathf.Max(value, System.Convert.ToInt32(range.x));
		v = Mathf.Min(value, System.Convert.ToInt32(range.y));
		return v;
	}

	public virtual bool Dead
	{
		get
		{
			return health <= 0;
		}	
	}


	[Export]
	public Vector2 healthRange = new Vector2(0f, 800f);

	[Export]
	public Vector2 speedLevelRange = new Vector2(1f, 10f);


	protected int health;
	protected int speedLevel;
}
