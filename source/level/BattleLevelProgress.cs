using Godot;
using Godot.Collections;


public class BattleLevelProgress : Node
{
	public void OnSpecialistDead(Spatial specialist)
	{
		HandleSpecialistDeath(specialist);

		if(battleEndTimer.IsStopped() && specialistAliveList.Count < 2)
			battleEndTimer.Start();
	}

	public void ActivateCharacters() // Called by an animation
	{
		for(int i = 0; i < specialistAliveList.Count; i++)
			specialistCharacters[i].Call(this.GetMethodTransitTo(), "active");
	}

	public void OnBattleEndDelayTimerTimeout()
	{
		battleTimer.Paused = true;

		if(specialistAliveList.Count > 0)
		{
			int specialistId = this.Call<int>(specialistAliveList[0],
					this.GetMethodGetSpecialistId());
			string winKey = "winsSpecialistIndex" + specialistId;
			int wins = GetGlobal<int>(winKey);
			int firstTo = GetGlobal<int>("firstTo");
			PutGlobal(winKey, wins + 1);

			if(wins + 1 < firstTo)
				animationStateMachine.Travel("round_won");
			else
				animationStateMachine.Travel("battle_won");
			
			specialistAliveList[0].Call(this.GetMethodCheer());
		}
		else
			animationStateMachine.Travel("round_draw");
	}

	public void OnAirStrikeTimerTimeout()
	{
		laserDeviceAirStrike.Call(this.GetMethodSetAirStrikeLevel(), 1);
	}

	public void OnBattleTimerTimeout()
	{
		laserDeviceAirStrike.Call(this.GetMethodSetAirStrikeLevel(), 2);
	}

	private void HandleSpecialistDeath(Spatial specialist)
	{
		int id = this.Call<int>(specialist, this.GetMethodGetSpecialistId());
		string skill = GetGlobal<string>("battleStartingSkill");
		Array slotList = new Array();
		slotList.Add("S");
		slotList.Add("S");
		PutGlobal("healthSpecialistIndex" + id, 100);
		PutGlobal("speedLevelSpecialistIndex" + id, 1);
		PutGlobal("laserRayLevelSpecialistIndex" + id, 2);
		PutGlobal("laserDeviceAmountSpecialistIndex" + id, 1);
		PutGlobal("detonateTimeLevelSpecialistIndex" + id, 1);
		PutGlobal("slotListSpecialistIndex" + id, slotList);
		PutGlobal("selectedSlotSpecialistIndex" + id, 0);
		PutGlobal("skillSpecialistIndex" + id, skill);
		specialistAliveList.Remove(specialist);
	}

	private void InitializeSpecialistCharacters()
	{
		specialistAliveList = new Array<Spatial>();

		if(specialistCharacters != null)
		{
			int specialistAmount = GetGlobal<int>("specialistAmount");

			for(int i = 0; i < specialistCharacters.Length; i++)
			{
				if(i < specialistAmount)
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

	private void UpdateTimeLabel()
	{
		if(airStrikeTimer.IsStopped())
			timeLabel.Text = string.Format("Hurry Up\n{0:0.000}", battleTimer.TimeLeft);
		else
			timeLabel.Text = string.Format("Time\n{0:0.000}", battleTimer.TimeLeft);
	}

	private void PutGlobal(string key, object value)
	{
		globalData.Call(this.GetMethodPut(), key, value);
	}

	private T GetGlobal<T>(string key)
	{
		return this.Call<T>(globalData, this.GetMethodGet(), key);
	}

	private void Initialize()
	{
		battleRound = GetGlobal<int>("battleRound");
		battleRound = battleRound == 0 ? 1 : battleRound;
		float brt = GetGlobal<float>("battleRoundTime");
		brt = brt < 180f ? 180f : brt;
		battleTimer.WaitTime = brt;
		airStrikeTimer.WaitTime = brt - 90f;
		timeLabel.Text = string.Format("Time\n{0:0.000}", brt);
		introLabel.Text = "Round " + battleRound;
		animationStateMachine.Travel(bgmAnimationName);
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		introLabel = GetNode<Label>(introLabelNP);
		timeLabel = GetNode<Label>(timeLabelNP);
		airStrikeTimer = GetNode<Timer>(airStrikeTimerNP);
		battleTimer = GetNode<Timer>(battleTimerNP);
		battleEndTimer = GetNode<Timer>(battleEndTimerNP);
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).Get(
				"parameters/playback") as AnimationNodeStateMachinePlayback;
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
		InitializeSpecialistCharacters();
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

	public Node LaserDeviceAirStrike
	{
		set
		{
			laserDeviceAirStrike = value;
		}
	}

	public Spatial[] SpecialistCharacters
	{
		set
		{
			specialistCharacters = value;
		}
	}


	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath introLabelNP;

	[Export]
	public NodePath timeLabelNP;

	[Export]
	public NodePath airStrikeTimerNP;

	[Export]
	public NodePath battleTimerNP;

	[Export]
	public NodePath battleEndTimerNP;

	[Export]
	public NodePath animationTreeNP;


	private string bgmAnimationName;
	private Node laserDeviceAirStrike;
	private Spatial[] specialistCharacters;

	private Node globalData;
	private Label introLabel;
	private Label timeLabel;
	private Timer battleTimer;
	private Timer airStrikeTimer;
	private Timer battleEndTimer;
	private AnimationNodeStateMachinePlayback animationStateMachine;

	private Array<Spatial> specialistAliveList;
	private int battleRound;
}
