using Godot;
using System;

public partial class Truck : Area2D
{
	// Called when the node enters the scene tree for the first time.
	Timer timer;
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		var val = new Random(DateTime.Now.Millisecond).Next(10, 30);
		timer.Start(val);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(timer.TimeLeft < 0.1f)
		{
			Move();
		}

	}

	private void Move()
	{
		throw new NotImplementedException();
	}
}
