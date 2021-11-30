using System.Collections.Generic;


// A constant time amortized data structure that supports index
// based get, insert, remove and search operations.
// It doesn't support duplicates.
public class HashList<T> : Godot.Object
{
	public HashList()
	{
		hashMap = new Dictionary<T, int>();
		list = new List<T>();
	}

	public HashList(int size)
	{
		hashMap = new Dictionary<T, int>(size);
		list = new List<T>(size);
	}

	public T this[int index]
	{
		get
		{
			if(IsValidIndex(index))
				return list[index];

			throw new System.IndexOutOfRangeException();
		}
	}

	public void Add(T item)
	{
		if(!hashMap.ContainsKey(item))
		{
			hashMap.Add(item, list.Count);
			list.Add(item);
		}
	}

	public bool Remove(T item)
	{
		if(hashMap.ContainsKey(item))
		{
			int removedIndex = hashMap[item];
			hashMap.Remove(item);

			if(removedIndex < hashMap.Count)
			{
				T lastValue = list[hashMap.Count];
				list[removedIndex] = lastValue;
				hashMap[lastValue] = removedIndex;
			}

			list.RemoveAt(hashMap.Count);
			return true;
		}

		return false;
	}

	public void RemoveAt(int index)
	{
		if(IsValidIndex(index))
		{
			Remove(list[index]);
			return;
		}

		throw new System.IndexOutOfRangeException();
	}

	public void Clear()
	{
		hashMap.Clear();
			list.Clear();
	}

	public bool Contains(T item)
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


	private IList<T> list;
	private Dictionary<T, int> hashMap;
}
