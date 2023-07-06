using Godot;
using System;

public partial class AnotherStreetMap : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void _on_go_to_other_street_map_body_entered(Node body)
	{
		if(body is Player)
		{
			GD.Print("Should go to other map");
			GetTree().ChangeSceneToFile("res://Levels/first_level.tscn");
		}
	}
}
