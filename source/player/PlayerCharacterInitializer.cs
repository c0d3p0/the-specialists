using Godot;


public class PlayerCharacterInitializer : SpecialistCharacterInitializer
{
	protected override void ObtainNodes()
	{
		base.ObtainNodes();
		playerInputInterpreter = GetNode<PlayerInputInterpreter>(
				playerInputInterpreterNP);
	}

	private void InitializePlayerInputInterpreter()
	{		
		playerInputInterpreter.AddUserSignal(this.GetSignalPlantDevice());
		playerInputInterpreter.AddUserSignal(this.GetSignalChangeSelectedSlot());
		playerInputInterpreter.AddUserSignal(this.GetSignalExecuteSkill());
		playerInputInterpreter.AddUserSignal(this.GetSignalMove());

		playerInputInterpreter.Connect(this.GetSignalPlantDevice(),
				specialistCharacter, this.GetMethodPlantDevice());
		playerInputInterpreter.Connect(this.GetSignalChangeSelectedSlot(),
				specialistCharacter, this.GetMethodChangeSelectedSlot());
		playerInputInterpreter.Connect(this.GetSignalExecuteSkill(),
				specialistCharacter, this.GetMethodExecuteSkill());
		playerInputInterpreter.Connect(this.GetSignalMove(),
				specialistCharacter, this.GetMethodMove());
	}
	
	public override void _EnterTree()
	{
		base._EnterTree();
		InitializePlayerInputInterpreter();
	}


	[Export]
	public NodePath playerInputInterpreterNP;


	private PlayerInputInterpreter playerInputInterpreter;
}