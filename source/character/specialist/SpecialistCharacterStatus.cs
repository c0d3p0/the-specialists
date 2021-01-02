using Godot;
using Godot.Collections;


public class SpecialistCharacterStatus : BaseCharacterStatus
{
	public void ApplyItem(string method, object itemFeature)
	{
		Call(method, itemFeature);
	}

	public override void IncreaseHealth(int amount)
	{
		base.IncreaseHealth(amount);
		PutGlobal("healthSpecialistIndex" + specialistId, health);
	}

	public override void IncreaseSpeedLevel(int amount)
	{
		base.IncreaseSpeedLevel(amount);
		PutGlobal("speedLevelSpecialistIndex" + specialistId, speedLevel);
	}

	public void IncreaseLaserRayLevel(int amount)
	{
		laserRayLevel = IncreaseValue(laserRayLevel, amount, laserRayLevelRange);
		PutGlobal("laserRayLevelSpecialistIndex" + specialistId, laserRayLevel);
	}

	public void IncreaseLaserDeviceAmount(int amount)
	{
		laserDeviceAmount = IncreaseValue(laserDeviceAmount,
				amount, laserDeviceAmountRange);
		PutGlobal("laserDeviceAmountSpecialistIndex" + specialistId,
				laserDeviceAmount);
	}

	public void IncreaseDetonateTimeLevel(int amount)
	{
		detonateTimeLevel = IncreaseValue(detonateTimeLevel,
				amount, detonateTimeLevelRange);
		PutGlobal("detonateTimeLevelSpecialistIndex" + specialistId,
				detonateTimeLevel);
	}

	public Dictionary TryToIncreasePlantedLaserDeviceAmount()
	{
		if(!HasNoLaserDeviceDisease() && plantedLaserDeviceAmount < laserDeviceAmount)
		{
			IncreasePlantedLaserDeviceAmount(1);
			laserDeviceDataMap.Clear();
			laserDeviceDataMap.Add(0, slotList[selectedSlot]);
			laserDeviceDataMap.Add(1, HasWeakLaserRayDisease() ? 1 : laserRayLevel);
			laserDeviceDataMap.Add(2, HasSlowDetonation() ? 1 : detonateTimeLevel);
			return laserDeviceDataMap;
		}
		
		return null;
	}

	public void IncreasePlantedLaserDeviceAmount(int amount)
	{
		plantedLaserDeviceAmount = Mathf.Max(plantedLaserDeviceAmount + amount, 0);
		plantedLaserDeviceAmount = Mathf.Min(plantedLaserDeviceAmount, laserDeviceAmount);
	}

	public void ChangeSelectedSlot()
	{
		selectedSlot++;
		selectedSlot = selectedSlot > slotList.Count - 1 ? 0 : selectedSlot;
		PutGlobal("selectedSlotSpecialistIndex" + specialistId, selectedSlot);
	}

	public void SetLaserDevice(string laserDeviceType)
	{
		slotList[selectedSlot] = laserDeviceType;
	}

	public void SetSkill(string newSkill)
	{
		skill = newSkill;
		PutGlobal("skillSpecialistIndex" + specialistId, skill);
	}

	public bool HasPushSkill()
	{
		return skill != null && skill.Equals("P");
	}

	public bool HasShootSkill()
	{
		return skill != null && skill.Equals("L");
	}

	public void SetDisease(string newDisease)
	{
		disease = newDisease;
	}

	public virtual void ClearDisease()
	{
		disease = null;
	}

	public bool HasVirusDisease()
	{
		return disease != null && disease.Equals("V");
	}

	protected bool HasVeryHeavyDisease()
	{
		return disease != null && disease.Equals("H");
	}

	protected bool HasWeakLaserRayDisease()
	{
		return disease != null && disease.Equals("R");
	}

	protected bool HasNoLaserDeviceDisease()
	{
		return disease != null && disease.Equals("N");
	}

	protected bool HasSlowDetonation()
	{
		return disease != null && disease.Equals("S");
	}

	protected bool HasSuperSpeedSkill()
	{
		return skill != null && skill.Equals("S");
	}

	protected void UpdateSpecialistMaterial()
	{
		if(specialistId > -1)
		{
			int ci = GetGlobal<int>("colorSpecialistIndex" + specialistId);
			ci = ci < 0 ? 0 : ci;
			visorMeshInstance.SetSurfaceMaterial(0, specialistMaterialList[ci]);
			bodyMeshInstance.SetSurfaceMaterial(0, specialistMaterialList[ci]);
		}
	}

	protected virtual void Initialize()
	{
		laserDeviceDataMap = new Dictionary();
		health = this.TryToGetGlobal<int>(
				"healthSpecialistIndex" + specialistId, 100);
		speedLevel = this.TryToGetGlobal<int>(
				"speedLevelSpecialistIndex" + specialistId, 1);
		laserRayLevel = this.TryToGetGlobal<int>(
				"laserRayLevelSpecialistIndex" + specialistId, 1);
		laserDeviceAmount = this.TryToGetGlobal<int>(
				"laserDeviceAmountSpecialistIndex" + specialistId, 1);
		detonateTimeLevel = this.TryToGetGlobal<int>(
				"detonateTimeLevelSpecialistIndex" + specialistId, 1);
		selectedSlot = this.GetGlobal<int>("selectedSlotSpecialistIndex" + specialistId);
		skill = this.GetGlobal<string>("skillSpecialistIndex" + specialistId);
		slotList = this.GetGlobal<Array>("slotListSpecialistIndex" + specialistId);

		if(health < 1)
			health = 100;
	}

	protected virtual void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		visorMeshInstance = GetNode<MeshInstance>(visorMeshInstanceNP);
		bodyMeshInstance = GetNode<MeshInstance>(bodyMeshInstanceNP);
	}

	protected void PutGlobal(string key, object value)
	{
		globalData.Call(this.GetMethodPut(), key, value);
	}

	protected T GetGlobal<T>(string key)
	{
		return this.Call<T>(globalData, this.GetMethodGet(), key);
	}

	protected T TryToGetGlobal<T>(string key, T defaultValue)
	{
		return this.TryToCall<T>(globalData, this.GetMethodGet(), defaultValue, key);
	}
	
	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
		UpdateSpecialistMaterial();
	}

	public int SpecialistId
	{
		set
		{
			specialistId = value;
		}	
	}

	public float MoveSpeedFactor
	{
		get
		{
			if(HasVeryHeavyDisease())
				return 0.5f;	
			if(HasSuperSpeedSkill())
				return 6f;
			else
				return 1f + ((speedLevel - 1f) * 0.25f);
		}
	}

	public string Skill
	{
		get
		{
			return skill;
		}
	}


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public Array<Material> specialistMaterialList;

	[Export]
	public Vector2 laserRayLevelRange = new Vector2(1f, 18f);

	[Export]
	public Vector2 laserDeviceAmountRange = new Vector2(1f, 9f);

	[Export]
	public Vector2 detonateTimeLevelRange = new Vector2(1f, 7f);

	[Export]
	public NodePath visorMeshInstanceNP;

	[Export]
	public NodePath bodyMeshInstanceNP;


	protected Node globalData;
	protected MeshInstance visorMeshInstance;
	protected MeshInstance bodyMeshInstance;

	protected int specialistId;
	protected int laserRayLevel;
	protected int laserDeviceAmount;
	protected int detonateTimeLevel;
	protected int selectedSlot;
	protected Array slotList;
	protected string skill;
	protected string disease;
	protected int plantedLaserDeviceAmount;
	protected Dictionary laserDeviceDataMap;
}
