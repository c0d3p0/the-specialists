using Godot;
using Godot.Collections;


public class SpecialistStoryHUD : Node
{
	public void OnUpdateTimerTimeout()
	{
		UpdateSignLabelVisibility(false);
	}

	public void UpdateLives(int lives)
	{
		UpdateLabel(LIVES_LABEL_ID, UPDATE_LIVES_LABEL_ID, this.lives, lives);
		this.lives = lives;
	}

	public void UpdateSpeedLevel(int speedLevel)
	{
		UpdateLabel(SPEED_LEVEL_LABEL_ID, UPDATE_SPEED_LEVEL_LABEL_ID,
				this.speedLevel, speedLevel);
		this.speedLevel = speedLevel;
	}

	public void UpdateLaserRayLevel(int laserRayLevel)
	{
		UpdateLabel(LASER_RAY_LEVEL_LABEL_ID, UPDATE_LASER_RAY_LEVEL_LABEL_ID,
				this.laserRayLevel, laserRayLevel);
		this.laserRayLevel = laserRayLevel;
	}

	public void UpdateLaserDeviceAmount(int laserDeviceAmount)
	{
		UpdateLabel(DEVICE_AMOUNT_LABEL_ID, UPDATE_DEVICE_AMOUNT_LABEL_ID,
				this.laserDeviceAmount, laserDeviceAmount);
		this.laserDeviceAmount = laserDeviceAmount;
	}

	public void UpdateDetonateTimeLevel(int detonateTimeLevel)
	{
		UpdateLabel(DETONATE_TIME_LABEL_ID, UPDATE_DETONATE_TIME_LABEL_ID,
				this.detonateTimeLevel, detonateTimeLevel);
		this.detonateTimeLevel = detonateTimeLevel;
	}

	public void UpdateDeviceSlot(int selectedSlot)
	{
		textureButtonMap[SLOT1_BORDER_ID].Disabled = selectedSlot == 0;
		textureButtonMap[SLOT2_BORDER_ID].Disabled = selectedSlot != 0;
	}

	public void UpdateDeviceSlotsContent(Array slotList)
	{
		for(int i = 0; i < slotList.Count; i++)
			labelMap[SLOT1_LABEL_ID + i].Text = slotList[i] as string;
	}

	public void UpdateHealthHUD(int health)
	{
		textureRectMap[ARMOR_PICTURE_ID].Visible = health > 100f;
	}

	public void UpdateSkillHUD(string skill)
	{
		int id = skill == null ? -1 : skill.Equals("P") ? 0 :
				skill.Equals("L") ? 1 : skill.Equals("S") ? 2 : -1;

		if(id > -1)
		{
			textureRectMap[SKILL_PICTURE_ID].Texture = skillTextureList[id];
			textureRectMap[SKILL_PICTURE_ID].Visible = true;
		}
		else
			textureRectMap[SKILL_PICTURE_ID].Visible = false;
	}

	private void UpdateLabel(int labelId, int labelSignId, int oldValue, int value)
	{
		Label l = labelMap[labelId];
		UpdateSignLabel(labelSignId, oldValue, value);
		l.Text = value.ToString();
	}

	private void UpdateSignLabel(int updateLabelId, int oldValue, int newValue)
	{
		Label l = labelMap[updateLabelId];

		if(oldValue > -1 && oldValue != newValue)
		{
			bool increase = newValue > oldValue;
			l.Text = increase ? "+" : "-";
			l.AddColorOverride("font_color", increase ? increaseColor : decreaseColor);
			l.Visible = true;
			updateTimer.Start();
		}
	}

	private void UpdateSignLabelVisibility(bool visible)
	{
		for(int i = UPDATE_LIVES_LABEL_ID; i <= UPDATE_DETONATE_TIME_LABEL_ID; i++)
			labelMap[i].Visible = visible;
	}

	private void UpdateProfilePicture()
	{
		int c = this.GetGlobal<int>("colorSpecialistIndex" + specialistId);
		c = Mathf.Min(Mathf.Max(0, c), specialistTextureList.Count);
		textureRectMap[PROFILE_PICTURE_ID].Texture = specialistTextureList[c];
	}

	private void UpdateHUD()
	{
		int nl = this.TryToGetGlobal<int>("livesSpecialistIndex" + specialistId, lives);
		int nh = this.TryToGetGlobal<int>("healthSpecialistIndex" + specialistId, 0);
		int nsl = this.TryToGetGlobal<int>("speedLevelSpecialistIndex" + specialistId, 1);
		int lrl = this.TryToGetGlobal<int>("laserRayLevelSpecialistIndex" + specialistId, 1);
		int lda = this.TryToGetGlobal<int>("laserDeviceAmountSpecialistIndex" + specialistId, 1);
		int dtl = this.TryToGetGlobal<int>("detonateTimeLevelSpecialistIndex" + specialistId, 1);
		int ss = this.TryToGetGlobal<int>("selectedSlotSpecialistIndex" + specialistId, 1);
		string s = this.GetGlobal<string>("skillSpecialistIndex" + specialistId);
		Array sl = this.GetGlobal<Array>("slotListSpecialistIndex" + specialistId);

		UpdateLives(nl);
		UpdateHealthHUD(nh);
		UpdateDeviceSlotsContent(sl);
		UpdateDeviceSlot(ss);
		UpdateSkillHUD(s);
		UpdateSpeedLevel(nsl);
		UpdateLaserRayLevel(lrl);
		UpdateLaserDeviceAmount(lda);
		UpdateDetonateTimeLevel(dtl);
	}

	private void UpdateGUI()
	{
		bool active = specialistId < specialistAmount &&
				this.TryToGetGlobal<int>("livesSpecialistIndex" + specialistId, lives) > -1;
		dataControl.Visible = active;
		notInBattleControl.Visible = !active;
		SetProcess(active);
	}

	private void Initialize()
	{
		labelMap = new Dictionary<int, Label>();
		textureRectMap = new Dictionary<int, TextureRect>();
		textureButtonMap = new Dictionary<int, TextureButton>();
		specialistAmount = this.GetGlobal<int>("specialistAmount");
		lives = -1;
		speedLevel = -1;
		laserRayLevel = -1;
		laserDeviceAmount = -1;
		detonateTimeLevel = -1;
	}

	private void ObtainNodesFromProfileControl()
	{
		Node pc = GetNode(profileControlNP);
		textureRectMap.Add(PROFILE_PICTURE_ID, pc.GetChild<TextureRect>(1));
		labelMap.Add(LIVES_LABEL_ID, pc.GetChild<Label>(2));
		labelMap.Add(UPDATE_LIVES_LABEL_ID, pc.GetChild<Label>(3));
	}

	private void ObtainNodesFromDeviceSlotControl()
	{
		Node dsc = GetNode(deviceSlotControlNP);
		textureButtonMap.Add(SLOT1_BORDER_ID, dsc.GetChild<TextureButton>(0));
		labelMap.Add(SLOT1_LABEL_ID, dsc.GetChild<Label>(1));
		textureButtonMap.Add(SLOT2_BORDER_ID, dsc.GetChild<TextureButton>(2));
		labelMap.Add(SLOT2_LABEL_ID, dsc.GetChild<Label>(3));
	}

	private void ObtainNodesFromExtrasControl()
	{
		Node ec = GetNode(extrasControlNP);
		textureRectMap.Add(ARMOR_PICTURE_ID, ec.GetChild<TextureRect>(0));
		textureRectMap.Add(SKILL_PICTURE_ID, ec.GetChild<TextureRect>(1));
	}

	private void ObtainNodesFromSpeedLevelControl()
	{
		Node slc = GetNode(speedLevelControlNP);
		labelMap.Add(SPEED_LEVEL_LABEL_ID, slc.GetChild<Label>(1));
		labelMap.Add(UPDATE_SPEED_LEVEL_LABEL_ID, slc.GetChild<Label>(2));
	}

	private void ObtainNodesFromLaserRayLevelControl()
	{
		Node lrlc = GetNode(laserRayLevelControlNP);
		labelMap.Add(LASER_RAY_LEVEL_LABEL_ID, lrlc.GetChild<Label>(1));
		labelMap.Add(UPDATE_LASER_RAY_LEVEL_LABEL_ID, lrlc.GetChild<Label>(2));
	}

	private void ObtainNodesFromDeviceAmountControl()
	{
		Node dac = GetNode(deviceAmountControlNP);
		labelMap.Add(DEVICE_AMOUNT_LABEL_ID, dac.GetChild<Label>(1));
		labelMap.Add(UPDATE_DEVICE_AMOUNT_LABEL_ID, dac.GetChild<Label>(2));
	}

	private void ObtainNodesFromDetonateTimeControl()
	{
		Node dtc = GetNode(detonateTimeControlNP);
		labelMap.Add(DETONATE_TIME_LABEL_ID, dtc.GetChild<Label>(1));
		labelMap.Add(UPDATE_DETONATE_TIME_LABEL_ID, dtc.GetChild<Label>(2));
	}

	private void ObtainNodes()
	{
		updateTimer = GetNode<Timer>(updateTimerNP);
		globalData = GetNode(globalDataNodePath);
		dataControl = GetNode<Control>(dataControlNP);
		notInBattleControl = GetNode<Control>(notInBattleControlNP);
	}

	private T GetGlobal<T>(string key)
	{
		return this.Call<T>(globalData, this.GetMethodGet(), key);
	}

	private T TryToGetGlobal<T>(string key, T defaultValue)
	{
		return this.TryToCall<T>(globalData, this.GetMethodGet(), defaultValue, key);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
		ObtainNodesFromProfileControl();
		ObtainNodesFromDeviceSlotControl();
		ObtainNodesFromExtrasControl();
		ObtainNodesFromSpeedLevelControl();
		ObtainNodesFromLaserRayLevelControl();
		ObtainNodesFromDeviceAmountControl();
		ObtainNodesFromDetonateTimeControl();
		UpdateProfilePicture();
	}

	public override void _Ready()
	{
		UpdateGUI();
	}

	public override void _Process(float delta)
	{
		UpdateHUD();
	}


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath dataControlNP;

	[Export]
	public NodePath notInBattleControlNP;

	[Export]
	public NodePath profileControlNP;

	[Export]
	public NodePath deviceSlotControlNP;

	[Export]
	public NodePath extrasControlNP;

	[Export]
	public NodePath speedLevelControlNP;

	[Export]
	public NodePath laserRayLevelControlNP;

	[Export]
	public NodePath deviceAmountControlNP;

	[Export]
	public NodePath detonateTimeControlNP;

	[Export]
	public NodePath updateTimerNP;

	[Export]
	public Array<StreamTexture> specialistTextureList;

	[Export]
	public Array<StreamTexture> skillTextureList;

	[Export]
	public Color increaseColor = new Color(0.4f, 0.9f, 0.1f, 1f);

	[Export]
	public Color decreaseColor = new Color(0.9f, 0.05f, 0.05f, 1f);

	[Export]
	public int specialistId;


	private Node globalData;
	private Control dataControl;
	private Control notInBattleControl;
	private Dictionary<int, Label> labelMap;
	private Dictionary<int, TextureRect> textureRectMap;
	private Dictionary<int, TextureButton> textureButtonMap;
	private Timer updateTimer;

	private int specialistAmount;
	private int lives;
	private int speedLevel;
	private int laserRayLevel;
	private int laserDeviceAmount;
	private int detonateTimeLevel;

	private const int PROFILE_PICTURE_ID = 0;
	private const int ARMOR_PICTURE_ID = 1;
	private const int SKILL_PICTURE_ID = 2;


	private const int LIVES_LABEL_ID = 10;
	private const int SLOT1_LABEL_ID = 11;
	private const int SLOT2_LABEL_ID = 12;
	private const int SPEED_LEVEL_LABEL_ID = 13;
	private const int LASER_RAY_LEVEL_LABEL_ID = 14;
	private const int DEVICE_AMOUNT_LABEL_ID = 15;
	private const int DETONATE_TIME_LABEL_ID = 16;
	private const int UPDATE_LIVES_LABEL_ID = 17;
	private const int UPDATE_SPEED_LEVEL_LABEL_ID = 18;
	private const int UPDATE_LASER_RAY_LEVEL_LABEL_ID = 19;
	private const int UPDATE_DEVICE_AMOUNT_LABEL_ID = 20;
	private const int UPDATE_DETONATE_TIME_LABEL_ID = 21;

	private const int SLOT1_BORDER_ID = 30;
	private const int SLOT2_BORDER_ID = 31;
}
