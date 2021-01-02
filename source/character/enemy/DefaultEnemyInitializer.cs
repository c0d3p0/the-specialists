using Godot;


public class DefaultEnemyInitializer : Node
{
	protected virtual void ObtainNodes()
	{
		enemyCharacter = GetNode<DefaultEnemyCharacter>(enemyCharacterNP);
		characterPhysics = GetNode<CharacterPhysics>(characterPhysicsNP);
		characterMove = GetNode<CharacterMove>(characterMoveNP);
		enemyAction = GetNode<DefaultEnemyAction>(enemyActionNP);
		enemyStatus = GetNode<DefaultEnemyStatus>(enemyStatusNP);
		enemyBehavior = GetNode(enemyBehaviorNP);
		hurtArea = GetNode<Area>(hurtAreaNP);
	}

	private void InitializeEnemyAction()
	{
		enemyAction.AddUserSignal(this.GetSignalApplyMove());
		enemyAction.AddUserSignal(this.GetSignalIncreaseHealth());

		enemyAction.Connect(this.GetSignalApplyMove(),
				characterMove, this.GetMethodApplyMove());
		enemyAction.Connect(this.GetSignalIncreaseHealth(),
				enemyStatus, this.GetMethodIncreaseHealth());		
	}

	private void InitializeCharacterMove()
	{
		characterMove.AddUserSignal(this.GetSignalIncreaseVelocity());
		characterMove.Connect(this.GetSignalIncreaseVelocity(),
				characterPhysics, this.GetMethodIncreaseVelocity());
	}

	private void InitializeEnemyBehavior()
	{
		enemyBehavior.AddUserSignal(this.GetSignalMove());
		enemyBehavior.AddUserSignal(this.GetSignalExecuteSkill());

		enemyBehavior.Connect(this.GetSignalMove(),
				enemyCharacter, this.GetMethodMove());
		enemyBehavior.Connect(this.GetSignalExecuteSkill(),
				enemyCharacter, this.GetMethodExecuteSkill());
	}
	
	private void InitializeHurtArea()
	{
		hurtArea.AddUserSignal(this.GetSignalHit());
		hurtArea.Connect(this.GetSignalHit(), enemyAction, this.GetMethodHit());
	}
	
	public override void _EnterTree()
	{
		ObtainNodes();
		InitializeCharacterMove();
		InitializeEnemyAction();
		InitializeEnemyBehavior();
		InitializeHurtArea();
	}


	[Export]
	public NodePath enemyCharacterNP;

	[Export]
	public NodePath characterPhysicsNP;

	[Export]
	public NodePath characterMoveNP;

	[Export]
	public NodePath enemyActionNP;

	[Export]
	public NodePath enemyStatusNP;	

	[Export]
	public NodePath enemyBehaviorNP;

	[Export]
	public NodePath hurtAreaNP;


	protected DefaultEnemyCharacter enemyCharacter;
	protected CharacterPhysics characterPhysics;
	protected CharacterMove characterMove;
	protected DefaultEnemyAction enemyAction;
	protected DefaultEnemyStatus enemyStatus;
	protected Node enemyBehavior;
	protected Area hurtArea;
}
