using Godot;

public partial class WatcherMannequinEnemy : HorrorEnemyBase
{
    // Ten enemy dziala jak "nie patrzysz = rusza".
    // Gdy kamera gracza jest skierowana na manekina, manekin staje.
    // Gdy gracz odwraca wzrok, manekin powoli skraca dystans.

    [ExportGroup("Watcher")]
    [Export] public float MoveSpeed = 2.15f;
    [Export] public float SeenDotThreshold = 0.82f;
    [Export] public float SeenFearPerSecond = 5f;

    protected override void RunEnemyAi(double delta, ref Vector3 velocity)
    {
        if (!HasTarget() || DistanceToTarget() > AggroRange)
        {
            StopFlat(ref velocity);
            return;
        }

        FaceTarget();

        if (IsSeenByTarget(SeenDotThreshold))
        {
            // Patrzenie na manekina zatrzymuje go, ale nie uspokaja.
            StopFlat(ref velocity);
            AddFearToTarget(SeenFearPerSecond * (float)delta);
            return;
        }

        if (DistanceToTarget() <= AttackRange)
        {
            StopFlat(ref velocity);
            TryAttackTarget(10f);
            return;
        }

        MoveFlat(ref velocity, DirectionToTargetFlat(), MoveSpeed);
    }
}
