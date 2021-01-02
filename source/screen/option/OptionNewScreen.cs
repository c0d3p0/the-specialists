using Godot;


public class OptionNewScreen : Node
{
	public void LoadCreditScreen()
	{
		PutGlobal("optionSection", 4);
		PutGlobal("sceneToLoad", this.GetScenePath(creditsScreenScenePath));
		GetTree().ChangeScene(this.GetScenePath(loadScreenScenePath));
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
	}

	private void PutGlobal(string key, object value)
	{
		globalData.Call(this.GetMethodPut(), key, value);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}


	[Export]
	public string loadScreenScenePath = "screen/load_screen";

	[Export]
	public string creditsScreenScenePath = "screen/credits_screen";

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	
	private Node globalData;
}
