using Godot;


public class LaserAvoidWalkerEnemyBehavior : WalkerEnemyBehavior
{
	private void RandomizeDirectionBySight()
	{
		bool left = validDirectionList.Contains(LEFT_RAY);
		bool right = validDirectionList.Contains(RIGHT_RAY);

		if(left && right)	
		{
			int index = this.RandiRange(rng, LEFT_RAY, RIGHT_RAY);
			direction = body.GlobalTransform.basis.z.Round().Rotated(
					Vector3.Up, directionAngles[index]).Round();        
		}
		else if(left)
		{
			direction = body.GlobalTransform.basis.z.Round().Rotated(
					Vector3.Up, directionAngles[LEFT_RAY]).Round();
		}
		else if(right)
		{
			direction = body.GlobalTransform.basis.z.Round().Rotated(
					Vector3.Up, directionAngles[RIGHT_RAY]).Round();
		}
	}

	protected override void RandomizeDirection()
	{
		directionChangedSpot = enemyCharacter.GlobalTransform.origin;

		if(laserDeviceSpotted)
			RandomizeDirectionBySight();
		else
			RandomizeDirectionByFrontCollision();
	}

	protected override bool ShouldChangeDirection()
	{
		rayCasts[FRONT_RAY].ForceRaycastUpdate();
		bool frontCollision = rayCasts[FRONT_RAY].GetCollider() != null;

		if(!frontCollision)
		{
			if(sightRayCast.IsColliding() && this.IsLayerInMask(
					(sightRayCast.GetCollider() as PhysicsBody), laserDeviceMask))
			{
				float ls = (directionChangedSpot - enemyCharacter.GlobalTransform.origin).
						Abs().LengthSquared();

				if(ls * ls >= 1)
				{
					laserDeviceSpotted = true;
					return true;
				}
			}
		}		

		laserDeviceSpotted = false;
		return frontCollision;
	}

	protected override void ObtainNodes()
	{
		base.ObtainNodes();
		sightRayCast = GetNode<RayCast>(sightRayCastNP);
	}


	[Export]
	public NodePath sightRayCastNP;

	[Export(PropertyHint.Layers3dPhysics)]
	public uint laserDeviceMask = 16;


	private RayCast sightRayCast;
	private bool laserDeviceSpotted;
	private Vector3 directionChangedSpot;
}
