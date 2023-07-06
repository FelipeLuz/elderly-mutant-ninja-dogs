using Godot;
using System;

public partial class Enemy1 : Area2D
{
	private AnimatedSprite2D _animation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("Animations");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	internal void Hurt()
	{
		UpdateAnimation(Animations.Hurt);
		GD.Print("Hurt");
	}

	void UpdateAnimation(string animation)
	{
		_animation.Play(animation);
	}
}
