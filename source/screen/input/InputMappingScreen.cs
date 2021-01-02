using Godot;


public class InputMappingScreen : Control
{
	public void ShowScreen(Control hideControl)
	{
		inputMappingGUI.ShowScreen(hideControl);
	}

	public void Load(string filePath, bool notNull, Godot.Object optional)
	{
		optional.Call(this.GetMethodSet(),
				jsonSerializer.Load(filePath, notNull));
	}

	private void ObtainNodes()
	{
		inputMappingGUI = GetNode<InputMappingGUI>(inputMappingGUINP);
		jsonSerializer = GetNode<JsonSerializer>(jsonSerializerNP);
	}

	public override void _EnterTree()
	{
		ObtainNodes();
	}


	
	[Export]
	public NodePath inputMappingGUINP;

	[Export]
	public NodePath jsonSerializerNP;


	private InputMappingGUI inputMappingGUI;
	private JsonSerializer jsonSerializer;

}
