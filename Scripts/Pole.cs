using Godot;

public partial class Pole : CharacterBody2D, IEnemy
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	private AnimatedSprite2D _animation;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animation = GetNode<AnimatedSprite2D>("Animations");
	}

	public override void _PhysicsProcess(double delta)
	{
	}

	void UpdateAnimation(string animation)
	{
		_animation.Play(animation);
	}

	void IEnemy.Hurt(int damage)
	{
		UpdateAnimation(Animations.Dead);
	}
}
