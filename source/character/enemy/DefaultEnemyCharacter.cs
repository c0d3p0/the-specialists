using Godot;


public class DefaultEnemyCharacter : KinematicBody
{
	public void Move(Vector3 direction, Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), enemyAction.Move(direction));
	}

	public void Move(Vector3 direction, float moveSpeedFactor,
			string animationName, Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(),
				enemyAction.Move(direction, moveSpeedFactor, animationName));
	}

	public void ExecuteSkill(Vector3 direction, Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(),
				enemyAction.ExecuteSkill(direction));
	}

	public void ExecuteSkill(Vector3 direction, string animationName, Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(),
				enemyAction.ExecuteSkill(direction, animationName));
	}

	public void CanExecuteSkill(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), enemySkill.CanExecuteSkill());
	}

	public void SetIgnoreHit(bool active)
	{
		enemyAction.IgnoreHit = active;
	}

	public void FixBodyDirection(Vector3 direction)
	{
		enemyAction.FixBodyDirection(direction);
	}

	public void TransitTo(string actionName)
	{
		enemyAction.TransitTo(actionName);
	}

	public void SetProcessBehavior(bool active)
	{
		enemyBehavior.SetProcess(active);
		enemyBehavior.SetPhysicsProcess(active);
	}

	public void IsDead(Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(), enemyStatus.Dead);
	}

	protected virtual void Initialize()
	{
		if(enemySkill != null)
		{
			enemySkill.LevelManager = levelManager;
			enemySkill.SkillManager = skillManager;
		}
	}

	protected virtual void ObtainNodes()
	{
		enemyAction = GetNode<DefaultEnemyAction>(enemyActionNP);
		enemyStatus = GetNode<DefaultEnemyStatus>(enemyStatusNP);
		enemyBehavior = GetNode(enemyBehaviorNP);

		if(levelManagerNP != null)
			levelManager = GetNode(levelManagerNP);

		if(skillManagerNP != null)
			skillManager = GetNode(skillManagerNP);
		
		if(enemySkillNP != null)
			enemySkill = GetNode<BaseSkill>(enemySkillNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}


	[Export]
	public NodePath levelManagerNP;

	[Export]
	public NodePath skillManagerNP;

	[Export]
	public NodePath enemyActionNP;

	[Export]
	public NodePath enemyStatusNP;

	[Export]
	public NodePath enemyBehaviorNP;

	[Export]
	public NodePath enemySkillNP;


	protected Node levelManager;
	protected Node skillManager;
	protected DefaultEnemyAction enemyAction;
	protected DefaultEnemyStatus enemyStatus;
	protected Node enemyBehavior;
	protected BaseSkill enemySkill;
}
