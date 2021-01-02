public static class GameMethodExtension
{
	public static string GetMethodGet(this Godot.Object gdObj)
	{
		return GET;
	}

	public static string GetMethodGetAllAsList(this Godot.Object gdObj)
	{
		return GET_ALL_AS_LIST;
	}

	public static string GetMethodPut(this Godot.Object gdObj)
	{
		return PUT;
	}

	public static string GetMethodPutAll(this Godot.Object gdObj)
	{
		return PUT_ALL;
	}

	public static string GetMethodRemove(this Godot.Object gdObj)
	{
		return REMOVE;
	}

	public static string GetMethodClear(this Godot.Object gdObj)
	{
		return CLEAR;
	}

	public static string GetMethodSetActive(this Godot.Object gdObj)
	{
		return SET_ACTIVE;
	}

	public static string GetMethodGetNodeType(this Godot.Object gdObj)
	{
		return GET_NODE_TYPE;
	}

	public static string GetMethodRequest(this Godot.Object gdObj)
	{
		return REQUEST;
	}


	private const string GET = "Get";
	private const string GET_ALL_AS_LIST = "GetAllAsList";
	private const string PUT = "Put";
	private const string PUT_ALL = "PutAll";
	private const string REMOVE = "Remove";
	private const string CLEAR = "Clear";
	private const string SET_ACTIVE = "SetActive";
	private const string GET_NODE_TYPE = "GetNodeType";
	private const string REQUEST = "Request";
}