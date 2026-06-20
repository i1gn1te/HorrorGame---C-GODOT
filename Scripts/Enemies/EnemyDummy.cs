using Godot;

public partial class EnemyDummy : CharacterBody3D, IDamageable, IStunnable, IJumpscareActivatable
{
    // Prosty przeciwnik do testowania walki.
    // Na razie to czerwony klocek, który podchodzi do gracza i zadaje obrażenia z bliska.

    [Export] public string EnemyName = "Cichy Manekin";
    [Export] public bool StartHidden = false;
    [Export] public bool StartInactive = false;
    [Export] public float MaxHealth = 80f;
    [Export] public float MoveSpeed = 2.3f;
    [Export] public float Gravity = 24f;
    [Export] public float AggroRange = 9f;
    [Export] public float AttackRange = 1.5f;
    [Export] public float AttackDamage = 12f;
    [Export] public float AttackCooldown = 1.3f;

    private float _health;
    private float _attackCooldownTimer;
    private float _stunTimer;
    private bool _isActive = true;
    private Node3D _target;
    private MeshInstance3D _bodyMesh;

    public override void _Ready()
    {
        AddToGroup("enemies");
        _health = MaxHealth;
        _bodyMesh = GetNodeOrNull<MeshInstance3D>("Body");
        _isActive = !StartInactive;
        Visible = !StartHidden;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isActive)
        {
            Velocity = Vector3.Zero;
            return;
        }

        if (_stunTimer > 0f)
        {
            _stunTimer -= (float)delta;
            Velocity = new Vector3(0f, Velocity.Y, 0f);
            MoveAndSlide();
            return;
        }

        FindTargetIfNeeded();

        if (_attackCooldownTimer > 0f)
        {
            _attackCooldownTimer -= (float)delta;
        }

        Vector3 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity.Y -= Gravity * (float)delta;
        }
        else if (velocity.Y < 0f)
        {
            velocity.Y = -0.1f;
        }

        Vector3 horizontalVelocity = Vector3.Zero;

        if (_target != null && IsInstanceValid(_target))
        {
            float distance = GlobalPosition.DistanceTo(_target.GlobalPosition);

            if (distance <= AggroRange && distance > AttackRange)
            {
                Vector3 direction = _target.GlobalPosition - GlobalPosition;
                direction.Y = 0f;
                horizontalVelocity = direction.Normalized() * MoveSpeed;
                LookAt(GlobalPosition + direction.Normalized(), Vector3.Up);
            }
            else if (distance <= AttackRange)
            {
                TryAttack();
            }
        }

        velocity.X = horizontalVelocity.X;
        velocity.Z = horizontalVelocity.Z;
        Velocity = velocity;
        MoveAndSlide();
    }

    public void TakeDamage(float amount, Node3D source)
    {
        // Tę metodę woła hitbox gracza.
        _health -= amount;
        FlashAfterHit();

        if (_health <= 0f)
        {
            GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
            gameManager?.ShowMessage($"{EnemyName} rozpada sie w ciszy.", 2.5f);
            QueueFree();
        }
    }

    public void ActivateJumpscare()
    {
        // Tę metodę odpala trigger jumpscare.
        // Przeciwnik pojawia się dopiero wtedy, gdy chcemy wywołać konkretny moment strachu.
        Visible = true;
        _isActive = true;
    }

    public void Stun(float seconds)
    {
        // FlashBurst nie zabija od razu. Daje graczowi chwilę na ucieczkę albo przejście dalej.
        _stunTimer = Mathf.Max(_stunTimer, seconds);
    }

    private void FindTargetIfNeeded()
    {
        if (_target != null && IsInstanceValid(_target))
        {
            return;
        }

        _target = GetTree().GetFirstNodeInGroup("player") as Node3D;
    }

    private void TryAttack()
    {
        if (_attackCooldownTimer > 0f || _target == null)
        {
            return;
        }

        PlayerStats stats = _target.GetNodeOrNull<PlayerStats>("PlayerStats");

        if (stats != null)
        {
            stats.TakeDamage(AttackDamage);
        }

        _attackCooldownTimer = AttackCooldown;
    }

    private void FlashAfterHit()
    {
        // Na razie sygnalizujemy trafienie skalą modelu. Później zamienisz to na animację.
        if (_bodyMesh == null)
        {
            return;
        }

        _bodyMesh.Scale = new Vector3(1.15f, 1.15f, 1.15f);
        GetTree().CreateTimer(0.08).Timeout += () => _bodyMesh.Scale = Vector3.One;
    }
}
