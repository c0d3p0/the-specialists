using Godot;


public class SpecialistCharacterInitializer : Node
{
	protected virtual void ObtainNodes()
	{
		specialistCharacter = GetNode<SpecialistCharacter>(specialistCharacterNP);
		characterPhysics = GetNode<CharacterPhysics>(characterPhysicsNP);
		characterMove = GetNode<CharacterMove>(characterMoveNP);
		hurtArea = GetNode<Area>(hurtAreaNP);
		specialistCharacterAction = GetNode<SpecialistCharacterAction>(
					specialistCharacterActionNP);
		specialistCharacterStatus = GetNode<SpecialistCharacterStatus>(
				specialistCharacterStatusNP);
	}

	private void InitializeSpecialistCharacterAction()
	{
		specialistCharacterAction.AddUserSignal(this.GetSignalApplyMove());
		specialistCharacterAction.AddUserSignal(this.GetSignalChangeSelectedSlot());
		specialistCharacterAction.AddUserSignal(this.GetSignalIncreaseHealth());

		specialistCharacterAction.Connect(this.GetSignalApplyMove(),
				characterMove, this.GetMethodApplyMove());
		specialistCharacterAction.Connect(this.GetSignalChangeSelectedSlot(),
				specialistCharacterStatus, this.GetMethodChangeSelectedSlot());
		specialistCharacterAction.Connect(this.GetSignalIncreaseHealth(),
				specialistCharacterStatus, this.GetMethodIncreaseHealth());				
	}

	private void InitializeCharacterMove()
	{
		characterMove.AddUserSignal(this.GetSignalIncreaseVelocity());
		characterMove.Connect(this.GetSignalIncreaseVelocity(),
				characterPhysics, this.GetMethodIncreaseVelocity());
	}

	private void InitializeHurtArea()
	{
		hurtArea.AddUserSignal(this.GetSignalHit());
		hurtArea.AddUserSignal(this.GetSignalApplyItem());

		hurtArea.Connect(this.GetSignalHit(),
				specialistCharacterAction, this.GetMethodHit());
		hurtArea.Connect(this.GetSignalApplyItem(),
				specialistCharacter, this.GetMethodApplyItem());
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		InitializeCharacterMove();
		InitializeSpecialistCharacterAction();
		InitializeHurtArea();
	}


	[Export]
	public NodePath specialistCharacterNP;

	[Export]
	public NodePath characterPhysicsNP;

	[Export]
	public NodePath characterMoveNP;

	[Export]
	public NodePath specialistCharacterActionNP;

	[Export]
	public NodePath specialistCharacterStatusNP;

	[Export]
	public NodePath hurtAreaNP;


	protected SpecialistCharacter specialistCharacter;
	protected CharacterPhysics characterPhysics;
	protected CharacterMove characterMove;
	protected SpecialistCharacterAction specialistCharacterAction;
	protected SpecialistCharacterStatus specialistCharacterStatus;
	protected Area hurtArea;
}
