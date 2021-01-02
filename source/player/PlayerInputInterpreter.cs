using System.Text;
using SCG = System.Collections.Generic;

using Godot;
using Godot.Collections;


public class PlayerInputInterpreter : Node
{
	private bool InterpretExecuteSkill()
	{
		return IsActionInputPressed(EXECUTE_SKILL_MASK) &&
		 		this.EmitSignal<bool>(this, this.GetSignalExecuteSkill());
	}

	private bool InterpretPlantDevice()
	{
		return Input.IsActionJustPressed(inputPlantDevice) &&
		 		this.EmitSignal<bool>(this, this.GetSignalPlantDevice());
	}

	private bool InterpretChangeSlot()
	{
		return Input.IsActionJustPressed(inputChangeSlot) &&
		 		this.EmitSignal<bool>(this, this.GetSignalChangeSelectedSlot());
	}

	private bool InterpretMove()
	{
		return this.EmitSignal<bool>(this, this.GetSignalMove(), direction);
	}

	private void ComputeDirectionAxisInput(ref float axis, string negative, string positive)
	{
		bool axisNegative = Input.IsActionPressed(negative);
		bool axisPositve = Input.IsActionPressed(positive);

		if(axisPositve && !axisNegative)
			axis = 1;
		else if(axisNegative && !axisPositve)
			axis = -1;
		else
			axis = 0;
	}

	private void ComputerActionInputs()
	{
		byte actionInputMask = (byte) (Input.IsActionPressed(inputPlantDevice) ?
				PLANT_DEVICE_MASK : 0);
		actionInputMask |= (byte) (Input.IsActionPressed(inputChangeSlot) ?
				CHANGE_SLOT_MASK : 0);
		actionInputMask |= (byte) (Input.IsActionPressed(inputExecuteSkill) ?
				EXECUTE_SKILL_MASK : 0);
		currentActionInputMask = actionInputMask;
		
		while(actionInputBufferList.Count >= buttonInputBufferLength)
			actionInputBufferList.RemoveAt(0);
		
		actionInputBufferList.Add(actionInputMask);
		ComputeActionInputUnionMask();
	}

	private void ComputeActionInputUnionMask()
	{
		SCG.IEnumerator<byte> it = actionInputBufferList.GetEnumerator();
		actionInputUnionMask = PLANT_DEVICE_MASK | CHANGE_SLOT_MASK | EXECUTE_SKILL_MASK;

		while(it.MoveNext())
			actionInputUnionMask &= it.Current;
	}

	private bool IsActionInputPressed(byte actionInput)
	{
		return ((actionInput & actionInputUnionMask) == 0) &&
				((actionInput & currentActionInputMask) == actionInput);
	}

	private void HandleDirectionInput()
	{
		direction.x = 0;
		direction.z = 0;
		ComputeDirectionAxisInput(ref direction.x, inputLeft, inputRight);
		ComputeDirectionAxisInput(ref direction.z, inputUp, inputDown);
	}

	private string GetFixedInputName(string inputName)
	{
		return new StringBuilder().Append('p').Append(playerId).
				Append('_').Append(inputName).ToString();
	}

	private void InterpretInputs()
	{
		HandleDirectionInput();
		ComputerActionInputs();
		InterpretChangeSlot();
		InterpretPlantDevice();
		bool ignore = InterpretExecuteSkill();

		if(!ignore)
			ignore = InterpretMove();
	}

	private void UpdateInputNames()
	{
		inputUp = GetFixedInputName(inputUp);
		inputDown = GetFixedInputName(inputDown);
		inputLeft = GetFixedInputName(inputLeft);
		inputRight = GetFixedInputName(inputRight);

		inputPlantDevice = GetFixedInputName(inputPlantDevice);
		inputChangeSlot = GetFixedInputName(inputChangeSlot);
		inputExecuteSkill = GetFixedInputName(inputExecuteSkill);
	}

	private void Initialize()
	{
		actionInputBufferList = new Array<byte>();
		actionInputBufferList.Add(0);

		Input.SetMouseMode(Input.MouseMode.Captured);
	}

  public override void _EnterTree()
  {
	Initialize();
  }
  
	public override void _PhysicsProcess(float delta)
	{
		InterpretInputs();
	}

	public int PlayerId
	{
		set
		{
			playerId = value < 1 ? 1 : value;
			UpdateInputNames();
		}
	}


	[Export]
	private byte buttonInputBufferLength = 5;


	private int playerId;

	private Vector3 direction;
	private Array<byte> actionInputBufferList;
	private byte currentActionInputMask;
	private byte actionInputUnionMask;

	private string inputUp = "up";
	private string inputDown = "down";
	private string inputLeft = "left";
	private string inputRight = "right";

	private string inputPlantDevice = "plant_device";
	private string inputChangeSlot = "change_slot";
	private string inputExecuteSkill = "execute_skill";

	private const byte PLANT_DEVICE_MASK = 1;
	private const byte CHANGE_SLOT_MASK = 2;
	private const byte EXECUTE_SKILL_MASK = 4;
}
