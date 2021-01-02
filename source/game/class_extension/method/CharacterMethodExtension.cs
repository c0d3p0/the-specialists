public static class CharacterMethodExtension
{
	public static string GetMethodPlantDevice(this Godot.Object gdObj)
	{
		return PLANT_DEVICE;
	}

	public static string GetMethodChangeSelectedSlot(this Godot.Object gdObj)
	{
		return CHANGE_SELECTED_SLOT;
	}

	public static string GetMethodExecuteSkill(this Godot.Object gdObj)
	{
		return EXECUTE_SKILL;
	}

	public static string GetMethodMove(this Godot.Object gdObj)
	{
		return MOVE;
	}

	public static string GetMethodHit(this Godot.Object gdObj)
	{
		return HIT;
	}

	public static string GetMethodCheer(this Godot.Object gdObj)
	{
		return CHEER;
	}

	public static string GetMethodFixBodyDirection(this Godot.Object gdObj)
	{
		return FIX_BODY_DIRECTION;
	}

	public static string GetMethodIsDead(this Godot.Object gdObj)
	{
		return IS_DEAD;
	}

	public static string GetMethodSetDirection(this Godot.Object gdObj)
	{
		return SET_DIRECTION;
	}	

	public static string GetMethodGetMoveSpeedFactor(this Godot.Object gdObj)
	{
		return GET_MOVE_SPEED_FACTOR;
	}

	public static string GetMethodTryToIncreasePlantedLaserDeviceAmount(this Godot.Object gdObj)
	{
		return TRY_TO_INCREASE_PLANTED_LASER_DEVICE_AMOUNT;
	}

	public static string GetMethodCanExecuteSkill(this Godot.Object gdObj)
	{
		return CAN_EXECUTE_SKILL;
	}

	public static string GetMethodApplyItem(this Godot.Object gdObj)
	{
		return APPLY_ITEM;
	}

	public static string GetMethodTransitTo(this Godot.Object gdObj)
	{
		return TRANSIT_TO;
	}

	public static string GetMethodApplyMove(this Godot.Object gdObj)
	{
		return APPLY_MOVE;
	}

	public static string GetMethodApplyConstantMove(this Godot.Object gdObj)
	{
		return APPLY_CONSTANT_MOVE;
	}

	public static string GetMethodIncreaseVelocity(this Godot.Object gdObj)
	{
		return INCREASE_VELOCITY;
	}

	public static string GetMethodIncreaseTranslation(this Godot.Object gdObj)
	{
		return INCREASE_TRANSLATION;
	}	

	public static string GetMethodSetProcessBehavior(this Godot.Object gdObj)
	{
		return SET_PROCESS_BEHAVIOR;
	}

	public static string GetMethodGetClearProjectile(this Godot.Object gdObj)
	{
		return CLEAR_PROJECTILE;
	}

	public static string GetMethodGetSpecialistId(this Godot.Object gdObj)
	{
		return GET_SPECIALIST_ID;
	}


	private const string PLANT_DEVICE = "PlantDevice";
	private const string CHANGE_SELECTED_SLOT = "ChangeSelectedSlot";
	private const string EXECUTE_SKILL = "ExecuteSkill";
	private const string MOVE = "Move";
	private const string HIT = "Hit";
	private const string CHEER = "Cheer";

	private const string FIX_BODY_DIRECTION = "FixBodyDirection";
	
	private const string SET_DIRECTION = "SetDirection";
	private const string GET_MOVE_SPEED_FACTOR = "GetMoveSpeedFactor";

	private const string IS_DEAD = "IsDead";
	private const string TRY_TO_INCREASE_PLANTED_LASER_DEVICE_AMOUNT =
			"TryToIncreasePlantedLaserDeviceAmount";
	private const string CAN_EXECUTE_SKILL = "CanExecuteSkill";
	private const string APPLY_ITEM = "ApplyItem";
	private const string TRANSIT_TO = "TransitTo";

	private const string APPLY_MOVE = "ApplyMove";
	private const string APPLY_CONSTANT_MOVE = "ApplyConstantMove";
	private const string INCREASE_VELOCITY = "IncreaseVelocity";
	private const string INCREASE_TRANSLATION = "IncreaseTranslation";
	
	private const string SET_PROCESS_BEHAVIOR = "SetProcessBehavior";
	private const string CLEAR_PROJECTILE = "ClearProjectile";
	private const string GET_SPECIALIST_ID = "GetSpecialistId";
}
