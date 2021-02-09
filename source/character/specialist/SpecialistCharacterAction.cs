using Godot;
using Godot.Collections;


public class SpecialistCharacterAction : BaseCharacterAction
{
	public void Hit(Area attackerArea, Area victimArea, int damageTaken)
	{
		if(!cheer && (!ignoreTransition || damageTaken >= 100000))
		{
			ignoreTransition = true;
			int fixedDamageTaken = damageTaken >= 100000 ? -damageTaken : -100;
			EmitSignal(this.GetSignalIncreaseHealth(), fixedDamageTaken);
			character.Call(this.GetMethodSetProcessBehavior(), false);
			TryToDropLaserDeviceTrigger();

			if(this.Call<bool>(character, this.GetMethodIsDead()))
				animationStateMachine.Travel("die");
			else
				animationStateMachine.Travel("hit");
		}
	}

	public bool PlantDevice()
	{
		string a = CanTransitToPlantDevice();

		if(a != null && !ignoreTransition)
		{
			Dictionary lddm = this.Call<Dictionary>(character,
					this.GetMethodTryToIncreasePlantedLaserDeviceAmount());

			if(lddm != null)
			{
				Vector3 translation = character.GlobalTransform.origin;
				translation.x = Mathf.Round(translation.x);
				translation.z = Mathf.Round(translation.z);
				lddm.Add(3, translation);
				Array plantDataList = this.Call<Array>(laserDeviceManager,
						this.GetMethodPlant(), character, lddm);

				if(plantDataList != null && plantDataList.Count > 0)
					return true;
				else
					character.Call(this.GetMethodIncreasePlantedLaserDeviceAmount(), -1);
			}
		}

		return false;
	}

	public bool ChangeSelectedSlot()
	{
		EmitSignal(this.GetSignalChangeSelectedSlot());
		return true;
	}

	public bool ExecuteSkill()
	{
		string a = CanTransitToExecuteSkill();

		if(!cheer && !ignoreTransition && a != null)
		{
			if(this.Call<bool>(character, this.GetMethodHasPushSkill()))
			{
				character.Call(this.GetMethodSetProcessBehavior(), false);
				animationStateMachine.Travel("skill");
				return true;
			}		
			else if(CanShootLaserDeviceTrigger())
			{
				character.Call(this.GetMethodSetProcessBehavior(), false);
				animationStateMachine.Travel("skill");
				return true;
			}				
		}

		return false;
	}

	public bool Move(Vector3 direction)
	{
		string a = CanTransitToMove();

		if(!cheer && !ignoreTransition && a != null)
		{

			if(direction.x != 0 || direction.z != 0 && a != null)
			{
				float moveSpeedFactor = this.Call<float>(character,
						this.GetMethodGetMoveSpeedFactor());
				FixBodyDirection(direction);
				EmitSignal(this.GetSignalApplyMove(), direction, moveSpeedFactor);

				if(moveSpeedFactor < 2f)
					animationStateMachine.Travel("walk");
				else
					animationStateMachine.Travel("run");
			}
			else
			{
				EmitSignal(this.GetSignalApplyMove(), Vector3.Zero);
				animationStateMachine.Travel("idle");
			}
		}

		return false;
	}		

	public void ExecuteSkillAction()	// Called by an animation
	{
		if(this.Call<bool>(character, this.GetMethodHasPushSkill()))
			PushObject();
		else if(this.Call<bool>(character, this.GetMethodHasShootSkill()))
			ShootLaserDeviceTrigger();
	}

	public void ActivateDisease()
	{
		diseaseSprite.Visible = true;
		diseaseTimer.Start();
	}

	public void Cheer()
	{
		cheer = true;
		ignoreTransition = true;
		character.Call(this.GetMethodSetProcessBehavior(), false);
	}	

	public void Die() // Called by animation
	{
		character.EmitSignal(this.GetSignalDead());
	}

	public void OnDiseaseTimeout()
	{
		if(this.Call<bool>(character, this.GetMethodHasVirusDisease()))
			Hit(null, null, 100);

		character.Call(this.GetMethodClearDisease());
		diseaseSprite.Visible = false;
	}

	private void PushObject()	
	{
		Spatial c = pushRayCast.GetCollider() as Spatial;

		if(c != null)
			c.Call(this.GetMethodMove(), character, GetBodyDirection());
	}

	public void ClearProjectile()
	{
		laserDeviceTrigger = null;
	}
	
	private void ShootLaserDeviceTrigger()
	{
		laserDeviceTrigger.Translation = laserDeviceTriggerPosition.GlobalTransform.origin;
		laserDeviceTrigger.Call(this.GetMethodSetDirection(), body.GlobalTransform.basis.z);
		laserDeviceTrigger.Call(this.GetMethodTransitTo(), "active");
	}

	private void TryToDropLaserDeviceTrigger()
	{
		if(laserDeviceTrigger != null &&
				this.Call<bool>(laserDeviceTrigger, this.GetMethodIsInactive()))
		{
			laserDeviceTrigger.Call(this.GetMethodOnRecycle());
			laserDeviceTrigger = null;
		}
	}

	private void ForceCheer()
	{
		if(cheer)
		{
			string a = CanTransitToCheer();
			character.Call(this.GetMethodSetProcessBehavior(), false);
			
			if(a != null)
				animationStateMachine.Travel("cheer");
		}
	}

	private bool CanShootLaserDeviceTrigger()
	{
		if(this.Call<bool>(character, this.GetMethodHasShootSkill()) &&
				laserDeviceTrigger == null)
		{	
			Array a = this.Call<Array>(skillManager,
					this.GetMethodRequest(), character, "L");
			laserDeviceTrigger = a.Count > 0 ? a[0] as Spatial : null;
			return laserDeviceTrigger != null;
		}

		return false;
	}

	protected override void ObtainNodes()
	{
		base.ObtainNodes();
		pushRayCast = GetNode<RayCast>(pushRayCastNP);
		diseaseSprite = GetNode<Sprite3D>(diseaseSpriteNP);
		diseaseTimer = GetNode<Timer>(diseaseTimerNP);
		laserDeviceTriggerPosition = GetNode<Spatial>(laserDeviceTriggerPositionNP);
	}

	public override void _Process(float delta)
	{
		ForceCheer();
	}

	protected string CanTransitToPlantDevice()
	{	
		return GetValue(toPlantDeviceMap, animationStateMachine.GetCurrentNode());
	}

	protected string CanTransitToExecuteSkill()
	{	
		return GetValue(toExecuteSkillMap, animationStateMachine.GetCurrentNode());
	}

	protected string CanTransitToCheer()
	{	
		return GetValue(toCheerMap, animationStateMachine.GetCurrentNode());
	}

	public Node LaserDeviceManager
	{
		set
		{
			laserDeviceManager = value;
		}	
	}

	public Node SkillManager
	{
		set
		{
			skillManager = value;
		}	
	}

	public Spatial LaserDeviceTrigger
	{
		set
		{
			laserDeviceTrigger = value;
		}	
	}


	[Export]
	public Dictionary<string, string> toPlantDeviceMap;

	[Export]
	public Dictionary<string, string> toExecuteSkillMap;

	[Export]
	public Dictionary<string, string> toCheerMap;

	[Export]
	public NodePath pushRayCastNP;

	[Export]
	public NodePath diseaseTimerNP;

	[Export]
	public NodePath diseaseSpriteNP;

	[Export]
	public NodePath laserDeviceTriggerPositionNP;

	
	protected Node laserDeviceManager;
	protected Node skillManager;
	protected RayCast pushRayCast;
	protected Timer diseaseTimer;
	protected Sprite3D diseaseSprite;
	private Spatial laserDeviceTriggerPosition;


	private Spatial laserDeviceTrigger;
	protected bool cheer;
}
