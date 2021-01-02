using Godot;


public class CharacterPhysics : Node
{
	public void IncreaseVelocity(Vector3 desiredVelocity)
	{
		this.desiredVelocity += desiredVelocity;
	}

	public void IncreaseVelocity(float x, float y, float z)
	{
		this.desiredVelocity.x += x;
		this.desiredVelocity.y += y;
		this.desiredVelocity.z += z;
	}

	public void IncreaseTranslation(Vector3 desiredTranslation)
	{
		this.desiredTranslation += desiredTranslation;
	}

	public void IncreaseTranslation(float x, float y, float z)
	{
		this.desiredTranslation.x += x;
		this.desiredTranslation.y += y;
		this.desiredTranslation.z += z;
	}

	private void FixPositionY()
	{
		if(lockYAxis && kinematicBody.Translation.y != lockYPosition)
		{
			desiredTranslation.y = lockYPosition -
					kinematicBody.GlobalTransform.origin.y;
		}
	}

	private void ExecuteMoveAndSlide()
	{
		if(desiredVelocity.LengthSquared() > 0 || forceMoveAndSlide)
		{
			velocity = kinematicBody.MoveAndSlide(desiredVelocity,
					Vector3.Up, true, 4, Mathf.Deg2Rad(0.1f), false);
			desiredVelocity = Vector3.Zero;
			forceMoveAndSlide = false;
		}
	}

	private void ExecuteMoveAndCollide()
	{
		if(desiredTranslation.LengthSquared() > 0)
		{
			kinematicBody.MoveAndCollide(desiredTranslation);
			desiredTranslation = Vector3.Zero;
		}
	}

	private void ApplyGravity(float delta)
	{
		if(gravityEnabled)
			desiredVelocity.y += velocity.y + (gravity * delta);
	}

	private void ResetCurrentVelocity()
	{
		velocity.x = 0;
		velocity.y = 0;
		velocity.z = 0;
	}

	public void SetForceMoveAndSlide(bool forceMoveAndSlide)
	{
		this.forceMoveAndSlide = forceMoveAndSlide;
	}

	public override void _PhysicsProcess(float physicsDelta)
	{
		ApplyGravity(physicsDelta);
		FixPositionY();
		ExecuteMoveAndSlide();
		ExecuteMoveAndCollide();
	}

	public override void _EnterTree()
	{
		kinematicBody = GetNode<KinematicBody>(kinematicBodyNP);
	}

	
	[Export]
	public NodePath kinematicBodyNP;

	[Export]
	public float gravity = -19.61f;

	[Export]
	public bool gravityEnabled = false;

	[Export]
	public bool lockYAxis = false;
	
	[Export]
	public float lockYPosition = 0f;


	private KinematicBody kinematicBody;

	private Vector3 desiredVelocity;
	private Vector3 desiredTranslation;
	private Vector3 velocity;
	private bool forceMoveAndSlide;
}