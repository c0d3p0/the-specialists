public static class ScreenMethodExtension
{
	public static string GetMethodShowScreen(this Godot.Object gdObj)
	{
		return SHOW_SCREEN;
	}

	public static string GetMethodSave(this Godot.Object gdObj)
	{
		return SAVE;
	}

	public static string GetMethodLoad(this Godot.Object gdObj)
	{
		return LOAD;
	}

	public static string GetMethodOnToggleCheatCode(this Godot.Object gdObj)
	{
		return ON_TOGGLE_CHEAT_CODE;
	}


	private const string SHOW_SCREEN = "ShowScreen";
	private const string SAVE = "Save";
	private const string LOAD = "Load";
	private const string ON_TOGGLE_CHEAT_CODE = "OnToggleCheatCode";
}
