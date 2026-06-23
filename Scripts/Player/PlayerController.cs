using Godot;
using System.Collections.Generic;

public partial class PlayerController : CharacterBody3D
{
    // Ten skrypt steruje graczem:
    // - wolny ruch w pierwszej osobie,
    // - płynna kamera,
    // - VHS / handheld feeling podczas chodzenia,
    // - stamina,
    // - unik,
    // - prosty fundament pod akcję horrorową.

    [ExportGroup("Ruch")]
    [Export] public float WalkSpeed = 3.65f;
    [Export] public float SprintSpeed = 6.35f;
    [Export] public float Acceleration = 4.2f;
    [Export] public float Deceleration = 5.8f;
    [Export] public float Gravity = 24f;

    [ExportGroup("Kamera - Obrót")]
    [Export] public float MouseSensitivity = 0.00115f;
    [Export] public float MinCameraPitch = -0.85f;
    [Export] public float MaxCameraPitch = 0.35f;
    [Export] public float LookSmoothness = 16.0f;
    [Export] public float CameraFov = 66.0f;

    [ExportGroup("Kamera - VHS Chód")]
    [Export] public float WalkHeadBobFrequency = 2.85f;
    [Export] public float SprintHeadBobFrequency = 4.15f;

    [Export] public float WalkVerticalBobAmount = 0.012f;
    [Export] public float SprintVerticalBobAmount = 0.020f;

    [Export] public float WalkSideSwayAmount = 0.006f;
    [Export] public float SprintSideSwayAmount = 0.010f;

    [Export] public float WalkForwardSwayAmount = 0.004f;
    [Export] public float SprintForwardSwayAmount = 0.007f;

    [Export] public float WalkCameraRollDegrees = 0.18f;
    [Export] public float SprintCameraRollDegrees = 0.35f;

    [Export] public float CameraMotionSmoothness = 8.0f;

    [ExportGroup("Kamera - Idle")]
    [Export] public float IdleBreathingFrequency = 0.75f;
    [Export] public float IdleBreathingAmount = 0.003f;
    [Export] public float IdleDriftAmount = 0.002f;

    [ExportGroup("Unik")]
    [Export] public float DodgeSpeed = 8.5f;
    [Export] public float DodgeDuration = 0.22f;
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
    private Camera3D _camera;
    private Area3D _weaponHitbox;
    private PlayerStats _stats;

    private Vector3 _lastMoveDirection = Vector3.Forward;
    private Vector3 _dodgeDirection = Vector3.Forward;
    private Vector3 _weaponHitboxStartPosition;

    private float _facingYaw;

    private float _targetYaw;
    private float _targetPitch;
    private float _currentYaw;
    private float _currentPitch;

    private Vector3 _cameraYawStartPosition;
    private Vector3 _cameraStartPosition;
    private Vector3 _cameraCurrentOffset;
    private Vector3 _cameraStartRotation;

    private float _headBobTime;
    private float _idleTime;
    private float _cameraCurrentRoll;

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
        _camera = GetNode<Camera3D>("CameraYaw/CameraPitch/Camera3D");

        _cameraYawStartPosition = _cameraYaw.Position;
        _cameraStartPosition = _camera.Position;
        _cameraStartRotation = _camera.Rotation;

        _targetYaw = _cameraYaw.Rotation.Y;
        _currentYaw = _cameraYaw.Rotation.Y;

        _targetPitch = _cameraPitch.Rotation.X;
        _currentPitch = _cameraPitch.Rotation.X;

        _camera.Fov = CameraFov;

        Input.MouseMode = Input.MouseModeEnum.Captured;

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

        if (inputEvent is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured)
        {
            _targetYaw -= mouseMotion.Relative.X * MouseSensitivity;

            _targetPitch -= mouseMotion.Relative.Y * MouseSensitivity;
            _targetPitch = Mathf.Clamp(_targetPitch, MinCameraPitch, MaxCameraPitch);
        }
    }

    public override void _Process(double delta)
    {
        SmoothCameraRotation(delta);
        UpdateVhsCameraMotion(delta);
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

    private void SmoothCameraRotation(double delta)
    {
        float dt = (float)delta;
        float smoothT = 1.0f - Mathf.Exp(-LookSmoothness * dt);

        _currentYaw = Mathf.LerpAngle(_currentYaw, _targetYaw, smoothT);
        _currentPitch = Mathf.Lerp(_currentPitch, _targetPitch, smoothT);

        _cameraYaw.Rotation = new Vector3(0f, _currentYaw, 0f);
        _cameraPitch.Rotation = new Vector3(_currentPitch, 0f, 0f);
    }

    private void UpdateVhsCameraMotion(double delta)
    {
        float dt = (float)delta;

        Vector3 horizontalVelocity = new Vector3(Velocity.X, 0f, Velocity.Z);
        float horizontalSpeed = horizontalVelocity.Length();

        bool isMoving = IsOnFloor() && horizontalSpeed > 0.05f && _dodgeTimer <= 0f;
        bool isSprinting = Input.IsActionPressed("sprint") && isMoving;

        Vector3 targetOffset = Vector3.Zero;
        float targetRoll = 0f;

        _idleTime += dt;

        if (isMoving)
        {
            float maxSpeed = isSprinting ? SprintSpeed : WalkSpeed;
            float speedRatio = Mathf.Clamp(horizontalSpeed / maxSpeed, 0.0f, 1.0f);

            float frequency = isSprinting ? SprintHeadBobFrequency : WalkHeadBobFrequency;

            float verticalAmount = isSprinting ? SprintVerticalBobAmount : WalkVerticalBobAmount;
            float sideAmount = isSprinting ? SprintSideSwayAmount : WalkSideSwayAmount;
            float forwardAmount = isSprinting ? SprintForwardSwayAmount : WalkForwardSwayAmount;

            float rollDegrees = isSprinting ? SprintCameraRollDegrees : WalkCameraRollDegrees;
            float rollRadians = Mathf.DegToRad(rollDegrees);

            _headBobTime += dt * horizontalSpeed * frequency;

            float step = _headBobTime;

            float verticalBob = Mathf.Abs(Mathf.Sin(step)) * verticalAmount;
            verticalBob -= verticalAmount * 0.5f;

            float sideSway = Mathf.Sin(step * 0.5f) * sideAmount;
            float forwardSway = Mathf.Cos(step) * forwardAmount;

            targetOffset = new Vector3(
                sideSway,
                verticalBob,
                forwardSway
            ) * speedRatio;

            targetRoll = -Mathf.Sin(step * 0.5f) * rollRadians * speedRatio;
        }
        else
        {
            float idleVertical = Mathf.Sin(_idleTime * IdleBreathingFrequency) * IdleBreathingAmount;
            float idleSide = Mathf.Sin(_idleTime * IdleBreathingFrequency * 0.43f) * IdleDriftAmount;

            targetOffset = new Vector3(idleSide, idleVertical, 0f);
            targetRoll = Mathf.Sin(_idleTime * 0.35f) * Mathf.DegToRad(0.04f);
        }

        float smoothT = 1.0f - Mathf.Exp(-CameraMotionSmoothness * dt);

        _cameraCurrentOffset = _cameraCurrentOffset.Lerp(targetOffset, smoothT);
        _cameraCurrentRoll = Mathf.Lerp(_cameraCurrentRoll, targetRoll, smoothT);

        // Ważne:
        // CameraYaw NIE może być przesuwany przez headbob,
        // bo jest pivotem obrotu kamery.
        // Gdy pivot się rusza i jednocześnie obracasz myszką,
        // kamera może wyglądać jakby glitchowała.
        _cameraYaw.Position = _cameraYawStartPosition;

        // Headbob przesuwamy tylko na właściwej kamerze.
        _camera.Position = _cameraStartPosition + _cameraCurrentOffset;

        _camera.Rotation = new Vector3(
            _cameraStartRotation.X,
            _cameraStartRotation.Y,
            _cameraStartRotation.Z + _cameraCurrentRoll
        );
    }

    private Vector3 GetCameraRelativeDirection(Vector2 input)
    {
        if (input.LengthSquared() <= 0.01f)
        {
            return Vector3.Zero;
        }

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
        float dt = (float)delta;

        Vector3 velocity = Velocity;

        if (!IsOnFloor())
        {
            velocity.Y -= Gravity * dt;
        }
        else if (velocity.Y < 0f)
        {
            velocity.Y = -0.1f;
        }

        Vector3 currentHorizontalVelocity = new Vector3(velocity.X, 0f, velocity.Z);
        Vector3 targetHorizontalVelocity;

        if (_dodgeTimer > 0f)
        {
            _dodgeTimer -= dt;

            targetHorizontalVelocity = _dodgeDirection * DodgeSpeed;

            currentHorizontalVelocity = currentHorizontalVelocity.MoveToward(
                targetHorizontalVelocity,
                DodgeSpeed * 14f * dt
            );
        }
        else
        {
            bool wantsSprint = Input.IsActionPressed("sprint") && moveDirection.LengthSquared() > 0.01f;

            float speed = wantsSprint ? SprintSpeed : WalkSpeed;
            targetHorizontalVelocity = moveDirection * speed;

            float speedChange = targetHorizontalVelocity.LengthSquared() > 0.01f
                ? Acceleration
                : Deceleration;

            currentHorizontalVelocity = currentHorizontalVelocity.MoveToward(
                targetHorizontalVelocity,
                speedChange * dt
            );
        }

        velocity.X = currentHorizontalVelocity.X;
        velocity.Z = currentHorizontalVelocity.Z;

        Velocity = velocity;

        MoveAndSlide();
    }

    private void UpdateFirstPersonFacing()
    {
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
        // W wersji FPP horrorowej zostawiamy tę metodę pustą.
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
        return Mathf.Atan2(-direction.X, -direction.Z);
    }

    private Vector3 RotateByYaw(Vector3 value, float yaw)
    {
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