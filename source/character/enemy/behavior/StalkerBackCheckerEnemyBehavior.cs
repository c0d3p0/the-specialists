using Godot;
using Godot.Collections;


public class StalkerBackCheckerEnemyBehavior : BackCheckerEnemyBehavior
{
	public bool TryToStalk()
	{
		if(this.RandiRange(rng, 0, 1) == 0)
			return TryToStalkFirst();
		else
			return TryToStalkSecond();
	}

	private bool TryToStalkFirst()
	{
		PhysicsBody pb;

		for(int i = 0; i < stalkRayCasts.Length; i++)
		{
			pb = stalkRayCasts[i].GetCollider() as PhysicsBody;

			if(pb != null && this.IsLayerInMask(pb, specialistLayerMask))
			{
				direction = stalkDirections[i];
				return true;
			}
		}

		return false;
	}

	private bool TryToStalkSecond()
	{
		PhysicsBody pb;

		for(int i = stalkRayCasts.Length - 1; i > 0; i--)
		{
			pb = stalkRayCasts[i].GetCollider() as PhysicsBody;

			if(pb != null && this.IsLayerInMask(pb, specialistLayerMask))
			{
				direction = stalkDirections[i];
				return true;
			}
		}

		return false;
	}

	private void UpdateStalkDirections()
	{
		Basis basis = body.GlobalTransform.basis;
		stalkDirections[0] = basis.z.Normalized().Round();
		stalkDirections[1] = basis.x.Normalized().Round();
		stalkDirections[2] = -basis.x.Normalized().Round();
		stalkDirections[3] = -basis.z.Normalized().Round();
	}

	protected override void Initialize()
	{
		base.Initialize();
		stalkDirections = new Vector3[4];
	}

	protected override void ObtainNodes()
	{
		base.ObtainNodes();
		stalkRayCasts = this.GetNodes<RayCast>(this, stalkRayCastNPList);
	}

	public override void _PhysicsProcess(float delta)
	{
		UpdateStalkDirections();

		if(!(TryToStalk() || TryToGoBack()))
			TryToChangeDirection();
		
		TryToMove();
	}


	[Export]
	public Array<NodePath> stalkRayCastNPList;

	[Export(PropertyHint.Layers3dPhysics)]
	public uint specialistLayerMask = 1;


	private RayCast[] stalkRayCasts;
	private Vector3[] stalkDirections;
}
