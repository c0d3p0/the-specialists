using Godot;
using Godot.Collections;


public class Boss1Skill : BaseSkill
{
	public override bool CanExecuteSkill()
	{
		if(IsProjectileAvailable() &&
				(itemInstanceList.Count < maxItemAmount ||
				this.RandiRange(rng, 0, 1) == 1))
		{
			RequestProjectiles();
			skillId = 1;
			return true;
		}
		
		bool itemSkill = itemInstanceList.Count >= maxItemAmount;
		skillId = itemSkill ? 0 : -1;
		return itemSkill;
	}

	public override void ExecuteSkill()
	{
		if(skillId == 0)
			TryToActivateItem();
		else if(skillId == 1)
			TryToActivateProjectile();
	}

	public void CreateItemInstance()
	{
		if(currentItem == null)
			currentItem = itemPS.Instance() as Spatial;
	}

	public void OnItemInsideTree()
	{
		itemInstanceList.Add(currentItem);
		currentItem = null;
	}

	private void RequestProjectiles()
	{
		Spatial projectile;
		projectileList.Clear();

		for(int i = projectileList.Count; i < projectileAmount; i++)
		{
			projectile = this.Call<Array>(skillManager,
					this.GetMethodRequest(), character, projectileType)[0] as Spatial;
			projectile.Translation = this.Call<Vector3>(levelManager,
					this.GetMethodGetPositionFromRandomEmptyBlockSlot()) + projectileOffset;
			projectile.Call(this.GetMethodSetCharacter(), character);
			projectileList.Add(projectile);
		}
	}

	private void TryToActivateItem()
	{
		if(itemInstanceList.Count > 0)
		{
			Spatial item = itemInstanceList[0];
			itemInstanceList.RemoveAt(0);
			item.Call(this.GetMethodTransitTo(), "active");
			item.Translation = this.Call<Vector3>(levelManager,
					this.GetMethodGetPositionFromRandomEmptyBlockSlot()) + itemOffset;
		}
	}

	private void TryToActivateProjectile()
	{
		if(projectileList.Count > 0)
		{
			Spatial projectile = projectileList[0];
			projectile.Call(this.GetMethodTransitTo(), "active");
			projectile.Call(this.GetMethodSetDirection(), projectileDirection);
			projectileList.RemoveAt(0);
		}
	}

	private void AddItemToTheScene()
	{
		if(currentItem != null)
		{
			this.AddChildToItemContainer(this, currentItem);
			currentItem.Connect(this.GetGDSignalTreeEntered(),
					this, nameof(OnItemInsideTree));
		}
	}

	private void QueueCreateItemInstanceTask()
	{
		if(itemInstanceList.Count < maxItemAmount)
			taskRunner.Call(this.GetMethodPut(), this, nameof(CreateItemInstance));
	}

	private bool IsProjectileAvailable()
	{
		return this.Call<bool>(skillManager,
				this.GetMethodIsAvailable(), projectileType, projectileAmount);
	}

	private void Initialize()
	{
		rng = new RandomNumberGenerator();
		itemInstanceList = new Array<Spatial>();
		projectileList = new Array<Spatial>();
		taskRunner.Call(this.GetMethodClear());
		taskRunner.Call(this.GetMethodSetActive(), true);
	}

	private void ObtainNodes()
	{
		taskRunner = GetNode(taskRunnerNodePath);
		character = GetNode(characterNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
	}

	public override void _Process(float delta)
	{
		QueueCreateItemInstanceTask();
		AddItemToTheScene();
	}

	public override void _ExitTree()
	{
		taskRunner.Call(this.GetMethodSetActive(), false);
	}


	[Export]
	public NodePath taskRunnerNodePath = "/root/TaskRunner";

	[Export]
	public NodePath characterNP;

	[Export]
	public PackedScene itemPS;

	[Export]
	public Vector3 itemOffset = new Vector3(0f, 0.5f, 0f);

	[Export]
	public Vector3 projectileOffset = new Vector3(0f, 20f, 0f);

	[Export]
	public Vector3 projectileDirection = Vector3.Down;

	[Export]
	public int maxItemAmount = 2;

	[Export]
	public int projectileAmount = 5;

	[Export]
	public string projectileType = "I";


	private Node taskRunner;
	private Node character;

	private RandomNumberGenerator rng;
	private Array<Spatial> itemInstanceList;
	private Array<Spatial> projectileList;
	private Spatial currentItem;
	private int skillId;
}
