using Godot;


public class PlayerCharacterStatus : SpecialistCharacterStatus
{
	public void IncreaseLives(int amount)
	{
		lives = IncreaseValue(lives, amount, livesRange);
		PutGlobal("livesSpecialistIndex" + specialistId, lives);
	}

	protected override void Initialize()
	{
		base.Initialize();
		lives = GetGlobal<int>("livesSpecialistIndex" + specialistId);
	}


	[Export]
	public Vector2 livesRange = new Vector2(0f, 99f);


	private int lives;
}
