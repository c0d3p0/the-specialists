using Godot;


public class TeleportSkill : BaseSkill
{
	public override void ExecuteSkill()
	{
		Vector3 translation = this.Call<Vector3>(levelManager,
				this.GetMethodGetPositionFromRandomEmptyBlockSlot());
		translation.y = character.GlobalTransform.origin.y;
		character.Translation = translation;
	}

	private void ObtainNodes()
	{
		character = GetNode<Spatial>(characterNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}
	

	[Export]
	public NodePath characterNP;


	private Spatial character;
}
