public static class GUIMethodExtension
{
	public static string GetMethodGetGameplayRankingMap(this Godot.Object gdObj)
	{
		return GET_GAMEPLAY_RANKING_MAP;
	}

	public static string GetMethodGetLastGameplayDataMap(this Godot.Object gdObj)
	{
		return GET_LAST_GAMEPLAY_DATA_MAP;
	}

	public static string GetMethodGetGameplayDataMap(this Godot.Object gdObj)
	{
		return GET_GAMEPLAY_DATA_MAP;
	}

	public static string GetMethodIsValidDataMap(this Godot.Object gdObj)
	{
		return IS_VALID_DATA_MAP;
	}

	
	private const string GET_GAMEPLAY_RANKING_MAP  = "GetGameplayRankingMap";
	private const string GET_LAST_GAMEPLAY_DATA_MAP  = "GetLastGameplayDataMap";
	private const string GET_GAMEPLAY_DATA_MAP  = "GetGameplayDataMap";
	private const string IS_VALID_DATA_MAP  = "IsValidDataMap";
}
