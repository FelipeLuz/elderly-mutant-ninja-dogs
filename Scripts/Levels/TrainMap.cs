using Godot;
using System;

public partial class TrainMap : Node2D
{
	Player _player;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_player = GetNode<Player>("Player");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void OnBodyEnteredStepUp(Node body)
	{
		if (body is Player)
		{
			_player.StepUp(120);
		}
	}

	void OnBodyEnteredFirstStep(Node body)
	{
		if(body is Player)
		{
			GD.Print("Player Entered first step Area");
			_player.StepDown(40);
		}
	}
}
