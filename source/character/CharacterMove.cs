using Godot;


public class CharacterMove : Node
{
	public void ApplyConstantMove(Vector3 direction)
	{
		constantDirection = direction;
	}

	public void ApplyMove(Vector3 desiredDirection)
	{
		ApplyMove(desiredDirection, 1f);
	}

	public void ApplyMove(Vector3 desiredDirection, float moveSpeedFactor)
	{
		velocity.y = 0;
		velocityLimit.y = desiredDirection.y * moveSpeed.y * moveSpeedFactor;
		velocityLimit.x = desiredDirection.x * moveSpeed.x * moveSpeedFactor;
		velocityLimit.z = desiredDirection.z * moveSpeed.z * moveSpeedFactor;
		float interpolateTime = GetPhysicsProcessDeltaTime() *
				GetAcceleration(desiredDirection);
		velocity = velocity.LinearInterpolate(velocityLimit, interpolateTime);
		EmitSignal(this.GetSignalIncreaseVelocity(), velocity);
	}

	private float GetAcceleration(Vector3 direction)
	{
		if(direction.LengthSquared() > 0)
			return acceleration;
		else
			return deacceleration;
	}

  public override void _PhysicsProcess(float physicsDelta)
  {
    if(constantDirection.LengthSquared() > 0)
			ApplyMove(constantDirection);
  }


  [Export]
	public Vector3 moveSpeed = new Vector3(4.2f, 0f, 4.2f);

	[Export]
	public float acceleration = 25f;

	[Export]
	public float deacceleration = 45f;


	private Vector3 constantDirection;
	private Vector3 velocity;
	private Vector3 velocityLimit;
}
