public static class CharacterPhysicsSignalExtension
{
	public static string GetSignalPlantDevice(this Godot.Object gdObj)
	{
		return PLANT_DEVICE;
	}

	public static string GetSignalChangeSelectedSlot(this Godot.Object gdObj)
	{
		return CHANGE_SELECTED_SLOT;
	}

	public static string GetSignalExecuteSkill(this Godot.Object gdObj)
	{
		return EXECUTE_SKILL;
	}

	public static string GetSignalMove(this Godot.Object gdObj)
	{
		return MOVE;
	}

	public static string GetSignalHit(this Godot.Object gdObj)
	{
		return HIT;
	}

	public static string GetSignalDead(this Godot.Object gdObj)
	{
		return DEAD;
	}

	public static string GetSignalApplyItem(this Godot.Object gdObj)
	{
		return APPLY_ITEM;
	}

	public static string GetSignalApplyMove(this Godot.Object gdObj)
	{
		return APPLY_MOVE;
	}

	public static string GetSignalIncreaseVelocity(this Godot.Object gdObj)
	{
		return INCREASE_VELOCITY;
	}

	public static string GetSignalIncreaseTranslation(this Godot.Object gdObj)
	{
		return INCREASE_TRANSLATION;
	}


	private const string PLANT_DEVICE = "p_d";
	private const string CHANGE_SELECTED_SLOT = "c_s_s";
	private const string EXECUTE_SKILL = "e_s";
	private const string MOVE = "mov";
	private const string HIT = "hit";
	private const string DEAD = "dead";

	private const string APPLY_ITEM = "a_i";

	private const string APPLY_MOVE = "a_m";

	private const string INCREASE_VELOCITY = "i_v";
	private const string INCREASE_TRANSLATION = "i_t";
}
