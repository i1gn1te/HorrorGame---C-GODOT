using Godot;

public partial class PipeRunnerEnemy : HorrorEnemyBase
{
    // Pipe Runner nie skrada sie caly czas.
    // Najpierw stoi i "nakreca" szarze, potem szybko wpada w korytarz.
    // To dobry enemy do ciasnych rur i zakretow.

    [ExportGroup("Pipe Runner")]
    [Export] public float WindupSeconds = 0.7f;
    [Export] public float ChargeSeconds = 0.85f;
    [Export] public float RecoverSeconds = 1.15f;
    [Export] public float ChargeSpeed = 8.4f;
    [Export] public string WindupMessage = "Rury zaczynaja stukac coraz szybciej.";

    private int _mode;
    private float _modeTimer;
    private bool _warned;
    private Vector3 _chargeDirection = Vector3.Forward;

    protected override void RunEnemyAi(double delta, ref Vector3 velocity)
    {
        if (!HasTarget() || DistanceToTarget() > AggroRange)
        {
            StopFlat(ref velocity);
            _mode = 0;
            return;
        }

        FaceTarget();

        if (_mode == 0)
        {
            StartWindup();
        }

        _modeTimer -= (float)delta;

        if (_mode == 1)
        {
            // Windup: gracz dostaje krotka informacje, ze zaraz bedzie szarza.
            StopFlat(ref velocity);

            if (_modeTimer <= 0f)
            {
                StartCharge();
            }

            return;
        }

        if (_mode == 2)
        {
            // Charge: enemy leci prosto. Gracz ma unik albo FlashBurst.
            MoveFlat(ref velocity, _chargeDirection, ChargeSpeed);

            if (DistanceToTarget() <= AttackRange)
            {
                TryAttackTarget(16f);
                StartRecover();
            }
            else if (_modeTimer <= 0f)
            {
                StartRecover();
            }

            return;
        }

        if (_mode == 3)
        {
            // Recover: po nieudanej szarzy enemy ma okno slabosci.
            StopFlat(ref velocity);

            if (_modeTimer <= 0f)
            {
                _mode = 0;
            }
        }
    }

    private void StartWindup()
    {
        _mode = 1;
        _modeTimer = WindupSeconds;

        if (!_warned)
        {
            _warned = true;
            ShowMessage(WindupMessage, 2f);
            PlayJumpscareSound();
        }
    }

    private void StartCharge()
    {
        _mode = 2;
        _modeTimer = ChargeSeconds;
        _chargeDirection = DirectionToTargetFlat();
    }

    private void StartRecover()
    {
        _mode = 3;
        _modeTimer = RecoverSeconds;
    }
}
