using Godot;


public class DefaultProjectile : KinematicBody
{
	public void IsInactive(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), projectileAction.Inactive);
	}

	public void OnRecycle() // Called by an animation
	{
		if(manager != null)
		{
			projectileBehavior.Direction = Vector3.Zero;
			manager.Call(this.GetMethodAddAsAvailable(), projectileType,
					this, this.GlobalTransform.origin);
		}

		if(character != null && characterParent.HasNode(characterNodeName))
			character.Call(this.GetMethodGetClearProjectile());
		
		character = null;
	}

	public void SetProcessBehavior(bool active)
	{
		projectileBehavior.SetProcess(active);
		projectileBehavior.SetPhysicsProcess(active);
	}
	
	public void GetNodeType(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), projectileType);
	}

	public void TransitTo(string actionName)
	{
		projectileAction.TransitTo(actionName);
	}

	public void SetManager(Node manager)
	{
		this.manager = manager;
	}

	public void SetCharacter(Node character)
	{
		this.character = character;
		characterParent = character != null ? character.GetParent() : null;
		characterNodeName = character != null ? character.Name : "";
	}

	public void SetDirection(Vector3 direction)
	{
		projectileBehavior.Direction = direction;
	}

	protected virtual void ObtainNodes()
	{
		projectileAction = GetNode<ProjectileAction>(projectileActionNP);
		projectileBehavior = GetNode<DefaultProjectileBehavior>(projectileBehaviorNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}


	[Export]
	public NodePath projectileActionNP;

	[Export]
	public NodePath projectileBehaviorNP;

	[Export]
	public string projectileType;


	protected Node manager;
	protected Node character;
	protected Node characterParent;
	protected string characterNodeName;

	protected ProjectileAction projectileAction;
	protected DefaultProjectileBehavior projectileBehavior;
}
