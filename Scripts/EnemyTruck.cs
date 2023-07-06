using Godot;
using System;
using System.Threading.Tasks;

public partial class EnemyTruck : CharacterBody2D
{
	Timer timer;
	bool _isMoving = false;
	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		Reset();
	}

	void StartTimerRandom()
	{
		var val = new Random().Next(5, 15);
		timer.Start(val);
	}

	public override void _PhysicsProcess(double delta)
	{
		if(timer.TimeLeft < 0.1f && !_isMoving)
		{
			Move();
			_isMoving = true;
		}
		else if(ReachedEnd())
		{
			Reset();
		}

		MoveAndSlide();
	}

	void Reset()
	{
		_isMoving = false;
		StartTimerRandom();

		var newVelocity = Velocity;
		newVelocity.X = 0;
		this.Velocity = newVelocity;
		
		var newPos = Position;
		newPos.X = -100;
		this.Position = newPos;
	}

	bool ReachedEnd()
	{
		return this.Position.X >= 2500;
	}

	private void Move()
	{
		var newVelocity = Velocity;
		newVelocity.X = Constants.TRUCK_SPEED;
		Velocity = newVelocity;
	}

	void OnBodyEntered(Node body)
	{
		if(body is Player)
		{
			var player = body as Player;
			player.RolledOver(false, Constants.ROLLOVER_SPEED);
		}
	}
}