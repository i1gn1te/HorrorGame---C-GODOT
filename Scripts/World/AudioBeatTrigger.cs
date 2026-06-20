using Godot;

public partial class AudioBeatTrigger : Area3D
{
    // Trigger do reżyserii dźwięku.
    // Wejście gracza może podbić napięcie, odpalić stinger i pokazać krótki tekst.

    [Export] public float ThreatLevel = 0.65f;
    [Export] public bool PlayStinger = true;
    [Export] public bool TriggerOnlyOnce = true;
    [Export] public string Message = "";

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

        AudioMoodDirector audioDirector = GetTree().GetFirstNodeInGroup("audio_director") as AudioMoodDirector;
        audioDirector?.SetThreatLevel(ThreatLevel);

        if (PlayStinger)
        {
            audioDirector?.PlayStinger();
        }

        if (!string.IsNullOrWhiteSpace(Message))
        {
            GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
            gameManager?.ShowMessage(Message, 3f);
        }
    }
}
