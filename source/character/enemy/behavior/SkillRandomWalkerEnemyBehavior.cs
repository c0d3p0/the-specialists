using Godot;


public class SkillRandomWalkerEnemyBehavior : RandomWalkerEnemyBehavior
{
	private bool TryToExecuteSkill()
	{
		if(ShouldExecuteSkill())
		{
			if(this.EmitSignal<bool>(this, this.GetSignalExecuteSkill(), direction))
			{
				RandomizeCooldownWaitTime();
				return true;
			}
		}

		return false;
	}

	private bool ShouldExecuteSkill()
	{
		return skillCooldownTimer.IsStopped() && 
				this.RandiRange(rng, 0, rngMaxValue) <= skillRNG;
	}

	private void RandomizeCooldownWaitTime()
	{
		skillCooldownTimer.WaitTime = this.RandiRange(rng,
				System.Convert.ToInt32(skillCooldownTimeRange.x),
				System.Convert.ToInt32(skillCooldownTimeRange.y));
	}

	protected override void ObtainNodes()
	{
		base.ObtainNodes();
		skillCooldownTimer = GetNode<Timer>(skillCooldownTimerNP);
	}

  public override void _PhysicsProcess(float delta)
  {
		if(!TryToExecuteSkill())
		{
			TryToChangeDirection();
			TryToMove();
		}
  }


	[Export]
	public NodePath skillCooldownTimerNP;

	[Export]
	public Vector2 skillCooldownTimeRange = new Vector2(4f, 8f);

	[Export]
	public int rngMaxValue = 10000;

	[Export]
	public int skillRNG = 400;


	private Timer skillCooldownTimer;

}
