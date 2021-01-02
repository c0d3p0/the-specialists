using Godot;


public class SpecialistCharacterDebug : Node
{
	private void HandleInput(InputEventKey inputEventKey)
	{
		if(inputEventKey != null && inputEventKey.Pressed)
		{
			uint keyCode = inputEventKey.Scancode;

			if(keyCode == (uint) KeyList.Kp8)
				ExecuteDebugFeature(-1);
			else if(keyCode == (uint) KeyList.Kp9)
				ExecuteDebugFeature(1);
			else if(keyCode == (uint) KeyList.Kp7)
				IncreaseFeatureIndex(-1);
			else if(keyCode == (uint) KeyList.KpAdd)
				IncreaseFeatureIndex(1);
			else if(keyCode == (uint) KeyList.KpDivide)
				IncreaseDebugSpecialistId(-1);
			else if(keyCode == (uint) KeyList.KpMultiply)
				IncreaseDebugSpecialistId(1);
			else if(keyCode == (uint) KeyList.KpSubtract)
				ForceHit();
			else if(keyCode == (uint) KeyList.KpPeriod)
				KillAll();
		}
	}

	private void ExecuteDebugFeature(int amount)
	{
		if(IsThisSpecialistSelected())
		{
			if(featureIndex == 0)
				status.Call(this.GetMethodIncreaseLives(), amount);
			else if(featureIndex == 1)
				SetLaserDevice(amount);
			else if(featureIndex == 2)
				status.Call(this.GetMethodIncreaseHealth(), amount * 100);
			else if(featureIndex == 3)
				SetSkill(amount);
			else if(featureIndex == 4)
				status.Call(this.GetMethodIncreaseSpeedLevel(), amount);
			else if(featureIndex == 5)
				status.Call(this.GetMethodIncreaseLaserRayLevel(), amount);
			else if(featureIndex == 6)
				status.Call(this.GetMethodIncreaseLaserDeviceAmount(), amount);
			else if(featureIndex == 7)
				status.Call(this.GetMethodIncreaseDetonateTimeLevel(), amount);
		}
	}

	private void ForceHit()
	{
		if(IsThisSpecialistSelected())
			action.Hit(null, null, 100);
	}

	private void KillAll()
	{
		action.Hit(null, null, 1000000);
	}

	private void IncreaseFeatureIndex(int amount)
	{
		featureIndex = Mathf.Max(featureIndex + amount, 0);
		featureIndex = Mathf.Min(featureIndex, 7);
	}

	private void SetLaserDevice(int amount)
	{
		laserDeviceIndex = Mathf.Max(laserDeviceIndex + amount, 0);
		laserDeviceIndex = Mathf.Min(laserDeviceIndex, laserDevices.Length - 1);
		status.Call(this.GetMethodSetLaserDevice(), laserDevices[laserDeviceIndex]);
	}

	private void SetSkill(int amount)
	{
		skillIndex = Mathf.Max(skillIndex + amount, 0);
		skillIndex = Mathf.Min(skillIndex, skills.Length - 1);
		status.Call(this.GetMethodSetSkill(), skills[skillIndex]);
	}

	private void IncreaseDebugSpecialistId(int amount)
	{
		debugSpecialistId = Mathf.Max(debugSpecialistId + amount, 0);
		debugSpecialistId = Mathf.Min(debugSpecialistId, maximumSpecialistAmount - 1);

		if(IsThisSpecialistSelected())
		{
			GD.PushWarning("DebugSpecialistId: " + debugSpecialistId);
			GD.PushWarning("");
		}
	}

	private bool IsThisSpecialistSelected()
	{
		return specialistCharacter.specialistId == debugSpecialistId;
	}

	// NOTE: Unused
	private void PrintExecutedFeature(string method, object value)
	{
		GD.PushWarning("DebugSpecialistId: " + debugSpecialistId);
		GD.PushWarning("Feature: " + method);
		GD.PushWarning("Value: " + value.ToString());
		GD.PushWarning("");
	}
	// NOTE: Unused
	
	private void ObtainNodes()
	{
		status = GetNode(specialistCharacterStatusNP);
		action = GetNode<SpecialistCharacterAction>(specialistCharacterActionNP);
		specialistCharacter = GetNode<SpecialistCharacter>(specialistCharacterNP);
	}

	private void Initialize()
	{
		skills = new string[]{"", "P", "L", "S"};
		laserDevices = new string[]{"S", "A", "P", "M", "R"};
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}

	public override void _Ready()
	{
		SetProcessInput(OS.IsDebugBuild());
	}

	public override void _Input(InputEvent inputEvent)
	{
		HandleInput(inputEvent as InputEventKey);
	}


	[Export]
	public NodePath specialistCharacterNP;

	[Export]
	public NodePath specialistCharacterActionNP;

	[Export]
	public NodePath specialistCharacterStatusNP;

	[Export]
	public int maximumSpecialistAmount = 4;


	private SpecialistCharacter specialistCharacter;
	private Node status;
	private SpecialistCharacterAction action;

	private int featureIndex;
	private string[] skills;
	private string[] laserDevices;
	private int skillIndex;
	private int laserDeviceIndex;
	private int debugSpecialistId;
}
