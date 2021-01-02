public static class PrefabMethodExtension
{
	public static string GetMethodSetAirStrikeLevel(this Godot.Object gdObj)
	{
		return SET_AIR_STRIKE_LEVEL;
	}

	public static string GetMethodPlant(this Godot.Object gdObj)
	{
		return PLANT;
	}

	public static string GetMethodDetonate(this Godot.Object gdObj)
	{
		return DETONATE;
	}

	public static string GetMethodAddAsAvailable(this Godot.Object gdObj)
	{
		return ADD_AS_AVAILABLE;
	}

	public static string GetMethodIsAvailable(this Godot.Object gdObj)
	{
		return IS_AVAILABLE;
	}

	public static string GetMethodSetManager(this Godot.Object gdObj)
	{
		return SET_MANAGER;
	}

	public static string GetMethodSetCharacter(this Godot.Object gdObj)
	{
		return SET_CHARACTER;
	}

	public static string GetMethodSetLaserRayLevel(this Godot.Object gdObj)
	{
		return SET_LASER_RAY_LEVEL;
	}

	public static string GetMethodSetDetonateTimeLevel(this Godot.Object gdObj)
	{
		return SET_DETONATE_TIME_LEVEL;
	}

	public static string GetMethodSetItem(this Godot.Object gdObj)
	{
		return SET_ITEM;
	}

	public static string GetMethodIsInactive(this Godot.Object gdObj)
	{
		return IS_INACTIVE;
	}

	public static string GetMethodOnRecycle(this Godot.Object gdObj)
	{
		return ON_RECYCLE;
	}



	private const string SET_AIR_STRIKE_LEVEL = "SetAirStrikeLevel";
	private const string PLANT = "Plant";
	private const string DETONATE = "Detonate";
	private const string ADD_AS_AVAILABLE = "AddAsAvailable";
	private const string IS_AVAILABLE = "IsAvailable";
	private const string SET_MANAGER = "SetManager";
	private const string SET_CHARACTER = "SetCharacter";
	private const string SET_LASER_RAY_LEVEL = "SetLaserRayLevel";
	private const string SET_DETONATE_TIME_LEVEL = "SetDetonateTimeLevel";
	private const string SET_ITEM = "SetItem";
	private const string IS_INACTIVE = "IsInactive";
	private const string ON_RECYCLE = "OnRecycle";
}
