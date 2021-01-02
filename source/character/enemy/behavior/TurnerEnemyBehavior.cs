using Godot;


public class TurnerEnemyBehavior : WalkerEnemyBehavior
{
	protected override void RandomizeDirectionByFrontCollision()
	{
		if(validDirectionList.Count > 1)
		{
			byte index = (byte) this.RandiRange(rng, 1, validDirectionList.Count - 1);
			direction = body.GlobalTransform.basis.z.Round().Rotated(
					Vector3.Up, directionAngles[validDirectionList[index]]).Round();
		}
		else if(validDirectionList.Count > 0)
		{
			direction = body.GlobalTransform.basis.z.Round().Rotated(
					Vector3.Up, directionAngles[validDirectionList[0]]).Round();
		}
		else
			direction = Vector3.Zero;
	}
}
