using Godot;
using System;
using System.Linq;

public partial class WhiteGhost : CharacterBody2D, IEnemy
{
	StringName[] LOCKABLE_ANIMATIONS = { Animations.Dead, Animations.Spawn };
	
	Timer timer;
	Timer _directionTimer;
	Player _player;
	bool _animationLocked = false;
	AnimatedSprite2D _animation;
	bool _isSpawn = false;
	bool _attacked = false;
	private bool _followPlayer;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
		_directionTimer = GetNode<Timer>("ChangeDirectionTimer");
		_player = GetNode<Player>("../Player");
		_animation = GetNode<AnimatedSprite2D>("Animations");

		Reset();
	}

	void StartTimerRandom()
	{
		var val = new Random().Next(2, 5);
		timer.Start(val);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (timer.TimeLeft < 0.1f)
		{
			if (_attacked && !_animationLocked)
				Reset();
			else if (!_attacked)
			{
				GD.Print($"Ismoving: {_followPlayer}");
				Move();
			}
		}

		MoveAndSlide();
	}
	bool ReachedEnd()
	{
		return this.Position.X >= 2500;
	}

	void Reset()
	{
		_attacked = false;
		_animationLocked = false;
		var newPos = Position;

		var val = new Random().Next(1, 2500);
		newPos.X = val;
		newPos.Y = 150;
		this.Position = newPos;
		StartTimerRandom();
	}

	Vector2 FollowPlayer()
	{
		return (_player.Position - this.Position).Normalized();
	}

	Vector2 RandomDirection()
	{
		var random = new Random();
		var val = random.NextSingle();
		return new Vector2(
			val,
			1 - val
		);
	}

	private void Move()
	{
		if(!_followPlayer && _directionTimer.TimeLeft > 0.01f)
			return;

		var newVelocity = Velocity;
		var direction = new Vector2();
		GD.Print($"follow2: {_followPlayer}");
		if (_followPlayer)
		{
			GD.Print("Following");
			direction = FollowPlayer();
		}
		else if (_directionTimer.TimeLeft <= 0.01f)
		{
			GD.Print("Going random");
			direction = RandomDirection();
			_directionTimer.Start(2);
		}

		newVelocity = direction * Constants.GHOST_SPEED;
		Velocity = newVelocity;
		Flip(direction);

		if(direction.Y < 0)
			UpdateAnimation(Animations.WalkUp);
		else if(direction.Y > 0)
			UpdateAnimation(Animations.WalkRight);
	}

	void Flip(Vector2 direction)
	{
		if (direction.X < 0)
		{
			_animation.FlipH = true;
		}
		else if (direction.X > 0)
		{
			_animation.FlipH = false;
		}
	}

	internal void OnBodyEnteredAttackArea(Node body)
	{
		if(body is Player)
		{
			GD.Print("Player Entered Attack Area");
			Attack();
		}
	}

	internal void OnBodyExitedAttackArea(Node body)
	{
		if(body is Player)
		{
			GD.Print("Player Exited Attack Area");
		}
	}

	internal void OnBodyEnteredSpawnArea(Node body)
	{
		if(body is Player)
		{
			GD.Print("Player Entered Spawn Area");
			this._followPlayer = true;
			GD.Print($"follow: {_followPlayer}");
			UpdateAnimation(Animations.Spawn);
			_animationLocked = true;
		}
	}

	internal void OnBodyExitedSpawnArea(Node body)
	{
		if(body is Player)
		{
			GD.Print("Player Exited Spawn Area");
			_followPlayer = false;
			UpdateAnimation(Animations.Dead);
			_animationLocked = true;
		}
	}

	private void Attack()
	{
		if (!_attacked)
		{
			var direction = _player.Position.X < this.Position.X;
			_player.RolledOver(direction, Constants.GHOST_ATTACK_SPEED);
			UpdateAnimation(Animations.Dead);
			_animationLocked = true;
			_attacked = true;
		}
	}

	internal void Disappear()
	{
		UpdateAnimation(Animations.Dead);
		_animationLocked = true;
	}

	void UpdateAnimation(string animation)
	{
		if (!_animationLocked || animation == Animations.Dead || animation == Animations.Spawn)
			_animation.Play(animation);
	}

	public void OnAnimationFinished()
	{
		if (LOCKABLE_ANIMATIONS.Contains(_animation.Animation))
		{
			_animationLocked = false;
		}
	}

	public void Hurt(int damage)
	{
		Disappear();
		_attacked = true;
	}
}