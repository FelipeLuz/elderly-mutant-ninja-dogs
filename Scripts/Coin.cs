using Godot;
using System;

public partial class Coin : Area2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnBodyEntered(Node body)
	{
		if(body is Player)
		{
			var player = body as Player;
			player.CollectCoin();
			this.QueueFree();
		}
	}
}
