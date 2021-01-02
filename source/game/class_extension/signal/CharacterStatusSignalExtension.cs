public static class CharacterStatusSignalExtension
{
	public static string GetSignalIncreaseHealth(this Godot.Object gdObj)
	{
		return INCREASE_HEALTH;
	}


	private const string INCREASE_HEALTH = "i_h";
}