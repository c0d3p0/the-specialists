using Godot;


public class BackCheckerEnemyBehavior : WalkerEnemyBehavior
{
	protected bool TryToGoBack()
	{
		if(cooldownTimer.IsStopped() && this.RandiRange(rng, 0, rngMaxValue) <= backCheckRng)
		{
			direction *= -1;
			cooldownTimer.Start(this.RandfRange(rng, rngCooldownRange.x, rngCooldownRange.y));
			return true;
		}

		return false;
	}

	protected override void ObtainNodes()
	{
		base.ObtainNodes();
		cooldownTimer = GetNode<Timer>(cooldownTimerNP);
	}

	public override void _PhysicsProcess(float delta)
	{
		if(!TryToGoBack())
			TryToChangeDirection();
		
		TryToMove();
	}


	[Export]
	public NodePath cooldownTimerNP;

	[Export]
	public Vector2 rngCooldownRange = new Vector2(2f, 8f);

	[Export]
	public int rngMaxValue = 10000;

	[Export]
	public int backCheckRng = 250;


	private Timer cooldownTimer;
}
