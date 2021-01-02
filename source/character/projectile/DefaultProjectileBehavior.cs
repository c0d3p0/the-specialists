using Godot;


public class DefaultProjectileBehavior : Node
{
	public virtual bool Move()
	{
		EmitSignal(this.GetSignalMove(), direction);
		return true;
	}

	public override void _Ready()
	{
		SetProcess(false);
		SetPhysicsProcess(false);
	}

	public override void _PhysicsProcess(float delta)
	{
		Move();
	}

	public Vector3 Direction
	{
		set
		{
			direction = value;
		}
	}


	private Vector3 direction;
}
