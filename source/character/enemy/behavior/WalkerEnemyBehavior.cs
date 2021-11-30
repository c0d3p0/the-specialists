using Godot;
using Godot.Collections;


public class WalkerEnemyBehavior : Node
{
	protected virtual bool TryToMove()
	{
		return this.EmitSignal<bool>(this, this.GetSignalMove(), direction);
	}

	protected virtual bool TryToChangeDirection()
	{
			if(ShouldChangeDirection())
			{
				ChangeDirection();
				return true;
			}
			else if(direction.LengthSquared() <= 0.0f)
			{
				direction = enemyCharacter.GlobalTransform.basis.z.Round();
				return true;
			}

			return false;
	}

	protected virtual void ChangeDirection()
	{
		validDirectionList.Clear();
			
		for(int i = BACK_RAY; i < rayCasts.Length; i++)
		{
			rayCasts[i].ForceRaycastUpdate();

			if(rayCasts[i].GetCollider() == null)
				validDirectionList.Add(i);
		}

		RandomizeDirection();
	}
	
	protected virtual void RandomizeDirectionByFrontCollision()
	{
		if(validDirectionList.Count > 0)
		{
			int index = this.RandiRange(rng, 0, validDirectionList.Count - 1);
			direction = body.GlobalTransform.basis.z.Round().Rotated(
					Vector3.Up, directionAngles[validDirectionList[index]]).Round();
		}
		else
			direction = Vector3.Zero;
	}

	protected virtual void RandomizeDirection()
	{
		RandomizeDirectionByFrontCollision();
	}

	protected virtual bool ShouldChangeDirection()
	{
		rayCasts[FRONT_RAY].ForceRaycastUpdate();
		return rayCasts[FRONT_RAY].GetCollider() != null;
	}

	protected virtual void Initialize()
	{
		validDirectionList = new HashList<int>(5);
		rng = new RandomNumberGenerator();
		directionAngles = new float[]{Mathf.Deg2Rad(0f),
				Mathf.Deg2Rad(180f), Mathf.Deg2Rad(90f), Mathf.Deg2Rad(-90f)};
		int index = this.RandiRange(rng, 0, directionAngles.Length - 1);
		direction = body.GlobalTransform.basis.z.Round().Rotated(
				Vector3.Up, directionAngles[index]).Round();
	}

	protected void AddExceptionToRayCasts()
	{
		for(int i = 0; i < rayCasts.Length; i++)
			rayCasts[i].AddException(enemyCharacter);
	}

	protected virtual void ObtainNodes()
	{
		enemyCharacter = GetNode<KinematicBody>(enemyCharacterNP);
		body = GetNode<Spatial>(bodyNP);
		rayCasts = this.GetNodes<RayCast>(this, rayCastListNP);
	}

  public override void _EnterTree()
  {
		ObtainNodes();
		AddExceptionToRayCasts();
		Initialize();
  }

  public override void _PhysicsProcess(float delta)
  {
		TryToChangeDirection();
		TryToMove();
  }

	public override void _Ready()
	{
		SetProcess(false);
		SetPhysicsProcess(false);
	}
	

	[Export]
	public NodePath enemyCharacterNP;

	[Export]
	public NodePath bodyNP;

	[Export]
	public Array<NodePath> rayCastListNP;

	protected KinematicBody enemyCharacter;
	protected Spatial body;
	protected RayCast[] rayCasts;

	protected Vector3 direction;
	protected float[] directionAngles;
	protected HashList<int> validDirectionList;
	protected RandomNumberGenerator rng;

	protected const int FRONT_RAY = 0;
	protected const int BACK_RAY = 1;
	protected const int LEFT_RAY = 2;
	protected const int RIGHT_RAY = 3;
}
