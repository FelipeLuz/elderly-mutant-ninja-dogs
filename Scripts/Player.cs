using Godot;
using System;
using System.Linq;

public enum PlayerStates
{
	Idle,
	Move,
	Attack
}

public partial class Player : CharacterBody2D
{
   // Get the gravity from the project settings to be synced with RigidBody nodes.
   	StringName[] LOCKABLE_ANIMATIONS = { Animations.Dead, Animations.PunchDirect, Animations.JumpEnd };
	float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	PlayerStates _currentState = PlayerStates.Idle;
	AnimatedSprite2D _animation;
	CollisionShape2D _collision;
	private Label _heartsHUD;
	private Label _CoinsHUD;
	CanvasLayer _canvas;
	bool _isJumping = false;
	bool _isDoubleJumping = false;
	float JumpInitialPosY = 0f;
	bool _animationLocked = false;
	bool _isAttacking = false;
	bool _isLayedOnGround = false;
	bool _disableCollisionNextIteration = false;
	bool _doubleJumpNextIteration = false;
	bool _landingFromStepJump = false;
	IEnemy _enemyOnTarget = null;
	AudioStreamPlayer _jumpSound;
	AudioStreamPlayer _coinSound;
	AudioStreamPlayer _heartSound;
	AudioStreamPlayer _hurtSound;
	private Timer _invincibilityTimer;

	public void OnReady()
	{
		_animation = GetNode<AnimatedSprite2D>("Animations");
		_collision = GetNode<CollisionShape2D>("Collision");
		_heartsHUD = GetNode<Label>("../Canvas/ResourceDisplay/Life/Value");
		_CoinsHUD = GetNode<Label>("../Canvas/ResourceDisplay/Coins/Value");
		_coinSound = GetNode<AudioStreamPlayer>("Sounds/Coins");
		_jumpSound = GetNode<AudioStreamPlayer>("Sounds/Jump");
		_heartSound = GetNode<AudioStreamPlayer>("Sounds/Coins");
		_hurtSound = GetNode<AudioStreamPlayer>("Sounds/Hurt");
		_invincibilityTimer = GetNode<Timer>("InvincibilityTimer");
		_invincibilityTimer.Start(3);
		UpdateHUD();
	}

	Vector2 DoubleJump(Vector2 velocity)
	{
		velocity.Y = Constants.JUMP_VELOCITY * 2.0f;
		_isJumping = true;
		_isDoubleJumping = true;
		_doubleJumpNextIteration = false;
		_collision.Disabled = true;
		_animationLocked = true;
		_landingFromStepJump = true;
		UpdateAnimation(Animations.JumpStart);
		return velocity;
	}

	Vector2 Jump(Vector2 velocity)
	{
		if(_isDoubleJumping)
			return velocity;

		if (_isJumping)
		{
			velocity.Y += Constants.JUMP_VELOCITY;
			_isDoubleJumping = true;
		}
		else
		{
			JumpInitialPosY = this.Position.Y;
			velocity.Y = Constants.JUMP_VELOCITY;
			_animationLocked = true;
			_isJumping = true;
			_collision.Disabled = true;
		}

		UpdateAnimation(Animations.JumpStart);
		_jumpSound.Play();
		return velocity;
	}

	Vector2 Land(Vector2 velocity, double delta)
	{
		velocity.Y += gravity * (float)delta;

		if (_landingFromStepJump &&
			this.Position.Y <= JumpInitialPosY)
		{
			velocity.Y = 0;
			_isJumping = false;
			_isDoubleJumping = false;
			_landingFromStepJump = false;
			GD.Print($"Collision Enabled: {_collision.Disabled}");
			_collision.Disabled = false;
			UpdateAnimation(Animations.JumpEnd);
		}

		if (this.Position.Y >= JumpInitialPosY)
		{
			velocity.Y = 0;
			_isJumping = false;
			_isDoubleJumping = false;
			GD.Print($"Collision Enabled: {_collision.Disabled}");
			_collision.Disabled = false;
			UpdateAnimation(Animations.JumpEnd);
		}

		return velocity;
	}

	public override void _PhysicsProcess(double delta)
	{
		var currentVelocity = Velocity;

		switch(_currentState)
		{
			case PlayerStates.Idle:
				Idle(delta, currentVelocity);
				break;
			case PlayerStates.Move:
				Move(delta, currentVelocity);
				break;
			case PlayerStates.Attack:
				Attack(delta, currentVelocity);
				break;
		}
		
		MoveAndSlide();
		return;
	}

	private void Attack(double delta, Vector2 currentVelocity)
	{
		UpdateAnimation(Animations.PunchDirect);

		if (_enemyOnTarget != null)
		{
			_enemyOnTarget.Hurt(25);
			GD.Print("Enemy hurt");
		}

		_animationLocked = true;
		_isAttacking = true;
	}

	void Idle(double delta, Vector2 currentVelocity)
	{
		var direction = Input.GetVector(Moves.Left, Moves.Right, Moves.Up, Moves.Down);
		if (direction != Vector2.Zero ||
			Input.IsActionJustPressed(Moves.Jump))
		{
			_currentState = PlayerStates.Move;
			Move(delta, direction, currentVelocity);
		}
		else if(Input.IsActionJustPressed(Moves.Attack))
		{
			Attack(delta, currentVelocity);
		}
		else
		{
			UpdateAnimation(Animations.Idle);
		}
	}

	void Move(double delta, Vector2 currentVelocity)
	{
		var direction = Input.GetVector(Moves.Left, Moves.Right, Moves.Up, Moves.Down);
		Move(delta, direction, currentVelocity);
	}

	void Move(double delta, Vector2 direction, Vector2 currentVelocity)
	{
		if (_disableCollisionNextIteration)
		{
			_collision.Disabled = true;
			_disableCollisionNextIteration = false;
		}

		if(_isLayedOnGround)
			return;

		var speed = _isJumping || _isAttacking ? Constants.SPEED/2 : Constants.SPEED;

		if (_isJumping)
		{
			currentVelocity = Land(currentVelocity, delta);
		}

		if(Input.IsActionJustPressed(Moves.Jump) )
			currentVelocity = Jump(currentVelocity);

		if(_doubleJumpNextIteration)
		{
			currentVelocity = DoubleJump(currentVelocity);
		}

		if (direction != Vector2.Zero)
		{
			currentVelocity.X = direction.X * speed;
			if(!_isJumping)
				currentVelocity.Y = direction.Y * Constants.SPEED/2;

			UpdateAnimation(Animations.WalkRight);
			Flip(direction);
		}
		else
		{
			currentVelocity.X = Mathf.MoveToward(Velocity.X, 0, speed);
			if(!_isJumping)
				currentVelocity.Y = Mathf.MoveToward(Velocity.Y, 0, Constants.SPEED);
		}

		if(Input.IsActionJustPressed(Moves.Attack))
		{
			Attack(delta, currentVelocity);
		}

		if(currentVelocity == Vector2.Zero)
		{
			_currentState = PlayerStates.Idle;
		}
		
		base.Velocity = currentVelocity;
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

	void UpdateAnimation(string animation)
	{
		if (!_animationLocked ||
			animation == Animations.JumpEnd)
		{
			_animation.Play(animation);
		}
	}

	public void OnAnimationFinished()
	{
		if (LOCKABLE_ANIMATIONS.Contains(_animation.Animation))
		{
			_animationLocked = false;
			_isAttacking = false;

			if (_animation.Animation == Animations.Dead)
			{
				_isLayedOnGround = false;
				var newVel = this.Velocity;
				newVel.X = 0;
				this.Velocity = newVel;
				CheckDeath();
			}
		}
	}

	internal void CollectCoin()
	{
		Inventory.Coins += 1;
		UpdateHUD();
		GD.Print("Coins: " + Inventory.Coins );
		_coinSound.Play();
	}

	internal void UpdateHUD()
	{
		_CoinsHUD.Text = Inventory.Coins.ToString();
		_heartsHUD.Text = Inventory.Hearts.ToString();
	}

	internal bool CollectHeart()
	{
		if (Inventory.Hearts < 3)
		{
			Inventory.Hearts += 1;
			UpdateHUD();
			GD.Print("Hearts: " + Inventory.Hearts);
			_coinSound.Play();
			return true;
		}
		else
			return false;

	}

	internal void OnBodyEnteredAttackArea(Node body)
	{
		GD.Print("Body entered");
		if(body is IEnemy)
		{
			GD.Print("Enemy Entered");
			_enemyOnTarget = (IEnemy)body;
		}
	}

	internal void OnBodyExitedAttackArea(Node body)
	{
		GD.Print("Body exited");
		if(body is IEnemy)
		{
			GD.Print("Enemy Exited");
			_enemyOnTarget = null;
		}
	}

	void CheckDeath()
	{
		if(Inventory.Hearts <= 0)
			GetTree().ChangeSceneToFile("res://UI/menu.tscn");

	}

	internal void RolledOver(bool direction, float velocity)
	{
		Inventory.Hearts -= 1;
		UpdateAnimation(Animations.Dead);
		_hurtSound.Play();
		UpdateHUD();
		_animationLocked = true;
		_isLayedOnGround = true;

		var newVel = this.Velocity;
		newVel.X = velocity * (direction ? -1 : 1);
		this._animation.FlipH = direction;
		this.Velocity = newVel;
		_invincibilityTimer.Start(3);
	}

	internal void StepDown(int stepSize)
	{
		_disableCollisionNextIteration = true;
		_isJumping = true;
		_isDoubleJumping = true;
		JumpInitialPosY = this.Position.Y + stepSize;
		_currentState = PlayerStates.Move;
	}

	internal void StepUp(int stepSize)
	{
		GD.Print("Entered StepUp");
		_doubleJumpNextIteration = true;
		JumpInitialPosY = this.Position.Y - stepSize;
		_currentState = PlayerStates.Move;
	}
}