using Godot;
using System;


public enum EnemyStates
{
	Idle,
	Move,
	Attack
}

public partial class Enemy2 : CharacterBody2D
{
	int _health = 100;
	private AnimatedSprite2D _animation;
	EnemyStates _currentState = EnemyStates.Idle;
	bool _animationLocked;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("Animations");
	}

	public override void _PhysicsProcess(double delta)
	{

		var currentVelocity = Velocity;

		switch(_currentState)
		{
			case EnemyStates.Idle:
				Idle(delta, currentVelocity);
				break;
			case EnemyStates.Move:
				Move(delta, currentVelocity);
				break;
			case EnemyStates.Attack:
				//Attack(delta, currentVelocity);
				break;
		}
		
		MoveAndSlide();
		return;
	}

	void Idle(double delta, Vector2 currentVelocity)
	{
		var direction = Vector2.Zero;
		if (ShouldMove())
		{
			_currentState = EnemyStates.Move;
			Move(delta, direction, currentVelocity);
		}
		else
		{
			UpdateAnimation(Animations.WalkRight);
		}
	}

	private bool ShouldMove()
	{
		return false;
	}

	void Move(double delta, Vector2 currentVelocity)
	{
		var direction = Vector2.Zero;
		Move(delta, direction, currentVelocity);
	}

	void Move(double delta, Vector2 direction, Vector2 currentVelocity)
	{ 	
		//if (direction != Vector2.Zero)
		//{
	//		currentVelocity.X = direction.X * speed;
	//		if(!_isJumping)
	//			currentVelocity.Y = direction.Y * Constants.SPEED;

//			UpdateAnimation(Animations.WalkRight);
//			_animation.FlipH = direction.X < 0;
//		}
//		else
//		{
//			currentVelocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
//			if(!_isJumping)
//				currentVelocity.Y = Mathf.MoveToward(Velocity.Y, 0, Constants.SPEED);
//		}
//
//		if(Input.IsActionJustPressed(Moves.Attack))
//		{
//			Attack(delta, currentVelocity);
//		}

		if(currentVelocity == Vector2.Zero)
		{
			_currentState = EnemyStates.Idle;
		}
		
		base.Velocity = currentVelocity;
	}


	internal void Hurt(int damage)
	{
		_health -= damage;

		if(_health <= 0)
			Dead();

		UpdateAnimation(Animations.Hurt);
		_animationLocked = true;
		GD.Print("Hurt");
	}

	private void Dead()
	{
		UpdateAnimation(Animations.Dead);
		_animationLocked = true;
	}

	void UpdateAnimation(string animation)
	{
		if (!_animationLocked)
		{
			_animation.Play(animation);
		}
	}

	void OnAnimationFinished()
	{
		if (_animation.Animation == Animations.Hurt)
		{
			_animationLocked = false;
		}
	}
}
