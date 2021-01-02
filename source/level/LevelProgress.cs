using Godot;


public class LevelProgress : Node
{
	public void OnEnemyCharacterDead(Spatial enemyCharacter)
	{
		enemyAliveList.Remove(enemyCharacter);
		bossAliveList.Remove(enemyCharacter);

		if(enemyAliveList.Count + bossAliveList.Count < 1)
			exitGate.Call(this.GetMethodUnlock(), true);
	}

	public void OnSpecialistDead(Spatial specialist)
	{
		if(!levelCleared)
		{
			if(specialistAliveList.Contains(specialist))
				HandleSpecialistDeath(specialist);

			if(specialistAliveList.Count < 1)
				HandleLevelFailed();
		}
	}

	public void OnObjectiveCompleted()
	{
		UpdateGameplayTotalTime();
		animationStateMachine.Travel("next_level");
		levelCleared = true;

		for(int i = 0; i < specialistAliveList.Count; i++)
			specialistAliveList[i].Call(this.GetMethodCheer());
	}

	public void OnLevelTimerTimeout()
	{
		if(!infiniteTime && !levelCleared)
		{
			Spatial specialist;

			while(specialistAliveList.Count > 0)
			{
				specialist = specialistAliveList[0];
				specialist.Call(this.GetMethodSetProcessBehavior(), false);
				specialist.Call(this.GetMethodTransitTo(), "inactive");
				HandleSpecialistDeath(specialist);
			}

			HandleLevelFailed();
		}
	}

	public void UpdateIntroLabel()
	{
		introLabel.Text = levelName.Replace("&", "\n");
	}

	public void StartLevel() // Called by an animation
	{
		startTime = OS.GetTicksMsec();
		ForceCharactersToTransitTo(specialistAliveList, "active");
		ForceCharactersToTransitTo(enemyAliveList, "active");
		ForceCharactersToTransitTo(bossAliveList, "active");
	}

	private void ForceCharactersToTransitTo(
			HashList<Spatial> characterAliveList, string animation)
	{
		for(int i = 0; i < characterAliveList.Count; i++)
		{
			if(!this.Call<bool>(characterAliveList[i], this.GetMethodIsDead()))
				characterAliveList[i].Call(this.GetMethodTransitTo(), animation);
		}
	}

	private void HandleLevelFailed()
	{
		UpdateGameplayTotalTime();
		ForceCharactersToTransitTo(enemyAliveList, "inactive");
		ForceCharactersToTransitTo(bossAliveList, "inactive");

		if(ShouldReloadLevel())
			animationStateMachine.Travel("reload");
		else
			animationStateMachine.Travel("continue");
	}

	private void HandleSpecialistDeath(Spatial specialist)
	{
		int id = this.Call<int>(specialist, this.GetMethodGetSpecialistId());
		string key = this.CreateString("p", (id + 1), "Deaths");
		PutGlobal(key, GetGlobal<int>(key) + 1);

		specialistAliveList.Remove(specialist);
		specialist.Call(this.GetMethodApplyItem(),
				this.GetMethodIncreaseLives(), -1);
		specialist.Call(this.GetMethodApplyItem(),
				this.GetMethodIncreaseSpeedLevel(), -1);
		specialist.Call(this.GetMethodApplyItem(),
				this.GetMethodIncreaseLaserRayLevel(), -2);
		specialist.Call(this.GetMethodApplyItem(),
				this.GetMethodIncreaseLaserDeviceAmount(), -1);
	}

	private void InitializeSpecialistCharacters()
	{
		specialistAliveList = new HashList<Spatial>();

		if(specialistCharacters != null)
		{
			int specialistAmount = GetGlobal<int>("specialistAmount");

			for(int i = 0; i < specialistCharacters.Length; i++)
			{
				if(i < specialistAmount && GetGlobal<int>("livesSpecialistIndex" + i) > -1)
				{
					specialistAliveList.Add(specialistCharacters[i]);
					specialistCharacters[i].Connect(this.GetGDSignalTreeExited(),
							this, nameof(OnSpecialistDead),
							this.CreateSingleBind(specialistCharacters[i]));
				}
				else
				{
					specialistCharacters[i].Visible = false;
					specialistCharacters[i].QueueFree();
				}
			}
		}
	}

	private void UpdateGameplayTotalTime()
	{
		long totalTime = System.Convert.ToInt64(GetGlobal<string>("totalTime"));
		totalTime += OS.GetTicksMsec() - startTime;
		PutGlobal("totalTime", totalTime.ToString());
	}

	private bool ShouldReloadLevel()
	{
		bool srl = false;

		for(int i = 0; i < specialistAmount; i++)
			srl |= GetGlobal<int>("livesSpecialistIndex" + i) > -1;

		return srl;
	}

	private void UpdateTimeLabel()
	{
		if(!infiniteTime)
			timeLabel.Text = string.Format("Time\n{0:0.000}", levelTimer.TimeLeft);
	}

	private void InitializeTimer()
	{
		if(!infiniteTime)
		{
			levelTimer.WaitTime = levelTime;
			timeLabel.Text = string.Format("Time\n{0:0.000}", levelTime);
		}
	}

	private void Initialize()
	{
		specialistAmount = GetGlobal<int>("specialistAmount");
		timeHUD.Visible = !infiniteTime;
		animationStateMachine.Travel(bgmAnimationName);
	}

	private void InitializeEnemyCharacters()
	{
		enemyAliveList = new HashList<Spatial>();
		InitializeCPUCharacters(enemyCharacters, enemyAliveList);
	}

	private void InitializeBossCharacters()
	{
		bossAliveList = new HashList<Spatial>();
		InitializeCPUCharacters(bossCharacters, bossAliveList);
	}

	private void InitializeCPUCharacters(Spatial[] cpuCharacters,
			HashList<Spatial> charactersAliveList)
	{
		if(cpuCharacters != null)
		{
			for(int i = 0; i < cpuCharacters.Length; i++)
			{
				charactersAliveList.Add(cpuCharacters[i]);
				cpuCharacters[i].Connect(this.GetGDSignalTreeExited(),
						this, nameof(OnEnemyCharacterDead),
						this.CreateSingleBind(cpuCharacters[i]));
			}
		}
	}
	
	private void ObtainNodes()
	{
		timeHUD = GetNode<Control>(timeHUDNP);
		globalData = GetNode(globalDataNodePath);
		introLabel = GetNode<Label>(introLabelNP);
		levelTimer = GetNode<Timer>(levelTimerNP);
		timeLabel = GetNode<Label>(timeLabelNP);
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).Get(
				"parameters/playback") as AnimationNodeStateMachinePlayback;
	}

	protected void PutGlobal(string key, object value)
	{
		globalData.Call(this.GetMethodPut(), key, value);
	}

	private T GetGlobal<T>(string key)
	{
		return this.Call<T>(globalData, this.GetMethodGet(), key);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
		InitializeTimer();
		InitializeSpecialistCharacters();
		InitializeEnemyCharacters();
		InitializeBossCharacters();
	}

	public override void _Process(float delta)
	{
		UpdateTimeLabel();
	}

	public override void _Ready()
	{
		SetProcess(false);
	}

	public string BgmAnimationName
	{
		set
		{
			bgmAnimationName = value;
		}
	}

	public string LevelName
	{
		set
		{
			levelName = value;
		}
	}

	public float LevelTime
	{
		set
		{
			levelTime = value;
		}
	}

	public bool InfiniteTime
	{
		set
		{
			infiniteTime = value;
		}
	}

	public Node ExitGate
	{
		set
		{
			exitGate = value;
		}
	}

	public Spatial[] SpecialistCharacters
	{
		set
		{
			specialistCharacters = value;
		}
	}

	public Spatial[] EnemyCharacters
	{
		set
		{
			enemyCharacters = value;
		}
	}

	public Spatial[] BossCharacters
	{
		set
		{
			bossCharacters = value;
		}
	}


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath introLabelNP;

	[Export]
	public NodePath timeHUDNP;

	[Export]
	public NodePath timeLabelNP;

	[Export]
	public NodePath levelTimerNP;

	[Export]
	public NodePath animationTreeNP;

	[Export]
	public bool infiniteTime;


	private string bgmAnimationName;
	private string levelName;
	private float levelTime;
	private Node exitGate;
	private Spatial[] specialistCharacters;
	private Spatial[] enemyCharacters;
	private Spatial[] bossCharacters;

	private Node globalData;
	private Control timeHUD;
	private Label introLabel;
	private Label timeLabel;
	private Timer levelTimer;
	private AnimationNodeStateMachinePlayback animationStateMachine;

	private HashList<Spatial> specialistAliveList;
	private HashList<Spatial> enemyAliveList;
	private HashList<Spatial> bossAliveList;
	private int specialistAmount;
	private bool levelCleared;
	private long startTime;
}
