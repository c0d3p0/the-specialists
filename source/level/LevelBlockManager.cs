using Godot;
using Godot.Collections;


public class LevelBlockManager : Node
{
	public Vector3 GetPositionFromRandomDefaultBlockSlot()
	{
		int halfRow = (System.Convert.ToInt32(blockSlotAmount.x) - 1) / 2;
		int halfColumn = (System.Convert.ToInt32(blockSlotAmount.y) - 1) / 2;
		return new Vector3(this.RandiRange(rng, -halfColumn, halfColumn),
					0f, this.RandiRange(rng, -halfRow, halfRow));
	}

	public Vector3 GetPositionFromRandomEmptyBlockSlot()
	{
		int r = this.RandiRange(rng, 0, emptyBlockSlotList.Count - 1);
		return ConvertBlockSlotPositionToTranslation((Vector2) emptyBlockSlotList[r]);
	}

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

	private void RepositionEnemies()
	{
		if(enemyCharacters != null)
		{
			Vector3 translation;
			int randomIndex;

			for(int i = 0; i < enemyCharacters.Length; i++)
			{
				randomIndex = this.RandiRange(rng, 0, emptyBlockSlotList.Count - 1);
				translation = ConvertBlockSlotPositionToTranslation(
						(Vector2) emptyBlockSlotList[randomIndex]);
				translation.y = enemyCharacters[i].Translation.y;
				enemyCharacters[i].Translation = translation;
			}
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

		for(int i = 0; i < itemPrefabList.Count; i++)
		{
			for(int j = 0; j < itemAmountList[i]; j++)
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
	}

  public override void _EnterTree()
  {
		Initialize();
		CreateItems();
		InitializeEmptyBlockSlotList();
		AddPreDefinedSoftBlocks();
		AddSoftBlocksAtRandomPositions();
		RepositionEnemies();
		AddForcedEmptySlotFromEmptyBlockSlotList();
  }

  public override void _ExitTree()
  {
		emptyBlockSlotList.Clear();
		emptyBlockSlotList.Dispose();
  }

	public Spatial[] EnemyCharacters
	{
		set
		{
			enemyCharacters = value;
		}
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

	public Array<int> ItemAmountList
  {
		set
		{
			itemAmountList = value;
		}
  }


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
	private Spatial[] enemyCharacters;
	private Vector2 softBlockAmountRange;
	private Array<int> itemAmountList;
	
	private RandomNumberGenerator rng;

	private NGHashList emptyBlockSlotList;
	private Array<Spatial> itemToAddList;
}
