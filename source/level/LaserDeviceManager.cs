using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class LaserDeviceManager : Spatial
{
	public void Plant(Node requester, Dictionary data, Godot.Object optional)
	{
		Vector3 translation = (Vector3) data[3];
		translation.y = inactiveTranslation.y;
		plantDataList.Clear();

		if(this.Call<bool>(levelManager,
				this.GetMethodContainsEmptyBlockSlot(), translation))
		{
			Array<Spatial> laserDeviceList = availableLaserDeviceMap[data[0] as string];

			if(laserDeviceList.Count > 0)
			{
				Spatial laserDevice = laserDeviceList[0];
				laserDevice.Translation = translation;
				laserDevice.Call(this.GetMethodSetCharacter(), requester);
				laserDevice.Call(this.GetMethodSetLaserRayLevel(), data[1]);
				laserDevice.Call(this.GetMethodSetDetonateTimeLevel(), data[2]);
				laserDevice.Call(this.GetMethodPlant());

				levelManager.Call(this.GetMethodRemoveEmptyBlockSlot(), laserDevice.Translation);
				laserDeviceList.Remove(laserDevice);
				plantDataList.Add(laserDevice);
			}
		}

		optional.Call(this.GetMethodSet(), plantDataList);
	}

	public void AddAsAvailable(string type, Spatial laserDevice, Vector3 translation)
	{
		availableLaserDeviceMap[type].Add(laserDevice);
		laserDevice.Translation = inactiveTranslation;
		levelManager.Call(this.GetMethodAddEmptyBlockSlot(), translation);
	}

	public void AddEmptyBlockSlot(Vector3 translation)
	{
		levelManager.Call(this.GetMethodAddEmptyBlockSlot(), translation);
	}
	
	private void ObtainNodes()
	{
		globalData = GetNode(globalDataNodePath);
		levelManager = GetNode(levelManagerNP);
	}

	private void Initialize()
	{
		availableLaserDeviceMap = new Dictionary<string, Array<Spatial>>();
		specialistAmount = GetGlobal<int>("specialistAmount");
		plantDataList = new Array();
	}

	private void CreateLaserDeviceInstances()
	{
		Array<Spatial> laserDeviceList;
		string currentType;
		Spatial container;	
		Spatial laserDevice = null;
		SCG.IEnumerator<PackedScene> it = laserDevicePSList.GetEnumerator();
		int index = 0;
		int finalAmount = laserDeviceAmount * specialistAmount;

		while(it.MoveNext())
		{
			laserDeviceList = new Array<Spatial>();
			container = GetChild<Spatial>(index++);

			for(int i = 0; i < finalAmount; i++)
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
	}

	private T GetGlobal<T>(string key)
	{
		return this.Call<T>(globalData, this.GetMethodGet(), key);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
		Initialize();
		CreateLaserDeviceInstances();
	}



	[Export]
	public string globalDataNodePath = "/root/GlobalData";

	[Export]
	public NodePath levelManagerNP;

	[Export]
	public Array<PackedScene> laserDevicePSList;

	[Export]
	public int laserDeviceAmount = 10;

	[Export]
	public Vector3 inactiveTranslation = new Vector3(-19f, 0.5f, -14f);


	private Node globalData;
	private Node levelManager;

	private Dictionary<string, Array<Spatial>> availableLaserDeviceMap;
	private int specialistAmount;
	private Array plantDataList;
}
