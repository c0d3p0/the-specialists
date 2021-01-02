using Godot;


public class DefaultEnemyStatus : BaseCharacterStatus
{
	public override void _EnterTree()
	{
		health = IncreaseValue(0, startingHealth, healthRange);
	}


	[Export]
	public int startingHealth = 200;
}
