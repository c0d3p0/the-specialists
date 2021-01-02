using Godot;
using Godot.Collections;


public class Boss3Skill : BaseSkill
{
	public override bool CanExecuteSkill()
	{
		bool animationSkill = skillAnimationNameList.Count > 0;
		bool itemSkill = itemInstanceList.Count >= maxItemAmount;
		skillId = -1;

		if(animationSkill)
		{
			skillId = this.RandiRange(rng, 0,
					itemSkill ? skillAnimationNameList.Count : skillAnimationNameList.Count - 1);
			return true;
		}
		else if(itemSkill)
		{
			skillId = skillAnimationNameList.Count;
			return true;
		}

		return false;
	}

	public override void ExecuteSkill()
	{
		if(skillId >= skillAnimationNameList.Count)
			TryToActivateItem();
		else if(skillId > -1)
			skillAnimationPlayer.Play(skillAnimationNameList[skillId]);
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

	private void Initialize()
	{
		rng = new RandomNumberGenerator();
		itemInstanceList = new Array<Spatial>();
		taskRunner.Call(this.GetMethodClear());
		taskRunner.Call(this.GetMethodSetActive(), true);
	}

	private void ObtainNodes()
	{
		skillAnimationPlayer = GetNode<AnimationPlayer>(skillAnimationPlayerNP);
		taskRunner = GetNode(taskRunnerNodePath);
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
	public NodePath skillAnimationPlayerNP;

	[Export]
	public PackedScene itemPS;

	[Export]
	public Array<string> skillAnimationNameList;

	[Export]
	public Vector3 itemOffset = new Vector3(0f, 0.5f, 0f);

	[Export]
	public int maxItemAmount = 3;


	private Node taskRunner;
	private AnimationPlayer skillAnimationPlayer;

	private Array<Spatial> itemInstanceList;
	private Spatial currentItem;
	private RandomNumberGenerator rng;
	private int skillId;
}
