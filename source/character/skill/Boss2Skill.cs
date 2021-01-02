using Godot;
using Godot.Collections;


public class Boss2Skill : BaseSkill
{
	public override bool CanExecuteSkill()
	{
		if(skillAnimationNameList.Count > 0)
		{
			skillIndex = this.RandiRange(rng, 0, skillAnimationNameList.Count - 1);
			return true;
		}

		return false;
	}

	public override void ExecuteSkill()
	{
		skillAnimationPlayer.Play(skillAnimationNameList[skillIndex]);
	}

	private void Initialize()
	{
		rng = new RandomNumberGenerator();
	}

	private void ObtainNodes()
	{
		skillAnimationPlayer = GetNode<AnimationPlayer>(skillAnimationPlayerNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}


	[Export]
	public NodePath skillAnimationPlayerNP;

	[Export]
	public Array<string> skillAnimationNameList;


	public AnimationPlayer skillAnimationPlayer;

	private RandomNumberGenerator rng;
	private int skillIndex;
}
