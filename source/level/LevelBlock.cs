using Godot;


public class LevelBlock : StaticBody
{
	public void OnHurtAreaEntered(Area area)
	{
		area.EmitSignal(this.GetGDMethodAreaEntered(), hurtArea, area);
		
		if((exitUnlocked || !exitBlock) && this.IsLayerInMask(area, destroyMask))
			animationPlayer.Play("vanish");
	}

	public void SpawnItem() // Called by an animation
	{
		if(item != null)
		{
			item.Translation = this.GlobalTransform.origin + itemOffset;
			item.Call(this.GetMethodTransitTo(), "active");
		}
	}

	private void ObtainNodes()
	{
		hurtArea = GetNode<Area>(hurtAreaNP);

		if(destroyMask != 0)
			animationPlayer = GetNode<AnimationPlayer>(animationPlayerNP);
	}

  public override void _EnterTree()
  {
		ObtainNodes();
  }

	public void SetItem(Spatial item)
	{
		this.item = item;
	}

	public void Unlock(bool active)
	{
		exitUnlocked = active;
	}


	[Export]
	public NodePath hurtAreaNP;

	[Export]
	public NodePath animationPlayerNP;

	[Export(PropertyHint.Layers3dPhysics)]
	public uint destroyMask;

	[Export]
	public bool exitBlock = false;

	[Export]
	public Vector3 itemOffset = new Vector3(0f, 0.5f, 0f);


	private Spatial item;

	private Area hurtArea;
	private AnimationPlayer animationPlayer;

	private bool exitUnlocked;
}
