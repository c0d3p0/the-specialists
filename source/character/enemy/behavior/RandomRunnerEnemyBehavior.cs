using Godot;


public class RandomRunnerEnemyBehavior : RandomWalkerEnemyBehavior
{
	public void OnRunTimerTimeout()
	{
		runCooldownTimer.Start(this.RandfRange(rng,
				runCooldownTimeRange.x, runCooldownTimeRange.y));
	}

	protected override bool TryToMove()
	{
		string animation = moveSpeedFactor < 2.0f ? "walk" : "run";
		return this.EmitSignal<bool>(this, this.GetSignalMove(),
				direction, moveSpeedFactor, animation);
	}

	private void UpdateSpeedFactor()
	{
		if(ShouldRun())
		{
			if(runTimer.IsStopped())
				runTimer.Start(this.RandfRange(rng, runTimeRange.x, runTimeRange.y));

			moveSpeedFactor = 2.0f;
		}
		else
			moveSpeedFactor = 1.0f;
	}

	private bool ShouldRun()
	{
		return !runTimer.IsStopped() || (runCooldownTimer.IsStopped() &&
				this.RandiRange(rng, 0, rngMaxValue) <= increaseSpeedRNG);
	}

	protected override void ObtainNodes()
	{
		base.ObtainNodes();
		runTimer = GetNode<Timer>(runTimerNP);
		runCooldownTimer = GetNode<Timer>(runCooldownTimerNP);
	}

	public override void _PhysicsProcess(float delta)
	{
		UpdateSpeedFactor();
		base._PhysicsProcess(delta);
	}
  

	[Export]
	public NodePath runTimerNP;

	[Export]
	public NodePath runCooldownTimerNP;

	[Export]
	public Vector2 runTimeRange = new Vector2(2f, 8f);

	[Export]
	public Vector2 runCooldownTimeRange = new Vector2(2f, 8f);

	[Export]
	public int rngMaxValue = 10000;

	[Export]
	public int increaseSpeedRNG = 310;


	private Timer runTimer;
	private Timer runCooldownTimer;

	private float moveSpeedFactor;
}
