using System.Text;
using SC = System.Collections;
using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public static class NodeExtension
{	
	public static string GetPrefabPath(this Node gdNode, string prefabPath)
	{
		return new StringBuilder("res://prefab/").Append(prefabPath).
				Append(".tscn").ToString();
	}

	public static string GetScenePath(this Node gdNode, string scenePath)
	{
		return new StringBuilder("res://scene/").Append(scenePath).
				Append(".tscn").ToString();
	}

	public static Dictionary<TK, TV> GetNodeMap<TK, TV>(this Node gdNode,
			Node caller, Dictionary<TK, NodePath> nodePathMap) where TV : Godot.Object
	{
		Dictionary<TK, TV> nodeMap = new Dictionary<TK, TV>();

		if(nodePathMap != null)
		{
			TV value;

			SCG.IEnumerator<SCG.KeyValuePair<TK, NodePath>> it =
					nodePathMap.GetEnumerator();

			while(it.MoveNext())
			{
				value = caller.GetNode<TV>(it.Current.Value);

				if(value != null)
					nodeMap.Add(it.Current.Key, value);
			}
		}
		else
			GD.PushWarning("NodePathMap called by '" + caller.Name + "' is null");
		
		return nodeMap;
	}

	public static Dictionary<TK, TV> AddToNodeMap<TK, TV>(this Node gdNode,
			Node caller, Dictionary<TK, TV> nodeMap, Dictionary<TK, NodePath> nodePathMap)
			where TV : Godot.Object
	{
		if(nodePathMap != null)
		{
			SCG.IEnumerator<SCG.KeyValuePair<TK, NodePath>> it =
					nodePathMap.GetEnumerator();

			while(it.MoveNext())
				nodeMap.Add(it.Current.Key, caller.GetNode<TV>(it.Current.Value));
		}
		else
			GD.PushWarning("NodePathMap called by '" + caller.Name + "' is null");
		
		return nodeMap;
	}

	public static Array<T> GetNodeList<T>(this Node gdNode,
			Node caller, Array<NodePath> nodePathList) where T : Godot.Object
	{
		Array<T> nodeList = new Array<T>();

		if(nodePathList != null)
		{
			SCG.IEnumerator<NodePath> it = nodePathList.GetEnumerator();

			while(it.MoveNext())
				nodeList.Add(caller.GetNode<T>(it.Current));
		}
		else
			GD.PushWarning("NodePathList called by '" + caller.Name + "' is null");
		
		return nodeList;
	}

	public static T[] GetNodes<T>(this Node gdNode,
			Node caller, Array<NodePath> nodePathList) where T : Godot.Object
	{
		if(nodePathList != null)
		{
			int index = 0;
			T[] nodes = new T[nodePathList.Count];
			SCG.IEnumerator<NodePath> it = nodePathList.GetEnumerator();

			while(it.MoveNext())
				nodes[index++] = caller.GetNode<T>(it.Current);

			return nodes;
		}
		else
			GD.PushWarning("NodePathList called by '" + caller.Name + "' is null");
		
		return new T[0];
	}

	public static T[] GetChildren<T>(this Node gdNode, Node parent)
			where T : Godot.Object
	{
		if(parent != null)
		{
			int index = 0;
			T[] nodes = new T[parent.GetChildCount()];
			SC.IEnumerator it = parent.GetChildren().GetEnumerator();

			while(it.MoveNext())
				nodes[index++] = it.Current as T;

			return nodes;
		}
		else
			GD.PushWarning("NodePathList called by '" + parent.Name + "' is null");
		
		return new T[0];
	}

	public static T[] GetChildren<T>(this Node gdNode,
			Node parent, int from, int amount) where T : Godot.Object
	{
		if(parent != null)
		{
			int index = from;
			T[] nodes = new T[parent.GetChildCount()];
			SC.IEnumerator it = parent.GetChildren().GetEnumerator();

			while(it.MoveNext() && index < amount)
			{
				nodes[index - from] = it.Current as T;
				index++;
			}

			return nodes;
		}
		else
			GD.PushWarning("NodePathList called by '" + parent.Name + "' is null");
		
		return new T[0];
	}

	public static string CreateUniqueNodeName(this Node gdNode,
			Node node)
	{
		return new StringBuilder(node.Name).Append('_').
				Append(node.GetInstanceId()).ToString();
	}

	public static bool IsLayerInMask(this Node gdNode, Area area, uint mask)
	{
		return (area.CollisionLayer & mask) != 0;
	}

	public static bool IsLayerInMask(this Node gdNode, PhysicsBody body, uint mask)
	{
		return (body.CollisionLayer & mask) != 0;
	}
}
