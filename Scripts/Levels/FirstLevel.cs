using Godot;
using System;

public partial class FirstLevel : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	void GoToTrainMap(Node body)
	{
		if(body is Player)
		{
			GetTree().ChangeSceneToFile("res://Levels/TrainMap.tscn");
		}
	}

	void GoToOtherStreetLevel(Node body)
	{
		if(body is Player)
		{
			GetTree().ChangeSceneToFile("res://Levels/AnotherStreetMap.tscn");
		}
	}
}
