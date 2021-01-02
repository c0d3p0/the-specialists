using Godot;


// OS.WindowMaximized property is always returning false and I have no idea why.
// Maybe a GodotEngine 3.2.3 bug?
public class WindowManager : Node
{
	private void HandleToggleMaximize()
	{
		OS.WindowMaximized = !OS.WindowMaximized;
	}

	private void HandleToggleFullScreen()
	{
		OS.WindowFullscreen = !OS.WindowFullscreen;
	}

	private void HandleToggleWindowBorderless()
	{
		OS.WindowBorderless = !OS.WindowBorderless;
	}

  public override void _EnterTree()
  {
		OS.CenterWindow();
		OS.SetWindowTitle(gameTitle);
  }
  
	public override void _Input(InputEvent inputEvent)
	{
		if(inputEvent.IsActionPressed("toggle_maximize"))
			HandleToggleMaximize();
		else if(inputEvent.IsActionPressed("toggle_fullscreen"))
			HandleToggleFullScreen();
		else if(inputEvent.IsActionPressed("toggle_borderless"))
			HandleToggleWindowBorderless();
	}


	[Export]
	public string gameTitle = "The Specialists (github.com/c0d3p0 - Godot Engine)";
}
