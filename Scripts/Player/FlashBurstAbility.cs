using Godot;

public partial class FlashBurstAbility : Node
{
    // FlashBurst to pierwsza konkretna mechanika walki.
    // To nie jest klasyczna broń. To krótki błysk/impuls, który zatrzymuje byt na chwilę.
    // Ma działać jak horrorowa wersja szybkiej akcji: timing, odwaga, ryzyko, cooldown.

    [Export] public NodePath CameraPath = "../CameraYaw/CameraPitch/Camera3D";
    [Export] public NodePath StatsPath = "../PlayerStats";
    [Export] public NodePath FearPath = "../PlayerFear";
    [Export] public float Range = 7.5f;
    [Export] public float ConeAngleDegrees = 38f;
    [Export] public float LightDamage = 18f;
    [Export] public float HeavyDamage = 34f;
    [Export] public float LightStaminaCost = 22f;
    [Export] public float HeavyStaminaCost = 42f;
    [Export] public float CooldownSeconds = 0.85f;
    [Export] public float EnemyStunSeconds = 1.45f;
    [Export] public float FearRelief = 10f;

    private Camera3D _camera;
    private PlayerStats _stats;
    private PlayerFear _fear;
    private float _cooldownTimer;

    public override void _Ready()
    {
        _camera = GetNodeOrNull<Camera3D>(CameraPath);
        _stats = GetNodeOrNull<PlayerStats>(StatsPath);
        _fear = GetNodeOrNull<PlayerFear>(FearPath);
    }

    public override void _Process(double delta)
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= (float)delta;
        }

        if (Input.IsActionJustPressed("attack_light"))
        {
            TryBurst(LightDamage, LightStaminaCost, 1f);
        }
        else if (Input.IsActionJustPressed("attack_heavy"))
        {
            TryBurst(HeavyDamage, HeavyStaminaCost, 1.6f);
        }
    }

    private void TryBurst(float damage, float staminaCost, float stunMultiplier)
    {
        if (_cooldownTimer > 0f || _camera == null || _stats == null)
        {
            return;
        }

        if (!_stats.TrySpendStamina(staminaCost))
        {
            return;
        }

        _cooldownTimer = CooldownSeconds;
        _fear?.AddFear(-FearRelief);

        GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
        gameManager?.ShowMessage("Blysk rozcina powietrze tylko na moment.", 1.4f);

        AudioMoodDirector audioDirector = GetTree().GetFirstNodeInGroup("audio_director") as AudioMoodDirector;
        audioDirector?.PlayFlashBurst();

        foreach (Node node in GetTree().GetNodesInGroup("enemies"))
        {
            if (node is not Node3D enemy)
            {
                continue;
            }

            if (!IsEnemyInsideCone(enemy))
            {
                continue;
            }

            if (enemy is IDamageable damageable)
            {
                damageable.TakeDamage(damage, _camera);
            }

            if (enemy is IStunnable stunnable)
            {
                stunnable.Stun(EnemyStunSeconds * stunMultiplier);
            }
        }
    }

    private bool IsEnemyInsideCone(Node3D enemy)
    {
        Vector3 toEnemy = enemy.GlobalPosition - _camera.GlobalPosition;
        float distance = toEnemy.Length();

        if (distance > Range || distance <= 0.01f)
        {
            return false;
        }

        Vector3 forward = -_camera.GlobalTransform.Basis.Z.Normalized();
        float dot = forward.Dot(toEnemy.Normalized());
        float requiredDot = Mathf.Cos(Mathf.DegToRad(ConeAngleDegrees));
        return dot >= requiredDot;
    }
}
