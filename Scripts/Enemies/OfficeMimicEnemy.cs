using Godot;

public partial class OfficeMimicEnemy : HorrorEnemyBase
{
    // Office Mimic nie musi od razu zabijac.
    // Jego rola to presja psychiczna: stoi jak mebel, udaje dekoracje i powoli podnosi strach.

    [ExportGroup("Office Mimic")]
    [Export] public float DriftSpeed = 1.05f;
    [Export] public float SeenDotThreshold = 0.78f;
    [Export] public float FearPerSecond = 8f;

    protected override void RunEnemyAi(double delta, ref Vector3 velocity)
    {
        if (!HasTarget() || DistanceToTarget() > AggroRange)
        {
            StopFlat(ref velocity);
            return;
        }

        FaceTarget();
        AddFearToTarget(FearPerSecond * (float)delta);

        if (DistanceToTarget() <= AttackRange)
        {
            StopFlat(ref velocity);
            TryAttackTarget(12f);
            return;
        }

        if (IsSeenByTarget(SeenDotThreshold))
        {
            // Gdy gracz patrzy, mimic udaje martwy obiekt.
            StopFlat(ref velocity);
            return;
        }

        // Gdy gracz odwraca wzrok, obiekt przesuwa sie blizej.
        MoveFlat(ref velocity, DirectionToTargetFlat(), DriftSpeed);
    }
}
