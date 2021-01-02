using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class LaserDeviceAirStrike : Node
{
	public void SetAirStrikeLevel(int airStrikeLevel)
	{
		if(airStrikeLevel == 0)
			laserDeviceDropTimer.Stop();
		else if(airStrikeLevel == 1)
			OnLaserDeviceDropTimeout();
		else
			DropNuclearLaserDevice();
	}

	public void OnLaserDeviceDropTimeout()
	{
		DropLaserDevice();
		laserDeviceDropTimer.Start(this.RandfRange(rng, 0.5f, 1f));
	}	

	public void AddAsAvailable(string type, Spatial laserDevice, Vector3 translation)
	{
		if(laserDevice.GetInstanceId() != nuclearDevice.GetInstanceId())
		{
			laserDevice.Translation = inactiveTranslation;
			availableLaserDeviceMap[type].Add(laserDevice);
		}
	}
	
	private void DropLaserDevice()
	{
		Array<Spatial> deviceList = availableLaserDeviceMap["W"];

		if(deviceList.Count > 0)
		{
			Spatial ld = deviceList[0];
			ld.Translation = new Vector3(this.RandiRange(rng, -halfColumn, halfColumn),
					0f, this.RandiRange(rng, -halfRow, halfRow)) + dropOffset;
			ld.Call(this.GetMethodSetLaserRayLevel(), this.RandiRange(rng, 2, 5));
			ld.Call(this.GetMethodTransitTo(), "active");
			ld.Call(this.GetMethodMove(), this, Vector3.Down);
			deviceList.RemoveAt(0);
		}
	}

	private void DropNuclearLaserDevice()
	{
		if(nuclearDevice != null)
		{
			laserDeviceDropTimer.Stop();
			nuclearDevice.Translation = new Vector3(0f, 20, 0f);
			nuclearDevice.Call(this.GetMethodTransitTo(), "active");
			nuclearDevice.Call(this.GetMethodMove(), this, Vector3.Down);
		}
	}

	private void ObtainNodes()
	{
		laserDeviceDropTimer = GetNode<Timer>(laserDeviceDropTimerNP);
	}

	private void Initialize()
	{
		availableLaserDeviceMap = new Dictionary<string, Array<Spatial>>();
		rng = new RandomNumberGenerator();
		halfRow = (System.Convert.ToInt32(blockSlotAmount.x) - 1) / 2;
		halfColumn = (System.Convert.ToInt32(blockSlotAmount.y) - 1) / 2;
	}

	private void CreateLaserDeviceInstances()
	{
		Array<Spatial> laserDeviceList;
		string currentType;
		Spatial container;	
		Spatial laserDevice = null;
		SCG.IEnumerator<PackedScene> it = laserDevicePSList.GetEnumerator();
		int index = 0;

		while(it.MoveNext())
		{
			laserDeviceList = new Array<Spatial>();
			container = GetChild<Spatial>(index++);

			for(int i = 0; i < laserDeviceAmount; i++)
			{
				laserDevice = it.Current.Instance() as Spatial;
				laserDevice.Translation = inactiveTranslation;
				laserDevice.Name = this.CreateUniqueNodeName(laserDevice);
				laserDevice.Call(this.GetMethodSetManager(), this);
				container.CallDeferred(this.GetGDMethodAddChild(), laserDevice);
				laserDeviceList.Add(laserDevice);
			}

			currentType = this.Call<string>(laserDevice, this.GetMethodGetNodeType());
			availableLaserDeviceMap.Add(currentType, laserDeviceList);
		}

		nuclearDevice = nuclearDevicePS.Instance() as Spatial;
		nuclearDevice.Translation = inactiveTranslation;
		nuclearDevice.Name = this.CreateUniqueNodeName(nuclearDevice);
		nuclearDevice.Call(this.GetMethodSetManager(), this);
		container = GetChild<Spatial>(index++);
		container.CallDeferred(this.GetGDMethodAddChild(), nuclearDevice);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
		CreateLaserDeviceInstances();
	}


	[Export]
	public NodePath laserDeviceDropTimerNP;

	[Export]
	public Array<PackedScene> laserDevicePSList;

	[Export]
	public PackedScene nuclearDevicePS;

	[Export]
	public Vector3 inactiveTranslation = new Vector3(-19f, 0.5f, -14f);

	[Export]
	public Vector2 blockSlotAmount = new Vector2(13f, 21f);

	[Export]
	public Vector3 dropOffset = new Vector3(0f, 20f, 0f);

	[Export]
	public int laserDeviceAmount = 6;


	private Timer laserDeviceDropTimer;

	private RandomNumberGenerator rng;
	private Dictionary<string, Array<Spatial>> availableLaserDeviceMap;
	private Spatial nuclearDevice;
	private int halfRow;
	private int halfColumn;
}
