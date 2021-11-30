public static class CharacterStatusMethodExtension
{
	public static string GetMethodIncreaseSpeedLevel(this Godot.Object gdObj)
	{
		return INCREASE_SPEED_LEVEL;
	}

	public static string GetMethodIncreaseLaserRayLevel(this Godot.Object gdObj)
	{
		return INCREASE_LASER_RAY_LEVEL;
	}

	public static string GetMethodIncreaseDetonateTimeLevel(this Godot.Object gdObj)
	{
		return INCREASE_DETONATE_TIME_LEVEL;
	}

	public static string GetMethodIncreaseLaserDeviceAmount(this Godot.Object gdObj)
	{
		return INCREASE_LASER_DEVICE_AMOUNT;
	}

	public static string GetMethodIncreasePlantedLaserDeviceAmount(this Godot.Object gdObj)
	{
		return INCREASE_PLANTED_LASER_DEVICE_AMOUNT;
	}

	public static string GetMethodSetLaserDevice(this Godot.Object gdObj)
	{
		return SET_LASER_DEVICE;
	}

	public static string GetMethodIncreaseHealth(this Godot.Object gdObj)
	{
		return INCREASE_HEALTH;
	}

	public static string GetMethodIncreaseLives(this Godot.Object gdObj)
	{
		return INCREASE_LIVES;
	}

	public static string GetMethodSetSkill(this Godot.Object gdObj)
	{
		return SET_SKILL;
	}

	public static string GetMethodHasPushSkill(this Godot.Object gdObj)
	{
		return HAS_PUSH_SKILL;
	}

	public static string GetMethodHasShootSkill(this Godot.Object gdObj)
	{
		return HAS_SHOOT_SKILL;
	}

	public static string GetMethodSetDisease(this Godot.Object gdObj)
	{
		return SET_DISEASE;
	}

	public static string GetMethodClearDisease(this Godot.Object gdObj)
	{
		return CLEAR_DISEASE;
	}

	public static string GetMethodHasVirusDisease(this Godot.Object gdObj)
	{
		return HAS_VIRUS_DISEASE;
	}


	private const string INCREASE_SPEED_LEVEL = "IncreaseSpeedLevel";
	private const string INCREASE_LASER_RAY_LEVEL = "IncreaseLaserRayLevel";
	private const string INCREASE_LASER_DEVICE_AMOUNT = "IncreaseLaserDeviceAmount";
	private const string INCREASE_PLANTED_LASER_DEVICE_AMOUNT = "IncreasePlantedLaserDeviceAmount";
	private const string INCREASE_DETONATE_TIME_LEVEL = "IncreaseDetonateTimeLevel";
	private const string INCREASE_HEALTH = "IncreaseHealth";
	private const string INCREASE_LIVES = "IncreaseLives";
	private const string SET_LASER_DEVICE = "SetLaserDevice";
	private const string SET_SKILL = "SetSkill";
	private const string HAS_PUSH_SKILL = "HasPushSkill";
	private const string HAS_SHOOT_SKILL = "HasShootSkill";
	private const string SET_DISEASE = "SetDisease";
	private const string CLEAR_DISEASE = "ClearDisease";
	private const string HAS_VIRUS_DISEASE = "HasVirusDisease";
}
