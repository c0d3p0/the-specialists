using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class LoadResource : Node
{
	private void LoadResources()
	{
		if(resourcePathList != null)
		{
			int index;
			string[] split;
			SCG.IEnumerator<string> it = resourcePathList.GetEnumerator();

			while(it.MoveNext())
			{
				split = it.Current.Split("/");
				index = split.Length - 1;
				Resource res = ResourceLoader.Load(it.Current);

				if(res != null)
					globalResource.Call(this.GetMethodPut(), split[index].Split(".")[0], res);
			}
		}

		loaded = true;
	}

	private void HandleFinished()
	{
		if(loaded)
		{
			nextNode.SetProcess(true);
			SetProcess(false);
		}
	}

	private void Initialize()
	{
		globalResource = GetNode(globalResourceNodePath);
		nextNode = GetNode(nextNodeNP);
		
		globalData = GetNode(globalDataNodePath);
		PutGlobal("sceneToLoad", this.GetScenePath(nextScreenScenePath));
	}

	private void StartRequest()
	{
		Node taskRunner = GetNode(taskRunnerNodePath);
		taskRunner.Call(this.GetMethodPut(), this, nameof(LoadResources));
	}

	private void PutGlobal(string key, object value)
	{
		globalData.Call(this.GetMethodPut(), key, value);
	}

	public override void _EnterTree()
	{
		Initialize();
	}

	public override void _Ready()
	{
		StartRequest();
	}

	public override void _Process(float delta)
	{
		HandleFinished();
	}


	[Export]
	public Array<string> resourcePathList;

	[Export]
	public string nextScreenScenePath = "screen/title_screen";

	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public string globalResourceNodePath = "/root/GlobalResource";

	[Export]
	public string taskRunnerNodePath = "/root/TaskRunner";

	[Export]
	public NodePath nextNodeNP;


	private Node globalData;
	private Node globalResource;
	private Node nextNode;
	private bool loaded;
}
