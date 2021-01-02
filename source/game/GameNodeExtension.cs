using Godot;


// NOTE: Extra methods specific for this game
public static class GameNodeExtension
{
	// A node specific to hold dynamically created nodes should be added as the first
	// child of the scene. This is done so that enemies with GUI elements do not get
	// drawn over the PauseScreen, which will make impossible to have any interaction
	// with the PauseScreen.
	public static void AddChildToBlockContainer(this Node gdNode, Node caller, Node block)
	{
		if(caller.IsInsideTree())
		{
			caller.GetTree().CurrentScene.GetChild(0).GetChild(0).
					CallDeferred(caller.GetGDMethodAddChild(), block);
		}
	}

	public static void AddChildToItemContainer(this Node gdNode, Node caller, Node item)
	{
		if(caller.IsInsideTree())
		{
			caller.GetTree().CurrentScene.GetChild(0).GetChild(1).
					CallDeferred(caller.GetGDMethodAddChild(), item);
		}
	}
}
