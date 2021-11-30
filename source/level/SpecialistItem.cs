using Godot;


public class SpecialistItem : Area
{
	public void TransitTo(string actionName)
	{
		animationStateMachine.Travel(actionName);
	}

	public void OnHurtAreaEntered(Area area)
	{
		if(operating)
		{
			if(area.IsInGroup("specialist"))
			{
				operating = false;
				ApplyItemFeatureToTheSpecialist(area);
				animationStateMachine.Travel("get_picked");
			}
			else if(this.IsLayerInMask(area, vanishMask))
			{
				operating = false;
				animationStateMachine.Travel("dissolve");
				area.EmitSignal(this.GetGDMethodAreaEntered(), this, area);
			}
		}
	}

	private void ApplyItemFeatureToTheSpecialist(Area area)
	{
		int id = GetFixedFeatureId();
		area.EmitSignal(this.GetSignalApplyItem(),
				specialistMethods[id], features[id]);
	}

	private int GetFixedFeatureId()
	{
		if(featureId > LAST_FEATURE_ID)
		{
			if(featureId == 23)
				return this.RandiRange(rng, 0, 9);	// RandomMainItem
			else if(featureId == 24)
				return this.RandiRange(rng, 0, 3) * 2;	// RandomIncreaseItem
			else if(featureId == 25)
				return (this.RandiRange(rng, 0, 3) * 2) + 1;	// RandomDecreaseItem
			else if(featureId == 26)
				return this.RandiRange(rng, 10, 14);	// RandomDeviceItem
			else if(featureId == 27)
				return this.RandiRange(rng, 15, 17);	// RandomSkillItem
			else
				return this.RandiRange(rng, 18, 22);	// RandomDiseaseItem
		}
		else
			return featureId;
	}

	private void Initialize()
	{
		rng = new RandomNumberGenerator();
		operating = true;
	}

	private void InitializeFeatures()
	{
		features = new object[]
		{
			increaseAmount, // IncreaseSpeed
			-increaseAmount, // DecreaseSpeed
			increaseAmount, // IncreaseLaserRay
			-increaseAmount, // DecreaseLaserRay
			increaseAmount, // IncreaseDetonateTime
			-increaseAmount, // DecreaseDetonateTime
			increaseAmount, // IncreaseLaserDeviceAmount
			-increaseAmount, // DecreaseLaserDeviceAmount

			100 * increaseAmount, // IncreaseHealth
			increaseAmount, // IncreaseLives

			"S", // StandardLaserDevice
			"A", // ArmoredLaserDevice
			"P", // PierceLaserDevice
			"M", // MineLaserDevice
			"R", // MOABLaserDevice
			
			"P", // CanPushObjects
			"L", // CanShootLaserSpark
			"S", // SuperSpeed

			"H", // Disease VeryHeavy
			"R", // Disease WeakLaserRay
			"N", // Disease NoLaserDevice
			"S", // Disease SlowDetonation
			"V", // Disease Virus
		};	
	}

	private void InitializeSpecialistMethods()
	{
		specialistMethods = new string[]
		{
			this.GetMethodIncreaseSpeedLevel(), // IncreaseSpeedLevel
			this.GetMethodIncreaseSpeedLevel(), // DecreaseSpeedLevel
			this.GetMethodIncreaseLaserRayLevel(), // IncreaseLaserRayLevel
			this.GetMethodIncreaseLaserRayLevel(), // DecreaseLaserRayLevel
			this.GetMethodIncreaseDetonateTimeLevel(), // IncreaseDetonateTime
			this.GetMethodIncreaseDetonateTimeLevel(), // DecreaseDetonateTime
			this.GetMethodIncreaseLaserDeviceAmount(), // IncreaseLaserDeviceAmount
			this.GetMethodIncreaseLaserDeviceAmount(), // DecreaseLaserDeviceAmount
			this.GetMethodIncreaseHealth(), // IncreaseHealth
			this.GetMethodIncreaseLives(), // IncreaseLives

			this.GetMethodSetLaserDevice(), // StandardLaserDevice
			this.GetMethodSetLaserDevice(), // PierceLaserDevice
			this.GetMethodSetLaserDevice(), // MobileLaserDevice
			this.GetMethodSetLaserDevice(), // MineLaserDevice
			this.GetMethodSetLaserDevice(), // MOABLaserDevice
			
			this.GetMethodSetSkill(), // CanPushObjects
			this.GetMethodSetSkill(), // CanShootLaserSpark
			this.GetMethodSetSkill(), // SuperSpeed

			this.GetMethodSetDisease(), // Disease VeryHeavy
			this.GetMethodSetDisease(), // Disease WeakLaserRay
			this.GetMethodSetDisease(), // Disease NoLaserDevice
			this.GetMethodSetDisease(), // Disease SlowDetonation
			this.GetMethodSetDisease(), // Disease Virus
		};
	}

	private void ObtainNodes()
	{
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).Get(
				"parameters/playback") as AnimationNodeStateMachinePlayback;
	}

  public override void _EnterTree()
  {
		ObtainNodes();
		Initialize();
		InitializeFeatures();
		InitializeSpecialistMethods();
  }
  

	[Export]
	public NodePath animationTreeNP;

	[Export(PropertyHint.Enum, "IncreaseSpeed,DecreaseSpeed," +
			"IncreaseLaserRay,DecreaseLaserRay," +
			"IncreaseDetonateTime,DecreaseDetonateTime," +
			"IncreaseLaserDeviceAmount,DecreaseLaserDeviceAmount," +
			"IncreaseHealth,IncreaseLives," +

			"StandardLaserDevice,ArmoredLaserDevice,PierceLaserDevice," + 
			"MineLaserDevice,MOABLaserDevice," +

			"CanPushObject,CanShootLaserSpark,SuperSpeed," +

			"VeryHeavy,WeakLaserRay,NoLaserDevice,SlowDetonation,Virus," + 

			"RandomMainItem,RandomIncreaseItem,RandomDecreaseItem," +
			"RandomDevice,RandomSkill,RandomDisease")]
	public int featureId;

	[Export]
	public int increaseAmount = 1;

	[Export(PropertyHint.Layers3dPhysics)]
	public uint vanishMask = 1024 + 16384;


	protected AnimationNodeStateMachinePlayback animationStateMachine;

	private RandomNumberGenerator rng;
	private object[] features;
	private string[] specialistMethods;
	private bool operating;

	private const int LAST_FEATURE_ID = 22;
}
