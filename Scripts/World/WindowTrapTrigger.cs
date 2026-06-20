using Godot;

public partial class WindowTrapTrigger : Area3D
{
    // Trigger do czarnych okien z Level 4.
    // W tej wersji okno nie zabija gracza, ale karze za zbyt bliskie podejscie:
    // strach rosnie, audio robi sie ciezsze, a HUD daje krotki komunikat.

    [Export] public float FearAmount = 22f;
    [Export] public float ThreatLevel = 0.75f;
    [Export] public string Message = "W czarnym oknie cos stoi dokladnie tak, jak ty.";
    [Export] public bool TriggerOnlyOnce = false;
    [Export] public float RepeatCooldownSeconds = 4f;

    private bool _wasTriggered;
    private float _cooldownTimer;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    public override void _Process(double delta)
    {
        if (_cooldownTimer > 0f)
        {
            _cooldownTimer -= (float)delta;
        }
    }

    private void OnBodyEntered(Node3D body)
    {
        if (_cooldownTimer > 0f || (_wasTriggered && TriggerOnlyOnce))
        {
            return;
        }

        if (!body.IsInGroup("player"))
        {
            return;
        }

        _wasTriggered = true;
        _cooldownTimer = RepeatCooldownSeconds;

        PlayerFear fear = body.GetNodeOrNull<PlayerFear>("PlayerFear");
        fear?.AddFear(FearAmount);

        AudioMoodDirector audioDirector = GetTree().GetFirstNodeInGroup("audio_director") as AudioMoodDirector;
        audioDirector?.SetThreatLevel(ThreatLevel);
        audioDirector?.PlayJumpscare();

        GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
        gameManager?.ShowMessage(Message, 3f);
    }
}
