using Godot;
using System;

public partial class Menu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	void OnStartPressed()
	{
		GetTree().ChangeSceneToFile("res://Levels/first_level.tscn");
	}

	void OnAboutPressed()
	{
		//do something
	}
	void OnExitPressed()
	{
		GetTree().Quit();
	}
}
