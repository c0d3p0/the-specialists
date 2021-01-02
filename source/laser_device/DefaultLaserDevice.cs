using Godot;


public class DefaultLaserDevice : KinematicBody
{
	public void Plant()
	{
		laserDeviceAction.TransitTo("active");
		plantedTranslation = GlobalTransform.origin;
	}

	public void SetLaserRayLevel(int level)
	{
		if(laserDeviceAction != null)
			laserDeviceAction.LaserRayLevel = level;
	}

	public void SetDetonateTimeLevel(int level)
	{
		if(laserDeviceAction != null)
			laserDeviceAction.DetonateTimeLevel = level;
	}

	public void Move(Spatial pusher, Vector3 direction)
	{
		if(canBePushed)
		{
			laserDeviceAction.Pusher = pusher;
			characterMove.ApplyConstantMove(direction);

			if(manager != null)
				manager.Call(this.GetMethodAddEmptyBlockSlot(), plantedTranslation);
		}
	}

	public void Detonate()
	{
		laserDeviceAction.Detonate();
	}

	public void TransitTo(string action)
	{
		laserDeviceAction.TransitTo(action);
	}

	public void OnRecycle() // Called by an animation
	{
		laserDeviceAction.Reset();

		if(manager != null)
		{
			manager.Call(this.GetMethodAddAsAvailable(),
					laserDeviceType, this, plantedTranslation);
		}

		if(character != null && characterParent.HasNode(characterNodeName))
		{
			character.Call(this.GetMethodIncreasePlantedLaserDeviceAmount(), -1);
			character = null;
		}
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

	public void GetNodeType(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), laserDeviceType);
	}

	private void ObtainNodes()
	{
		characterMove = GetNode<CharacterMove>(characterMoveNP);
		laserDeviceAction = GetNode<DefaultLaserDeviceAction>(
				laserDeviceActionNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}

  public override void _ExitTree()
  {
		QueueFree();
  }


	[Export]
	public NodePath laserDeviceActionNP;

	[Export]
	public NodePath characterMoveNP;

	[Export]
	public string laserDeviceType;

	[Export]
	public bool canBePushed = true;


	protected Node manager;
	protected Node character;
	protected Node characterParent;
	protected string characterNodeName;
	protected Vector3 direction;


	private DefaultLaserDeviceAction laserDeviceAction;
	private CharacterMove characterMove;
	private Vector3 plantedTranslation;
}
