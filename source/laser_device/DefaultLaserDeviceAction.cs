using Godot;
using Godot.Collections;


public class DefaultLaserDeviceAction : Node
{
	public void Detonate()
	{
		string a = CanTransitToDetonate();

		if(a != null)
		{
			UpdateLaserMaximumSizes();
			animationStateMachine.Travel(a);

			if(hasTimer)
				timer.Stop();
		}
	}

	public void OnHurtAreaEntered(Area area)
	{
		if(centerLaserRayArea.GetInstanceId() != area.GetInstanceId())
		{
			area.EmitSignal(this.GetGDMethodAreaEntered(), hurtArea, area);

			if(!armored)
				Detonate();
		}
	}

	public void Reset()
	{
		laserDevice.Call(this.GetMethodMove(), pusher, Vector3.Zero);
		laserGrowing = false;
		pusher = null;
		LaserRayLevel = 0;
		DetonateTimeLevel = 0;
		ResetGrowLaserRayAreaScales();
	}

	public void TransitTo(string action)
	{
		animationStateMachine.Travel(action);
	}
	
	public void OnIgnoreCollisionBodyEntered(Node body)
	{
		laserDevice.AddCollisionExceptionWith(body);
	}

	public void OnIgnoreCollisionBodyExited(Node body)
	{
		laserDevice.RemoveCollisionExceptionWith(body);
	}

	public void OnLaserRayAreaEntered(Area areaEntered, Area laserRayArea)
	{
		HandleDamageGiven(areaEntered, laserRayArea);
	}

	protected void GrowLaserRay()
	{
		if(laserGrowing)
		{
			float laserGrowFactor = Mathf.Lerp(0f, laserRayLevel, laserGrowAnimationDuration);
			Vector3 growAxis;

			for(int i = 0; i < growLaserRayAreas.Length; i++)
			{
				growAxis = laserGrowAxisMap[i];
				growLaserRayAreas[i].Scale += growAxis * laserGrowFactor;

				if((growLaserRayAreas[i].Scale * growAxis).LengthSquared() >
						laserMaximumSizes[i].LengthSquared())
				{
					growLaserRayAreas[i].Scale =
							(Vector3.One - growAxis) + laserMaximumSizes[i];
				}
			}
		}	
	}

	protected void UpdateLaserMaximumSizesWithInterruption()
	{
		if(laserGrowing && laserInterruptMask != 0)
		{
			Area c;
			Vector3 d;
			float raySize;

			for(int i = 0; i < laserMaximumSizes.Length; i++)
			{
				c = sizeRayCasts[i].GetCollider() as Area;

				if(c != null && this.IsLayerInMask(c, laserInterruptMask))
				{
					d = (laserDevice.GlobalTransform.origin -
							sizeRayCasts[i].GetCollisionPoint()).Abs() * rayCastDirections[i];
					raySize = Mathf.Min(Mathf.Floor(d.x + d.y + d.z) + laserGrowMargin,
							laserRayLevel - laserGrowMargin);
				}
				else
					raySize = laserRayLevel - laserGrowMargin;

				laserMaximumSizes[i] = laserGrowAxisMap[i] * raySize;
			}
		}
	}

	protected void UpdateLaserMaximumSizes()
	{
		if(laserInterruptMask == 0)
		{
			for(int i = 0; i < laserMaximumSizes.Length; i++)
				laserMaximumSizes[i] = laserGrowAxisMap[i] * laserRayLevel;
		}
	}

	protected void HandlePhysicsCollision()
	{
		if(pusher != null && !laserGrowing)
		{
			KinematicCollision kc;

			for(int i = 0; i < laserDevice.GetSlideCount(); i++)
			{
				kc = laserDevice.GetSlideCollision(i);

				if(kc.Collider != null &&
						kc.Collider.GetInstanceId() != pusher.GetInstanceId())
				{
					Vector3 translation = laserDevice.GlobalTransform.origin;
					translation.x = Mathf.Round(translation.x);
					translation.z = Mathf.Round(translation.z);
					laserDevice.Translation = translation;
					EmitSignal(this.GetSignalMove(), Vector3.Zero);
					Detonate();
					pusher = null;
					break;
				}
			}
		}
	}

	protected void ResetGrowLaserRayAreaScales()
	{
		for(int i = 0; i < growLaserRayAreas.Length; i++)
			growLaserRayAreas[i].Scale = growLaserRayAreaDefaultSize;
	}

	protected void HandleDamageGiven(Area areaEntered, Area rayArea)
	{
		if(this.IsLayerInMask(areaEntered, hitCallbackMask))
		{
			areaEntered.EmitSignal(this.GetSignalHit(),
					rayArea, areaEntered, damageGiven);
		}
	}

	protected string CanTransitToDetonate()
	{
		string a;

		if(toDetonateMap.TryGetValue(animationStateMachine.GetCurrentNode(), out a))
			return a;

		return null;
	}

	protected virtual void Initialize()
	{
		laserGrowAxisMap = new Dictionary<int, Vector3>();
		laserMaximumSizes = new Vector3[growLaserRayAreas.Length];
		LaserRayLevel = 0;
		DetonateTimeLevel = 0;

		for(int i = 0; i < growLaserRayAreas.Length; i++)
			laserGrowAxisMap.Add(i, laserGrowVectorList[i]);

		if(laserInterruptMask != 0)
		{
			rayCastDirections = new Vector3[sizeRayCasts.Length];

			for(int i = 0; i < sizeRayCasts.Length; i++)
			{
				sizeRayCasts[i].AddException(hurtArea);
				rayCastDirections[i] = sizeRayCasts[i].CastTo.Normalized().Abs();
			}
		}
	}

	protected virtual void ObtainNodes()
	{
		growLaserRayAreas = this.GetNodes<Area>(this, growLaserRayAreaNPList);
		centerLaserRayArea = GetNode<Area>(centerLaserRayAreaNP);
		laserDevice = GetNode<KinematicBody>(laserDeviceNP);
		hurtArea = GetNode<Area>(hurtAreaNP);
		animationStateMachine = GetNode<AnimationTree>(animationTreeNP).Get(
				"parameters/playback") as AnimationNodeStateMachinePlayback;

		if(laserInterruptMask != 0)
			sizeRayCasts = this.GetNodes<RayCast>(this, sizeRayCastNPList);

		if(hasTimer)
			timer = GetNode<Timer>(timerNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
		ResetGrowLaserRayAreaScales();
	}

  public override void _PhysicsProcess(float delta)
  {
		UpdateLaserMaximumSizesWithInterruption();
		GrowLaserRay();
		HandlePhysicsCollision();
  }

	public void SetLaserGrowing(bool active)
	{
		laserGrowing = active;
	}

	public Spatial Pusher
	{
		set
		{
			pusher = value;
		}
	}

	public int LaserRayLevel
	{
		set
		{
			laserRayLevel = Mathf.Max(value, 
					System.Convert.ToInt32(laserRayLevelRange.x));
			laserRayLevel = Mathf.Min(laserRayLevel,
					System.Convert.ToInt32(laserRayLevelRange.y));
		}
	}

	public int DetonateTimeLevel
	{
		set
		{
			if(hasTimer)
			{
				detonateTimeLevel = Mathf.Max(value,
						System.Convert.ToInt32(detonateTimeLevelRange.x));
				detonateTimeLevel = Mathf.Min(detonateTimeLevel,
						System.Convert.ToInt32(detonateTimeLevelRange.y));
				timer.WaitTime = maximumDetonateTime -
						((detonateTimeLevel - 1f) * 0.25f);
			}
		}
	}


	[Export]
	public NodePath laserDeviceNP;

	[Export]
	public Array<NodePath> growLaserRayAreaNPList;

	[Export]
	public Array<NodePath> sizeRayCastNPList;

	[Export]
	public NodePath centerLaserRayAreaNP;

	[Export]
	public NodePath timerNP;

	[Export]
	public NodePath animationTreeNP;

	[Export]
	public NodePath hurtAreaNP;

	[Export]
	public Dictionary<string, string> toDetonateMap;

	[Export]
	public Array<Vector3> laserGrowVectorList;

	[Export]
	public Vector3 growLaserRayAreaDefaultSize = new Vector3(1f, 1f, 0.001f);

	[Export]
	public Vector2 laserRayLevelRange = new Vector2(1f, 18f);

	[Export]
	public Vector2 detonateTimeLevelRange = new Vector2(1f, 5f);

	[Export]
	public float maximumDetonateTime = 3f;

	[Export]
	public int damageGiven = 100;


	// [Export]
	// public string detonateAnimationName = "detonate";

	[Export]
	public float laserGrowAnimationDuration = 0.5f;

	[Export]
	public float laserGrowMargin = 0.1f;

	[Export]
	public bool hasTimer = true;

	[Export]
	public bool armored = false;

	[Export(PropertyHint.Layers3dPhysics)]
	public uint laserInterruptMask = 896; // 128 & 256 & 512

	[Export(PropertyHint.Layers3dPhysics)]
	public uint hitCallbackMask = 96; // 32 & 64



	protected Spatial pusher;
	protected int laserRayLevel;
	protected int detonateTimeLevel;

	protected KinematicBody laserDevice;
	protected Area[] growLaserRayAreas;
	protected RayCast[] sizeRayCasts;
	protected Area centerLaserRayArea;
	protected Timer timer;
	protected AnimationNodeStateMachinePlayback animationStateMachine;
	protected Area hurtArea;

	protected Dictionary<int, Vector3> laserGrowAxisMap;
	protected Vector3[] laserMaximumSizes;
	protected Vector3[] rayCastDirections;
	protected bool laserGrowing;
}

