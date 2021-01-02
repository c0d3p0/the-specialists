public class PlayerCharacter : SpecialistCharacter
{
	protected override void Initialize()
	{
		base.Initialize();
		playerInputInterpreter.PlayerId = specialistId + 1;
	}

	protected override void ObtainNodes()
	{
		base.ObtainNodes();
		playerInputInterpreter = specialistCharacterBehavior as PlayerInputInterpreter;
	}


	private PlayerInputInterpreter playerInputInterpreter;
}
