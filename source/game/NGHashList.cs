using SC = System.Collections;
using Godot.Collections;


public class NGHashList : Godot.Object
{
	public NGHashList()
	{
		hashMap = new Dictionary<object, int>();
		list = new SC.ArrayList();
	}

	public NGHashList(int size)
	{
		hashMap = new Dictionary<object, int>();
		list = new SC.ArrayList(size);
	}

  public object this[int index]
	{
		get
		{
			if(IsValidIndex(index))
				return list[index];

			throw new System.IndexOutOfRangeException();
		}
	}

  public void Add(object item)
  {
		if(!hashMap.ContainsKey(item))
		{
			hashMap.Add(item, list.Count);
			list.Add(item);
		}
  }

  public bool Remove(object item)
  {
		if(hashMap.ContainsKey(item))
		{
			int removedIndex = hashMap[item];
			hashMap.Remove(item);

			if(removedIndex < hashMap.Count)
			{
				object lastValue = list[hashMap.Count];
				list[removedIndex] = lastValue;
				hashMap[lastValue] = removedIndex;
			}
			
			return true;
		}

		return false;
  }

  public void RemoveAt(int index)
  {
		if(IsValidIndex(index))
			Remove(list[index]);

		throw new System.IndexOutOfRangeException();
  }

  public void Clear()
  {
		hashMap.Clear();
		list.Clear();
  }

  public bool Contains(object item)
  {
		return hashMap.ContainsKey(item);
  }

  public int Count
	{
		get
		{
			return hashMap.Count;
		}
	}

	private bool IsValidIndex(int index)
	{
		return index > -1 && index < hashMap.Count;
	}

	public override string ToString()
	{
		return hashMap.ToString();
	}


	private SC.ArrayList list;
	private Dictionary<object, int> hashMap;
}
