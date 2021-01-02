public static class LevelMethodExtension
{
	public static string GetMethodUnlock(this Godot.Object gdObj)
	{
		return UNLOCK;
	}

	public static string GetMethodOnObjectiveCompleted(this Godot.Object gdObj)
	{
		return ON_OBJECTIVE_COMPLETED;
	}

	public static string GetMethodGetPositionFromRandomEmptyBlockSlot(this Godot.Object gdObj)
	{
		return GET_POSITION_FROM_RANDOM_EMPTY_BLOCK_SLOT;
	}

	public static string GetMethodGetPositionFromRandomDefaultBlockSlot(this Godot.Object gdObj)
	{
		return GET_POSITION_FROM_RANDOM_DEFAULT_BLOCK_SLOT;
	}

	public static string GetMethodAddEmptyBlockSlot(this Godot.Object gdObj)
	{
		return ADD_EMPTY_BLOCK_SLOT;
	}

	public static string GetMethodRemoveEmptyBlockSlot(this Godot.Object gdObj)
	{
		return REMOVE_EMPTY_BLOCK_SLOT;
	}

	public static string GetMethodContainsEmptyBlockSlot(this Godot.Object gdObj)
	{
		return CONTAINS_EMPTY_BLOCK_SLOT;
	}


	private const string UNLOCK = "Unlock";
	private const string ON_OBJECTIVE_COMPLETED = "OnObjectiveCompleted";
	private const string GET_POSITION_FROM_RANDOM_EMPTY_BLOCK_SLOT =
			"GetPositionFromRandomEmptyBlockSlot";
	private const string GET_POSITION_FROM_RANDOM_DEFAULT_BLOCK_SLOT =
			"GetPositionFromRandomDefaultBlockSlot";
	private const string ADD_EMPTY_BLOCK_SLOT = "AddEmptyBlockSlot";
	private const string REMOVE_EMPTY_BLOCK_SLOT = "RemoveEmptyBlockSlot";
	private const string CONTAINS_EMPTY_BLOCK_SLOT = "ContainsEmptyBlockSlot";
}
