using Godot;

public abstract partial class HorrorEnemyBase : CharacterBody3D, IDamageable, IStunnable, IJumpscareActivatable
{
    // Wspolna baza dla nowych przeciwnikow.
    // Trzyma zdrowie, grawitacje, stun, aktywacje jumpscare i proste zadawanie obrazen.
    // Konkretne AI jest w klasach, ktore dziedzicza po tej bazie.

    [ExportGroup("Enemy Base")]
    [Export] public string EnemyName = "Nieznany byt";
    [Export] public bool StartHidden = false;
    [Export] public bool StartInactive = false;
    [Export] public float MaxHealth = 80f;
    [Export] public float Gravity = 24f;
    [Export] public float AggroRange = 10f;
    [Export] public float AttackRange = 1.45f;
    [Export] public float AttackDamage = 12f;
    [Export] public float AttackCooldown = 1.2f;
    [Export] public string WakeMessage = "";

    protected Node3D Target;
    protected MeshInstance3D BodyMesh;
    protected float StunTimer;
    protected float AttackCooldownTimer;
    protected bool IsActive = true;

    private float _health;

    public override void _Ready()
    {
        AddToGroup("enemies");
        _health = MaxHealth;
        BodyMesh = GetNodeOrNull<MeshInstance3D>("Body");
        IsActive = !StartInactive;
        Visible = !StartHidden;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!IsActive)
        {
            Velocity = Vector3.Zero;
            return;
        }

        FindTargetIfNeeded();
        UpdateTimers(delta);

        Vector3 velocity = Velocity;
        ApplyGravity(delta, ref velocity);

        if (StunTimer > 0f)
        {
            // FlashBurst zatrzymuje poziomy ruch, ale nadal pozwala grawitacji dzialac.
            velocity.X = 0f;
            velocity.Z = 0f;
            Velocity = velocity;
            MoveAndSlide();
            return;
        }

        RunEnemyAi(delta, ref velocity);

        Velocity = velocity;
        MoveAndSlide();
    }

    public virtual void TakeDamage(float amount, Node3D source)
    {
        _health -= amount;
        PulseBodyMesh();

        if (_health <= 0f)
        {
            Die();
        }
    }

    public virtual void Stun(float seconds)
    {
        // Kazdy nowy enemy reaguje na FlashBurst przez ten sam interfejs.
        StunTimer = Mathf.Max(StunTimer, seconds);
    }

    public virtual void ActivateJumpscare()
    {
        // Trigger jumpscare moze obudzic enemy, ktory zaczal ukryty.
        Visible = true;
        IsActive = true;

        if (!string.IsNullOrWhiteSpace(WakeMessage))
        {
            ShowMessage(WakeMessage, 2.5f);
        }
    }

    protected abstract void RunEnemyAi(double delta, ref Vector3 velocity);

    protected bool HasTarget()
    {
        return Target != null && IsInstanceValid(Target);
    }

    protected float DistanceToTarget()
    {
        if (!HasTarget())
        {
            return float.MaxValue;
        }

        return GlobalPosition.DistanceTo(Target.GlobalPosition);
    }

    protected Vector3 DirectionToTargetFlat()
    {
        if (!HasTarget())
        {
            return Vector3.Zero;
        }

        Vector3 direction = Target.GlobalPosition - GlobalPosition;
        direction.Y = 0f;
        return direction.LengthSquared() > 0.001f ? direction.Normalized() : Vector3.Zero;
    }

    protected void MoveFlat(ref Vector3 velocity, Vector3 direction, float speed)
    {
        velocity.X = direction.X * speed;
        velocity.Z = direction.Z * speed;
    }

    protected void StopFlat(ref Vector3 velocity)
    {
        velocity.X = 0f;
        velocity.Z = 0f;
    }

    protected void FaceTarget()
    {
        Vector3 direction = DirectionToTargetFlat();

        if (direction.LengthSquared() <= 0.001f)
        {
            return;
        }

        LookAt(GlobalPosition + direction, Vector3.Up);
    }

    protected bool IsSeenByTarget(float dotThreshold)
    {
        Camera3D camera = GetTargetCamera();

        if (camera == null)
        {
            return false;
        }

        Vector3 toEnemy = GlobalPosition - camera.GlobalPosition;

        if (toEnemy.LengthSquared() <= 0.001f)
        {
            return true;
        }

        Vector3 cameraForward = -camera.GlobalTransform.Basis.Z.Normalized();
        float dot = cameraForward.Dot(toEnemy.Normalized());
        return dot >= dotThreshold;
    }

    protected void TryAttackTarget(float fearOnHit = 0f)
    {
        if (!HasTarget() || AttackCooldownTimer > 0f || DistanceToTarget() > AttackRange)
        {
            return;
        }

        PlayerStats stats = Target.GetNodeOrNull<PlayerStats>("PlayerStats");
        stats?.TakeDamage(AttackDamage);
        AddFearToTarget(fearOnHit);
        AttackCooldownTimer = AttackCooldown;
    }

    protected void AddFearToTarget(float amount)
    {
        if (!HasTarget() || amount <= 0f)
        {
            return;
        }

        PlayerFear fear = Target.GetNodeOrNull<PlayerFear>("PlayerFear");
        fear?.AddFear(amount);
    }

    protected void ShowMessage(string message, float seconds)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return;
        }

        GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
        gameManager?.ShowMessage(message, seconds);
    }

    protected void PlayJumpscareSound()
    {
        AudioMoodDirector audioDirector = GetTree().GetFirstNodeInGroup("audio_director") as AudioMoodDirector;
        audioDirector?.PlayJumpscare();
    }

    private void FindTargetIfNeeded()
    {
        if (HasTarget())
        {
            return;
        }

        Target = GetTree().GetFirstNodeInGroup("player") as Node3D;
    }

    private Camera3D GetTargetCamera()
    {
        if (!HasTarget())
        {
            return null;
        }

        return Target.GetNodeOrNull<Camera3D>("CameraYaw/CameraPitch/Camera3D");
    }

    private void UpdateTimers(double delta)
    {
        if (AttackCooldownTimer > 0f)
        {
            AttackCooldownTimer -= (float)delta;
        }

        if (StunTimer > 0f)
        {
            StunTimer -= (float)delta;
        }
    }

    private void ApplyGravity(double delta, ref Vector3 velocity)
    {
        if (!IsOnFloor())
        {
            velocity.Y -= Gravity * (float)delta;
        }
        else if (velocity.Y < 0f)
        {
            velocity.Y = -0.1f;
        }
    }

    private void PulseBodyMesh()
    {
        if (BodyMesh == null)
        {
            return;
        }

        BodyMesh.Scale = new Vector3(1.12f, 1.12f, 1.12f);
        GetTree().CreateTimer(0.08).Timeout += () =>
        {
            if (IsInstanceValid(BodyMesh))
            {
                BodyMesh.Scale = Vector3.One;
            }
        };
    }

    private void Die()
    {
        ShowMessage($"{EnemyName} traci ksztalt.", 2.5f);
        QueueFree();
    }
}
