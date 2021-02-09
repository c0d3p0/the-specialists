using Godot;


public class SpecialistCharacter : KinematicBody
{
	public void PlantDevice(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), specialistCharacterAction.PlantDevice());
	}

	public void ExecuteSkill(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), specialistCharacterAction.ExecuteSkill());
	}

	public void ChangeSelectedSlot(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), specialistCharacterAction.ChangeSelectedSlot());
	}

	public void Move(Vector3 direction, Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), specialistCharacterAction.Move(direction));
	}

	public void TryToIncreasePlantedLaserDeviceAmount(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(),
				specialistCharacterStatus.TryToIncreasePlantedLaserDeviceAmount());
	}

	public void IncreasePlantedLaserDeviceAmount(int amount)
	{
		specialistCharacterStatus.IncreasePlantedLaserDeviceAmount(amount);				
	}

	public void GetMoveSpeedFactor(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), specialistCharacterStatus.MoveSpeedFactor);
	}

	public void ApplyItem(string method, object value)
	{
		specialistCharacterStatus.ApplyItem(method, value);

		if(method.Equals(this.GetMethodSetDisease()) && (value as string) != null)
			specialistCharacterAction.ActivateDisease();
	}

	public void Cheer()
	{
		specialistCharacterAction.Cheer();
	}

	public void SetIgnoreTransition(bool active)
	{
		specialistCharacterAction.IgnoreTransition = active;
	}

	public void FixBodyDirection(Vector3 direction)
	{
		specialistCharacterAction.FixBodyDirection(direction);
	}
	
	public void SetProcessBehavior(bool active)
	{
		specialistCharacterBehavior.SetProcess(active);
		specialistCharacterBehavior.SetPhysicsProcess(active);
	}

	public void TransitTo(string actionName)
	{
		specialistCharacterAction.TransitTo(actionName);
	}

	public void ClearProjectile()
	{
		specialistCharacterAction.ClearProjectile();
	}

	public void GetSpecialistId(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), specialistId);
	}

	public void HasVirusDisease(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), specialistCharacterStatus.HasVirusDisease());
	}

	public void ClearDisease()
	{
		specialistCharacterStatus.ClearDisease();
	}

	public void HasPushSkill(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), specialistCharacterStatus.HasPushSkill());
	}

	public void HasShootSkill(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), specialistCharacterStatus.HasShootSkill());
	}

	public void IsDead(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), specialistCharacterStatus.Dead);
	}

	protected virtual void Initialize()
	{
		specialistCharacterStatus.SpecialistId = specialistId;
		specialistCharacterAction.LaserDeviceManager = laserDeviceManager;
		specialistCharacterAction.SkillManager = skillManager;
	}

	protected virtual void ObtainNodes()
	{
		laserDeviceManager = GetNode(laserDeviceManagerNP);
		skillManager = GetNode(skillManagerNP);
		specialistCharacterBehavior = GetNode(specialistCharacterBehaviorNP);
		specialistCharacterStatus = GetNode<SpecialistCharacterStatus>(
				specialistCharacterStatusNP);
		specialistCharacterAction = GetNode<SpecialistCharacterAction>(
				specialistCharacterActionNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}


	[Export]
	public NodePath laserDeviceManagerNP;

	[Export]
	public NodePath skillManagerNP;

	[Export]
	public NodePath specialistCharacterActionNP;

	[Export]
	public NodePath specialistCharacterBehaviorNP;

	[Export]
	public NodePath specialistCharacterStatusNP;

	[Export]
	public int specialistId;


	protected Node laserDeviceManager;
	protected Node skillManager;
	protected SpecialistCharacterAction specialistCharacterAction;
	protected SpecialistCharacterStatus specialistCharacterStatus;
	protected Node specialistCharacterBehavior;
}
