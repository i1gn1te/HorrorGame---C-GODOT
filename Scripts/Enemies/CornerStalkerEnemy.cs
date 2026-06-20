using Godot;

public partial class CornerStalkerEnemy : HorrorEnemyBase
{
    // Corner Stalker jest duzy i wolny, ale co kilka sekund robi krotki zryw.
    // Uzywaj go do mocnych momentow zza rogu albo jako "mini-bossa" poziomu.

    [ExportGroup("Corner Stalker")]
    [Export] public float WalkSpeed = 1.45f;
    [Export] public float LurchSpeed = 4.6f;
    [Export] public float LurchEverySeconds = 3.4f;
    [Export] public float LurchDuration = 0.55f;

    private float _lurchCooldown;
    private float _lurchTimer;

    protected override void RunEnemyAi(double delta, ref Vector3 velocity)
    {
        if (!HasTarget() || DistanceToTarget() > AggroRange)
        {
            StopFlat(ref velocity);
            return;
        }

        FaceTarget();

        if (DistanceToTarget() <= AttackRange)
        {
            StopFlat(ref velocity);
            TryAttackTarget(18f);
            return;
        }

        _lurchCooldown -= (float)delta;

        if (_lurchTimer > 0f)
        {
            _lurchTimer -= (float)delta;
            MoveFlat(ref velocity, DirectionToTargetFlat(), LurchSpeed);
            return;
        }

        if (_lurchCooldown <= 0f)
        {
            _lurchCooldown = LurchEverySeconds;
            _lurchTimer = LurchDuration;
            PlayJumpscareSound();
        }

        MoveFlat(ref velocity, DirectionToTargetFlat(), WalkSpeed);
    }
}
