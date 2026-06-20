using Godot;
using System.Collections.Generic;

public partial class PlayerController : CharacterBody3D
{
    // Ten skrypt steruje graczem:
    // - ruch w pierwszej osobie,
    // - kamera z oczu gracza,
    // - stamina,
    // - unik,
    // - prosty atak,
    // - prosty fundament pod akcję horrorową.

    [ExportGroup("Ruch")]
    [Export] public float WalkSpeed = 4.1f;
    [Export] public float SprintSpeed = 6.4f;
    [Export] public float Acceleration = 22f;
    [Export] public float Deceleration = 30f;
    [Export] public float Gravity = 24f;

    [ExportGroup("Kamera")]
    [Export] public float MouseSensitivity = 0.00135f;
    [Export] public float MinCameraPitch = -0.85f;
    [Export] public float MaxCameraPitch = 0.35f;

    [ExportGroup("Unik")]
    [Export] public float DodgeSpeed = 11f;
    [Export] public float DodgeDuration = 0.28f;
    [Export] public float DodgeStaminaCost = 28f;

    [ExportGroup("Walka")]
    [Export] public float LightAttackDamage = 25f;
    [Export] public float HeavyAttackDamage = 45f;
    [Export] public float LightAttackStaminaCost = 18f;
    [Export] public float HeavyAttackStaminaCost = 34f;
    [Export] public float AttackDuration = 0.48f;
    [Export] public float AttackWindup = 0.14f;
    [Export] public float AttackActiveTime = 0.16f;
    [Export] public float LockOnRange = 12f;

    private Node3D _visualRoot;
    private Node3D _cameraYaw;
    private Node3D _cameraPitch;
    private Area3D _weaponHitbox;
    private PlayerStats _stats;

    private Vector3 _lastMoveDirection = Vector3.Forward;
    private Vector3 _dodgeDirection = Vector3.Forward;
    private Vector3 _weaponHitboxStartPosition;
    private float _facingYaw;
    private float _dodgeTimer;
    private float _attackTimer;
    private float _currentAttackDamage;
    private bool _isDead;
    private readonly HashSet<Node> _alreadyHitThisAttack = new HashSet<Node>();

    public override void _Ready()
    {
        AddToGroup("player");

        _visualRoot = GetNode<Node3D>("VisualRoot");
        _cameraYaw = GetNode<Node3D>("CameraYaw");
        _cameraPitch = GetNode<Node3D>("CameraYaw/CameraPitch");
        _weaponHitbox = GetNode<Area3D>("WeaponHitbox");
        _stats = GetNode<PlayerStats>("PlayerStats");
        _weaponHitboxStartPosition = _weaponHitbox.Position;

        _weaponHitbox.Monitoring = false;
        _weaponHitbox.BodyEntered += OnWeaponBodyEntered;
        _stats.Died += OnDied;
    }

    public override void _UnhandledInput(InputEvent inputEvent)
    {
        if (Input.IsActionJustPressed("pause"))
        {
            ToggleMouseCapture();
        }

        // Gdy kursor jest złapany przez grę, ruch myszy obraca kamerę.
        if (inputEvent is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            _cameraYaw.RotateY(-mouseMotion.Relative.X * MouseSensitivity);

            float newPitch = _cameraPitch.Rotation.X - mouseMotion.Relative.Y * MouseSensitivity;
            newPitch = Mathf.Clamp(newPitch, MinCameraPitch, MaxCameraPitch);
            _cameraPitch.Rotation = new Vector3(newPitch, 0f, 0f);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (_isDead)
        {
            Velocity = Vector3.Zero;
            return;
        }

        Vector2 input = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Vector3 moveDirection = GetCameraRelativeDirection(input);

        if (moveDirection.LengthSquared() > 0.01f)
        {
            _lastMoveDirection = moveDirection;
        }

        TryStartDodge(moveDirection);
        MovePlayer(delta, moveDirection);
        UpdateFirstPersonFacing();

        bool canRecoverStamina = _attackTimer <= 0f && _dodgeTimer <= 0f;
        _stats.RecoverStamina(delta, canRecoverStamina);
    }

    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        if (input.LengthSquared() <= 0.01f)
        {
            return Vector3.Zero;
        }

        // Kamera patrzy w stronę -Z. Z tej informacji robimy kierunek ruchu na płaskiej podłodze.
        Vector3 forward = -_cameraYaw.GlobalTransform.Basis.Z;
        Vector3 right = _cameraYaw.GlobalTransform.Basis.X;

        forward.Y = 0f;
        right.Y = 0f;
        forward = forward.Normalized();
        right = right.Normalized();

        return (right * input.X - forward * input.Y).Normalized();
    }

    private void MovePlayer(double delta, Vector3 moveDirection)
    {
        Vector3 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity.Y -= Gravity * (float)delta;
        }
        else if (velocity.Y < 0f)
        {
            velocity.Y = -0.1f;
        }

        Vector3 currentHorizontalVelocity = new Vector3(velocity.X, 0f, velocity.Z);
        Vector3 targetHorizontalVelocity;

        if (_dodgeTimer > 0f)
        {
            _dodgeTimer -= (float)delta;
            targetHorizontalVelocity = _dodgeDirection * DodgeSpeed;
            currentHorizontalVelocity = targetHorizontalVelocity;
        }
        else
        {
            bool wantsSprint = Input.IsActionPressed("sprint") && moveDirection.LengthSquared() > 0.01f;
            float speed = wantsSprint ? SprintSpeed : WalkSpeed;
            targetHorizontalVelocity = moveDirection * speed;

            // Ruch jest miękki: postać przyspiesza i hamuje zamiast natychmiast zmieniać prędkość.
            float speedChange = targetHorizontalVelocity.LengthSquared() > 0.01f ? Acceleration : Deceleration;
            currentHorizontalVelocity = currentHorizontalVelocity.MoveToward(targetHorizontalVelocity, speedChange * (float)delta);
        }

        velocity.X = currentHorizontalVelocity.X;
        velocity.Z = currentHorizontalVelocity.Z;
        Velocity = velocity;

        MoveAndSlide();
    }

    private void UpdateFirstPersonFacing()
    {
        // W FPP kierunek postaci to kierunek kamery.
        // Nie obracamy całego CharacterBody3D, żeby ruch i kolizje zostały stabilne.
        _facingYaw = _cameraYaw.Rotation.Y;
        ApplyFacingRotation();
    }

    private void TryStartDodge(Vector3 moveDirection)
    {
        if (!Input.IsActionJustPressed("dodge") || _dodgeTimer > 0f || _attackTimer > 0f)
        {
            return;
        }

        if (!_stats.TrySpendStamina(DodgeStaminaCost))
        {
            return;
        }

        _dodgeDirection = moveDirection.LengthSquared() > 0.01f ? moveDirection : GetFacingForward();
        _dodgeTimer = DodgeDuration;
    }

    private void TryStartAttack()
    {
        if (_attackTimer > 0f || _dodgeTimer > 0f)
        {
            return;
        }

        if (Input.IsActionJustPressed("attack_light"))
        {
            StartAttack(LightAttackDamage, LightAttackStaminaCost);
        }
        else if (Input.IsActionJustPressed("attack_heavy"))
        {
            StartAttack(HeavyAttackDamage, HeavyAttackStaminaCost);
        }
    }

    private void StartAttack(float damage, float staminaCost)
    {
        if (!_stats.TrySpendStamina(staminaCost))
        {
            return;
        }

        _currentAttackDamage = damage;
        _attackTimer = AttackDuration;
        _alreadyHitThisAttack.Clear();
    }

    private void ProcessAttack(double delta)
    {
        if (_attackTimer <= 0f)
        {
            _weaponHitbox.Monitoring = false;
            return;
        }

        _attackTimer -= (float)delta;

        float elapsed = AttackDuration - _attackTimer;
        bool hitboxIsActive = elapsed >= AttackWindup && elapsed <= AttackWindup + AttackActiveTime;
        _weaponHitbox.Monitoring = hitboxIsActive;
    }

    private void OnWeaponBodyEntered(Node3D body)
    {
        // Hitbox może dotknąć ściany albo podłogi, więc filtrujemy tylko grupę "enemies".
        if (!_weaponHitbox.Monitoring || !body.IsInGroup("enemies") || _alreadyHitThisAttack.Contains(body))
        {
            return;
        }

        _alreadyHitThisAttack.Add(body);
        if (body is IDamageable damageable)
        {
            damageable.TakeDamage(_currentAttackDamage, this);
        }
    }

    private void ToggleLockOn()
    {
        // Lock-on był potrzebny w wersji TPP soulslike.
        // W wersji FPP horrorowej zostawiamy tę metodę pustą, żeby Tab niczego nie psuł.
    }

    private Node3D FindNearestEnemy()
    {
        Node3D nearestEnemy = null;
        float nearestDistance = LockOnRange;

        foreach (Node node in GetTree().GetNodesInGroup("enemies"))
        {
            if (node is not Node3D enemy)
            {
                continue;
            }

            float distance = GlobalPosition.DistanceTo(enemy.GlobalPosition);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private void ApplyFacingRotation()
    {
        // VisualRoot obraca tylko wygląd gracza.
        // WeaponHitbox obraca i przesuwa pole obrażeń tak, żeby zawsze było przed postacią.
        _visualRoot.Rotation = new Vector3(0f, _facingYaw, 0f);
        _weaponHitbox.Rotation = new Vector3(0f, _facingYaw, 0f);
        _weaponHitbox.Position = RotateByYaw(_weaponHitboxStartPosition, _facingYaw);
    }

    private Vector3 GetFacingForward()
    {
        return RotateByYaw(Vector3.Forward, _facingYaw).Normalized();
    }

    private float GetYawFromDirection(Vector3 direction)
    {
        // W Godot przód modelu to zwykle kierunek -Z, czyli Vector3.Forward.
        return Mathf.Atan2(-direction.X, -direction.Z);
    }

    private Vector3 RotateByYaw(Vector3 value, float yaw)
    {
        // Ręczna rotacja po osi Y.
        // Dzięki temu nie musimy obracać całego gracza razem z kamerą.
        float sin = Mathf.Sin(yaw);
        float cos = Mathf.Cos(yaw);

        return new Vector3(
            value.X * cos + value.Z * sin,
            value.Y,
            -value.X * sin + value.Z * cos
        );
    }

    private void ToggleMouseCapture()
    {
        if (Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            Input.MouseMode = Input.MouseModeEnum.Visible;
        }
        else
        {
            Input.MouseMode = Input.MouseModeEnum.Captured;
        }
    }

    private void OnDied()
    {
        if (_isDead)
        {
            return;
        }

        _isDead = true;

        GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
        gameManager?.ShowMessage("Umierasz, ale to miejsce nie chce cie wypuscic.", 3f);

        GetTree().CreateTimer(1.2).Timeout += () =>
        {
            gameManager?.RespawnPlayer();
            _isDead = false;
        };
    }
}
