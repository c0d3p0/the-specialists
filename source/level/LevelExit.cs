using Godot;
using Godot.Collections;


public class LevelExit : Node
{
	public void Unlock(bool active)
	{
		for(int i = 0; i < exitBlocks.Length; i++)
			exitBlocks[i].Call(this.GetMethodUnlock(), active);

		exitArea.Monitoring = true;
	}

	public void OnExitAreaEntered(Area area)
	{
		levelManager.Call(this.GetMethodOnObjectiveCompleted());
	}

	private void ObtainNodes()
	{
		levelManager = GetNode(levelManagerNP);
		exitBlocks = this.GetNodes<Spatial>(this, exitBlockListNP);
		exitArea = GetNode<Area>(exitAreaNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}


	[Export]
	public NodePath levelManagerNP;

	[Export]
	public Array<NodePath> exitBlockListNP;

	[Export]
	public NodePath exitAreaNP;


	private Node levelManager;
	private Spatial[] exitBlocks;
	private Area exitArea;
}
