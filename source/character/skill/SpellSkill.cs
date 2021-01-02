using Godot;
using Godot.Collections;


public class SpellSkill : BaseSkill
{	
	public override bool CanExecuteSkill()
	{
		Array a = this.Call<Array>(skillManager, this.GetMethodRequest(),
				character, projectileType);

		if(a.Count > 0)
			projectile = a[0] as Spatial;

		return a.Count > 0 && projectile != null;
	}

	public override void ExecuteSkill()
	{
		projectile.Translation = position.GlobalTransform.origin;
		projectile.Call(this.GetMethodSetDirection(), body.GlobalTransform.basis.z);
		projectile.Call(this.GetMethodTransitTo(), "active");
	}

	private void ObtainNodes()
	{
		character = GetNode<Spatial>(characterNP);
		body = GetNode<Spatial>(bodyNP);
		position = GetNode<Spatial>(positionNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}
	

	[Export]
	public NodePath characterNP;

	[Export]
	public NodePath bodyNP;

	[Export]
	public NodePath positionNP;

	[Export]
	public string projectileType;


	private Spatial character;
	private Spatial body;
	private Spatial position;

	private Spatial projectile;
}
