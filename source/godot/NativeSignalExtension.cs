public static class NativeSignalExtension
{
	public static string GetGDSignalTreeExited(this Godot.Object gdObj)
	{
		return TREE_EXITED;
	}

	public static string GetGDSignalTreeEntered(this Godot.Object gdObj)
	{
		return TREE_ENTERED;
	}


	private const string TREE_EXITED = "tree_exited";
	private const string TREE_ENTERED = "tree_entered";
}
