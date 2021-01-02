using Godot;


public class DefaultProjectileInitializer : Node
{
	protected virtual void ObtainNodes()
	{
		projectile = GetNode<DefaultProjectile>(projectileNP);
		characterPhysics = GetNode<CharacterPhysics>(characterPhysicsNP);
		characterMove = GetNode<CharacterMove>(characterMoveNP);
		projectileAction = GetNode<ProjectileAction>(projectileActionNP);
		projectileBehavior = GetNode(projectileBehaviorNP);
	}

	private void InitializeCharacterMove()
	{
		characterMove.AddUserSignal(this.GetSignalIncreaseVelocity());
		characterMove.Connect(this.GetSignalIncreaseVelocity(),
				characterPhysics, this.GetMethodIncreaseVelocity());
	}

	private void InitializeProjectileAction()
	{
		projectileAction.AddUserSignal(this.GetSignalApplyMove());
		projectileAction.Connect(this.GetSignalApplyMove(),
				characterMove, this.GetMethodApplyMove());
	}

	private void InitializeProjectileBehavior()
	{
		projectileBehavior.AddUserSignal(this.GetSignalMove());
		projectileBehavior.Connect(this.GetSignalMove(),
				projectileAction, this.GetMethodMove());
	}

	
	public override void _EnterTree()
	{
		ObtainNodes();
		InitializeCharacterMove();
		InitializeProjectileAction();
		InitializeProjectileBehavior();
	}


	[Export]
	public NodePath projectileNP;

	[Export]
	public NodePath characterPhysicsNP;

	[Export]
	public NodePath characterMoveNP;

	[Export]
	public NodePath projectileActionNP;

	[Export]
	public NodePath projectileBehaviorNP;


	protected DefaultProjectile projectile;
	protected CharacterPhysics characterPhysics;
	protected CharacterMove characterMove;
	protected ProjectileAction projectileAction;
	protected Node projectileBehavior;
}