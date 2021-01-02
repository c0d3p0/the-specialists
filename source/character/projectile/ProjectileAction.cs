using Godot;
using Godot.Collections;


public class ProjectileAction : Node
{
	public void OnCameraExited(Camera camera)
	{
		if(animationStateMachine.GetCurrentNode().Equals("move"))
			Disappear();
	}

	public void OnStrikeAreaEntered(Area areaEntered, Area strikeArea)
	{
		if(this.IsLayerInMask(areaEntered, disappearMask))
			Disappear();

		areaEntered.EmitSignal(this.GetSignalHit(), strikeArea, areaEntered, damageGiven);
	}

	public void TransitTo(string actionName)
	{
		animationStateMachine.Travel(actionName);
	}

	protected void Disappear()
	{
		projectile.Call(this.GetMethodSetProcessBehavior(), false);
		animationStateMachine.Travel("disappear");
	}

	protected void Move(Vector3 direction)
	{
		string a = CanTransitToMove();

		if(a != null)
		{
			EmitSignal(this.GetSignalApplyMove(), direction);
			animationStateMachine.Travel("move");
		}
	}

	protected void TryToDestroyWhenColliding()
	{
		string a = CanTransitToMove();

		if(a != null)
		{
			for(int i = 0; i < projectile.GetSlideCount(); i++)
			{
				if(projectile.GetSlideCollision(i).Collider != null)
				{
					projectile.Call(this.GetMethodSetProcessBehavior(), false);
					animationStateMachine.Travel("vanish");
					break;
				}
			}
		}
	}

	protected string CanTransitToMove()
	{
		string a;

		if(toMoveMap.TryGetValue(animationStateMachine.GetCurrentNode(), out a))
			return a;

		return null;
	}

	protected virtual void ObtainNodes()
	{
		projectile = GetNode<KinematicBody>(projectileNP);
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).Get(
				"parameters/playback") as AnimationNodeStateMachinePlayback;
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}

	public override void _Ready()
	{
		SetPhysicsProcess(destroyWhenCollide);
	}

	public override void _PhysicsProcess(float delta)
	{
		TryToDestroyWhenColliding();
	}


	public bool Inactive
	{
		get
		{
			string ca = animationStateMachine.GetCurrentNode();
			return ca != null && ca.Equals("inactive");
		}	
	}


	[Export]
	public NodePath projectileNP;

	[Export]
	public NodePath animationTreeNP;

	[Export]
	public Dictionary<string, string> toMoveMap;

	[Export]
	public int damageGiven = 100;

	[Export(PropertyHint.Layers3dPhysics)]
	public uint disappearMask = 256;

	[Export]
	public bool destroyWhenCollide;


	private KinematicBody projectile;
	private AnimationNodeStateMachinePlayback animationStateMachine;
}
