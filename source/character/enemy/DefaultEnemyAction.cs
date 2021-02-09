using Godot;


public class DefaultEnemyAction : BaseCharacterAction
{
	public void Hit(Area attackerArea, Area victimArea, int damageTaken)
	{
		if(!ignoreTransition)
		{
			ignoreTransition = true;
			EmitSignal(this.GetSignalIncreaseHealth(), -damageTaken);
			character.Call(this.GetMethodSetProcessBehavior(), false);

			if(this.Call<bool>(character, this.GetMethodIsDead()))
				animationStateMachine.Travel("die");
			else
				animationStateMachine.Travel("hit");
		}
	}

	public bool Move(Vector3 direction)
	{
		return Move(direction, 1.0f, null);
	}

	public bool Move(Vector3 direction, float moveSpeedFactor,
			string animationName)
	{
		string a = CanTransitToMove();

		if(a != null)
		{
			if(direction.x != 0 || direction.z != 0)
			{
				FixBodyDirection(direction);
				EmitSignal(this.GetSignalApplyMove(), direction, moveSpeedFactor);
				animationStateMachine.Travel(animationName != null ? animationName : a);
			}
			else
			{
				EmitSignal(this.GetSignalApplyMove(), Vector3.Zero);
				animationStateMachine.Travel("idle");
			}

			return true;
		}

		return false;
	}

	public bool ExecuteSkill(Vector3 direction)
	{
		return ExecuteSkill(direction, null);
	}

	public bool ExecuteSkill(Vector3 direction, string animationName)
	{
		string a = CanTransitToSkill();

		if(a != null && !ignoreTransition && 
				this.Call<bool>(character, this.GetMethodCanExecuteSkill()))
		{
			hurtArea.Monitoring = false;
			ignoreTransition = true;
			FixBodyDirection(direction);
			animationStateMachine.Travel(animationName != null ? animationName : a);
			character.Call(this.GetMethodSetProcessBehavior(), false);
			return true;
		}

		return false;
	}

	public void OnStrikeAreaEntered(Area areaEntered, Area strikeArea)
	{
		areaEntered.EmitSignal(this.GetSignalHit(),
				strikeArea, areaEntered, damageGiven);
	}


	[Export]
	public int damageGiven = 100;
}
