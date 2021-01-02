public static class ScreenSignalExtension
{
	public static string GetSignalSave(this Godot.Object gdObj)
	{
		return SAVE;
	}

	public static string GetSignalLoad(this Godot.Object gdObj)
	{
		return LOAD;
	}


	private const string SAVE = "sav";
	private const string LOAD = "lod";
}