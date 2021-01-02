using Godot;


public class RandomWalkerEnemyBehavior : WalkerEnemyBehavior
{
	protected override bool TryToChangeDirection()
	{
		if(base.TryToChangeDirection())
		{
		  UpdateDestination();
			return true;
		}

		return false;
	}

	protected bool ReachedDestination()
	{
		if(direction.x < 0 || direction.z < 0)
			return enemyCharacter.GlobalTransform.origin.Floor() == destination;
		else
			return enemyCharacter.GlobalTransform.origin.Round() == destination;
	}

	protected void UpdateDestination()
	{
		int ba = this.RandiRange(rng, System.Convert.ToInt32(blockMoveRange.x),
				System.Convert.ToInt32(blockMoveRange.y));
		destination = (enemyCharacter.GlobalTransform.origin.Round() + 
				(direction * ba)).Round();
	}

  protected override bool ShouldChangeDirection()
  {
		return ReachedDestination() || base.ShouldChangeDirection();
  }

  protected override void Initialize()
  {
		base.Initialize();
		direction = body.GlobalTransform.basis.z.Round().Rotated(
				Vector3.Up, directionAngles[3]).Round();
		UpdateDestination();
  }

  
	[Export]
	public Vector2 blockMoveRange = new Vector2(2f, 9f);


	protected Vector3 destination;
}
