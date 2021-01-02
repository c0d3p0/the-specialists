using Godot;


public class MineLaserDeviceAction : DefaultLaserDeviceAction
{
	public void OnTimerTimeout()
	{
		sensorArea.Monitoring = true;
	}

	public void OnSensorAreaEntered(Area area)
	{
		if(area != hurtArea)
			Detonate();
	}

	protected override void ObtainNodes()
	{
		base.ObtainNodes();
		sensorArea = GetNode<Area>(sensorAreaNP);
	}
  

	[Export]
	public NodePath sensorAreaNP;
	

	private Area sensorArea;
}
