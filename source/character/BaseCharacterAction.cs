using Godot;
using Godot.Collections;


public class BaseCharacterAction : Node
{
	public void FixBodyDirection(Vector3 direction)
	{
		Vector2 key = new Vector2(direction.x, direction.z);

		if(bodyRotationMap.ContainsKey(key))
			body.RotationDegrees = new Vector3(0f, bodyRotationMap[key], 0f);
	}

	public Vector3 GetBodyDirection()
	{
		float key = Mathf.Round(body.RotationDegrees.y);

		if(bodyDirectionMap.ContainsKey(key))
			return bodyDirectionMap[key];

		return Vector3.Forward;
	}

	public void TransitTo(string actionName)
	{
		animationStateMachine.Travel(actionName);
	}

	public void SetIgnoreHit(bool active)
	{
		ignoreHit = active;
	}

	protected string CanTransitToMove()
	{
		return GetValue(toMoveMap, animationStateMachine.GetCurrentNode());
	}

	protected string CanTransitToSkill()
	{	
		return GetValue(toSkillMap, animationStateMachine.GetCurrentNode());
	}

	protected string GetValue(Dictionary<string, string> map, string key)
	{
		string value;

		if(map.TryGetValue(key, out value))
			return value;

		return null;
	}

	public void OnHurtAreaEntered(Area area)
	{
		if(this.IsLayerInMask(area, strikeDispatchMask))
			area.EmitSignal(this.GetGDMethodAreaEntered(), hurtArea, area);
	}

	protected virtual void InitializeBodyRotationMap()
	{
		bodyRotationMap = new Dictionary<Vector2, float>();

		bodyRotationMap.Add(new Vector2(-1f, 0f), -90f);
		bodyRotationMap.Add(new Vector2(-1f, 1f), -45f);
		bodyRotationMap.Add(new Vector2(-1f, -1f), -135f);

		bodyRotationMap.Add(new Vector2(1f, 0f), 90f);
		bodyRotationMap.Add(new Vector2(1f, 1f), 45f);
		bodyRotationMap.Add(new Vector2(1f, -1f), 135f);

		bodyRotationMap.Add(new Vector2(0f, 1f), 0f);
		bodyRotationMap.Add(new Vector2(0f, -1f), 180f);
	}

	protected virtual void InitializeBodyDirectionMap()
	{
		bodyDirectionMap = new Dictionary<float, Vector3>();

		bodyDirectionMap.Add(0, new Vector3(0f, 0f, 1f));
		bodyDirectionMap.Add(180, new Vector3(0f, 0f, -1f));

		bodyDirectionMap.Add(-90, new Vector3(-1f, 0f, 0f));
		bodyDirectionMap.Add(-45, new Vector3(-1f, 0f, 0f));
		bodyDirectionMap.Add(-135, new Vector3(-1f, 0f, 0f));

		bodyDirectionMap.Add(90, new Vector3(1f, 0f, 0f));
		bodyDirectionMap.Add(45, new Vector3(1f, 0f, 0f));
		bodyDirectionMap.Add(135, new Vector3(1f, 0f, 0f));
	}

	protected virtual void ObtainNodes()
	{
		character = GetNode<KinematicBody>(characterNP);
		body = GetNode<Spatial>(bodyNP);
		hurtArea = GetNode<Area>(hurtAreaNP);
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).Get(
				"parameters/playback") as AnimationNodeStateMachinePlayback;
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		InitializeBodyRotationMap();
		InitializeBodyDirectionMap();
	}

	public bool IgnoreHit
	{	
		set
		{	
			ignoreHit = value;
		}
	}


	[Export]
	public NodePath characterNP;

	[Export]
	public NodePath bodyNP;

	[Export]
	public NodePath hurtAreaNP;

	[Export]
	public NodePath animationTreeNP;

	[Export]
	public Dictionary<string, string> toMoveMap;

	[Export]
	public Dictionary<string, string> toSkillMap;

	[Export(PropertyHint.Layers3dPhysics)]
	public uint strikeDispatchMask = 1024 + 4096 + 8192 + 16384;


	protected KinematicBody character;
	protected Dictionary<Vector2, float> bodyRotationMap;
	protected Dictionary<float, Vector3> bodyDirectionMap;
	protected Spatial body;
	protected Area hurtArea;
	protected AnimationNodeStateMachinePlayback animationStateMachine;

	protected bool ignoreHit;
}
