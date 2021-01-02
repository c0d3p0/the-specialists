using Godot;
using Godot.Collections;


public class PreInstance : Node
{
	public void OnInstanceEnteredTree()
	{
		instanceInTheTree++;

		if(OS.IsDebugBuild())
			GD.PushWarning("InstanceInTheTree: " + instanceInTheTree);
	}

	private void CreateInstances()
	{
		Array resourceList = this.Call<Array>(globalResource, this.GetMethodGetAllAsList());
		instanceList = new Array<Node>();
		PackedScene ps;
		Node node;

		for(int i = 0; i < resourceList.Count; i++)
		{
			ps = (resourceList[i] as PackedScene);
			
			if(ps != null)
			{
				node = ps.Instance();
				node.Connect("tree_entered", this, nameof(OnInstanceEnteredTree));
				instanceList.Add(node);

				if(OS.IsDebugBuild())
					GD.PushWarning("NodeInstance: " + node.Name);
			}
		}

		instanceAmount = instanceList.Count;
		requestFinished = true;

		if(clearGlobalResources)
			globalResource.Call(this.GetMethodClear());

		if(OS.IsDebugBuild())
			GD.Print("InstanceAmount: " + instanceAmount);
	}

	private void StartRequest()
	{
		if(!requestStarted)
		{
			requestStarted = true;
			Node taskRunner = GetNode(taskRunnerNodePath);
			taskRunner.Call(this.GetMethodPut(), this,
					nameof(CreateInstances));
		}
	}

	private void AddInstancesToTheScene()
	{
		if(requestFinished && instanceList.Count > 0)
		{
			Node nodeInstance = instanceList[0];
			instanceContainer.CallDeferred(this.GetGDMethodAddChild(), nodeInstance);
			instanceList.RemoveAt(0);
			
			if(OS.IsDebugBuild())
				GD.PushWarning("AddingNode: " + nodeInstance.Name);
		}
	}

	private void RemoveInstance()
	{
		if(instanceContainer.GetChildCount() > 2)
		{
			Node node = instanceContainer.GetChild(0);
			node.QueueFree();
		}		
	}

	private void Finish()
	{
		if(requestFinished && instanceInTheTree >= instanceAmount)
		{
			nextNode.SetProcess(true);
			SetProcess(false);
		}
	}


	private void ObtainNodes()
	{
		globalResource = GetNode(globalResourceNodePath);
		nextNode = GetNode(nextNodeNP);
		instanceContainer = GetNode(instanceContainerNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}

	public override void _Ready()
	{
		SetProcess(process);
	}

	public override void _Process(float delta)
	{
		StartRequest();
		AddInstancesToTheScene();
		RemoveInstance();
		Finish();
	}


	[Export]
	public string globalResourceNodePath = "/root/GlobalResource";

	[Export]
	public string taskRunnerNodePath = "/root/TaskRunner";

	[Export]
	public NodePath nextNodeNP;

	[Export]
	public NodePath instanceContainerNP;

	[Export]
	public bool process;

	[Export]
	public bool clearGlobalResources;


	private Node globalResource;
	private Node nextNode;
	private Node instanceContainer;
	private Array<Node> instanceList;
	private int instanceAmount;
	private bool requestFinished;
	private bool requestStarted;
	private int instanceInTheTree;
}
