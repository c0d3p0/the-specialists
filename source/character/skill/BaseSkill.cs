using Godot;


public abstract class BaseSkill : Node
{
	public virtual bool CanExecuteSkill()
	{
		return true;
	}

	public abstract void ExecuteSkill();


	public Node LevelManager
	{
		set
		{
			levelManager = value; 
		}
	}

	public Node SkillManager
	{
		set
		{
			skillManager = value; 
		}
	}


	protected Node levelManager;
	protected Node skillManager;
}
