using Godot;


public class DefaultLaserDeviceInitializer : Node
{
	protected virtual void ObtainNodes()
	{
		laserDevice = GetNode(laserDeviceNP);
		characterPhysics = GetNode(characterPhysicsNP);
		characterMove = GetNode(characterMoveNP);
		laserDeviceAction = GetNode(laserDeviceActionNP);
		hurtArea = GetNode(hurtAreaNP);
	}

	private void InitializeCharacterMove()
	{
		characterMove.AddUserSignal(this.GetSignalIncreaseVelocity());
		characterMove.Connect(this.GetSignalIncreaseVelocity(),
				characterPhysics, this.GetMethodIncreaseVelocity());
	}

	private void InitializeLaserDeviceAction()
	{
		laserDeviceAction.AddUserSignal(this.GetSignalMove());
		laserDeviceAction.Connect(this.GetSignalMove(),
				characterMove, this.GetMethodApplyConstantMove());
	}

	private void InitializeHurtArea()
	{
		hurtArea.AddUserSignal(this.GetSignalMove());
		hurtArea.Connect(this.GetSignalMove(), laserDevice,
				this.GetMethodMove());
	}

  public override void _EnterTree()
  {
    ObtainNodes();
		InitializeCharacterMove();
		InitializeLaserDeviceAction();
		InitializeHurtArea();
  }


	[Export]
	public NodePath laserDeviceNP;

	[Export]
	public NodePath characterPhysicsNP;

	[Export]
	public NodePath characterMoveNP;

	[Export]
	public NodePath laserDeviceActionNP;

	[Export]
	public NodePath hurtAreaNP;


	protected Node laserDevice;
	protected Node characterMove;
	protected Node characterPhysics;
	protected Node laserDeviceAction;
	protected Node hurtArea;
}
