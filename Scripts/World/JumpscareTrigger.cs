using Godot;

public partial class JumpscareTrigger : Area3D
{
    // Trigger reżyseruje jumpscare.
    // Nie jest to zwykła walka ani puzzle: gracz przekracza punkt, a scena nagle zmienia stan.

    [Export] public NodePath EnemyPath = "";
    [Export] public NodePath OptionalLightPath = "";
    [Export] public float ThreatLevel = 1f;
    [Export] public float FearAmount = 35f;
    [Export] public string Message = "Cos wychodzi zza rogu.";
    [Export] public bool TriggerOnlyOnce = true;

    private bool _wasTriggered;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (_wasTriggered && TriggerOnlyOnce)
        {
            return;
        }

        if (!body.IsInGroup("player"))
        {
            return;
        }

        _wasTriggered = true;

        Node enemy = GetNodeOrNull<Node>(EnemyPath);

        if (enemy is IJumpscareActivatable activatableEnemy)
        {
            activatableEnemy.ActivateJumpscare();
        }

        Light3D light = GetNodeOrNull<Light3D>(OptionalLightPath);

        if (light != null)
        {
            light.LightEnergy *= 2.8f;
        }

        AudioMoodDirector audioDirector = GetTree().GetFirstNodeInGroup("audio_director") as AudioMoodDirector;
        audioDirector?.SetThreatLevel(ThreatLevel);
        audioDirector?.PlayJumpscare();

        PlayerFear fear = body.GetNodeOrNull<PlayerFear>("PlayerFear");
        fear?.AddFear(FearAmount);

        GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
        gameManager?.ShowMessage(Message, 2.5f);
    }
}
