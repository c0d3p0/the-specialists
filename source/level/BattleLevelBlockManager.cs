using Godot;
using Godot.Collections;


public class BattleLevelBlockManager : Node
{
	public void AddEmptyBlockSlot(Vector3 translation)
	{
		emptyBlockSlotList.Add(
				ConvertTranslationToBlockSlotPosition(translation));
	}

	public void RemoveEmptyBlockSlot(Vector3 translation)
	{
		emptyBlockSlotList.Remove(
				ConvertTranslationToBlockSlotPosition(translation));
	}

	public bool ContainsEmptyBlockSlot(Vector3 translation)
	{
		return emptyBlockSlotList.Contains(
				ConvertTranslationToBlockSlotPosition(translation));
	}
	
	public void OnSoftBlockDestroyed(Spatial softBlock)
	{
		emptyBlockSlotList.Add(
				ConvertTranslationToBlockSlotPosition(softBlock.Translation));
	}

	private void AddSoftBlocksAtRandomPositions()
	{
		int softBlockAmount = this.RandiRange(rng,
				System.Convert.ToInt32(softBlockAmountRange.x),
				System.Convert.ToInt32(softBlockAmountRange.y));

		if(softBlockPrefabList != null)
		{
			int softBlocksLeft = softBlockAmount - preDefinedSoftBlockSlotList.Count;
			int randomIndex;

			for(int i = 0; i < softBlocksLeft; i++)
			{
				randomIndex = this.RandiRange(rng, 0, emptyBlockSlotList.Count - 1);
				AddSoftBlock((Vector2) emptyBlockSlotList[randomIndex], true);
			}
		}
	}

	private void AddPreDefinedSoftBlocks()
	{
		if(preDefinedSoftBlockSlotList != null && softBlockPrefabList != null)
		{
			for(int i = 0; i < preDefinedSoftBlockSlotList.Count; i++)
				AddSoftBlock(preDefinedSoftBlockSlotList[i], false);
		}
	}

	private void AddSoftBlock(Vector2 blockSlot, bool tryToStoreItem)
	{
		int randomIndex = this.RandiRange(rng, 0, softBlockPrefabList.Count - 1);
		Spatial sb = softBlockPrefabList[randomIndex].Instance() as Spatial;
		this.AddChildToBlockContainer(this, sb);
		emptyBlockSlotList.Remove(blockSlot);
		sb.Name = this.CreateUniqueNodeName(sb);
		sb.Translation = ConvertBlockSlotPositionToTranslation(blockSlot);
		sb.Connect(this.GetGDSignalTreeExited(), this,
				nameof(OnSoftBlockDestroyed), this.CreateSingleBind(sb));
		
		if(tryToStoreItem)
			AddItemToSoftBlock(sb);
	}

	private void AddItemToSoftBlock(Spatial softBlock)
	{
		if(itemToAddList.Count > 0)
		{
			Spatial item = itemToAddList[0];
			item.Name = this.CreateUniqueNodeName(item);
			this.AddChildToItemContainer(this, item);
			itemToAddList.RemoveAt(0);
			softBlock.Call(this.GetMethodSetItem(), item);
		}
	}

	private Vector2 ConvertTranslationToBlockSlotPosition(Vector3 translation)
	{
		return new Vector2(translation.z + ((blockSlotAmount.x - 1) / 2),
				translation.x + ((blockSlotAmount.y - 1) / 2));
	}

	private Vector3 ConvertBlockSlotPositionToTranslation(Vector2 blockSlotPosition)
	{
		return new Vector3(((blockSlotAmount.y - 1) / -2) + blockSlotPosition.y, 0f,
				((blockSlotAmount.x - 1) / -2) + blockSlotPosition.x) + blockOffset;
	}

	private void CreateItems()
	{
		itemToAddList = new Array<Spatial>();
		int itemAmount;
		Vector2 iar;

		for(int i = 0; i < itemPrefabList.Count; i++)
		{
			iar = itemAmountRangeList[i] * specialistAmount;
			itemAmount = this.RandiRange(rng, System.Convert.ToInt32(iar.x),
					System.Convert.ToInt32(iar.y));

			for(int j = 0; j < itemAmount; j++)
				itemToAddList.Add(itemPrefabList[i].Instance() as Spatial);
		}
	}

	private void AddDefaultEmptyBlockSlotToEmptyBlockSlotList()
	{
		for(int i = 0; i < blockSlotAmount.x; i++)
		{
			for(int j = 0; j < blockSlotAmount.y; j++)
			{
				if(i % 2 == 0 || j % 2 == 0)
					emptyBlockSlotList.Add(new Vector2(i, j));
			}
		}
	}

	private void RemoveForcedEmptySlotFromEmptyBlockSlotList()
	{
		for(int i = 0; i < forceEmptyBlockSlotList.Count; i++)
			emptyBlockSlotList.Remove(forceEmptyBlockSlotList[i]);
	}

	private void AddForcedEmptySlotFromEmptyBlockSlotList()
	{
		for(int i = 0; i < forceEmptyBlockSlotList.Count; i++)
			emptyBlockSlotList.Add(forceEmptyBlockSlotList[i]);
	}

	private void InitializeEmptyBlockSlotList()
	{
		emptyBlockSlotList = new NGHashList(
				System.Convert.ToInt32(blockSlotAmount.x * blockSlotAmount.y));
		AddDefaultEmptyBlockSlotToEmptyBlockSlotList();
		RemoveForcedEmptySlotFromEmptyBlockSlotList();
	}

	private void Initialize()
	{
		rng = new RandomNumberGenerator();
		specialistAmount = GetGlobal<int>("specialistAmount");
		specialistAmount = specialistAmount > 1 ? specialistAmount : 2;
	}

	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
	}

	protected T GetGlobal<T>(string key)
	{
		return this.Call<T>(globalData, this.GetMethodGet(), key);
	}

  public override void _EnterTree()
  {
		ObtainNodes();
		Initialize();
		CreateItems();
		InitializeEmptyBlockSlotList();
		AddSoftBlocksAtRandomPositions();
		AddPreDefinedSoftBlocks();
		AddForcedEmptySlotFromEmptyBlockSlotList();
  }

  public override void _ExitTree()
  {
		emptyBlockSlotList.Clear();
		emptyBlockSlotList.Dispose();
  }

	public Array<PackedScene> SoftBlockPrefabList
  {
		set
		{
			softBlockPrefabList = value;
		}
  }

	public Array<PackedScene> ItemPrefabList
  {
		set
		{
			itemPrefabList = value;
		}
  }

	public Vector2 SoftBlockAmountRange
  {
		set
		{
			softBlockAmountRange = value;
		}
  }

	public Array<Vector2> ItemAmountRangeList
  {
		set
		{
			itemAmountRangeList = value;
		}
  }



	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public Vector2 blockSlotAmount = new Vector2(13f, 21f);

	[Export]
	public Array<Vector2> preDefinedSoftBlockSlotList;

	[Export]
	public Array<Vector2> forceEmptyBlockSlotList;

	[Export]
	public Vector3 blockOffset = new Vector3(0f, 0.5f, 0f);


	private Array<PackedScene> softBlockPrefabList;
	private Array<PackedScene> itemPrefabList;
	private Vector2 softBlockAmountRange;
	public Array<Vector2> itemAmountRangeList;
	
	private Node globalData;
	private RandomNumberGenerator rng;
	private NGHashList emptyBlockSlotList;
	private Array<Spatial> itemToAddList;
	private int specialistAmount;
}

