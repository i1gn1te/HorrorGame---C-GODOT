using Godot;

public partial class AudioProfileTrigger : Area3D
{
    // Ten trigger ustawia klimat audio dla aktualnego poziomu.
    // Wrzuc go przy PlayerSpawn, zeby gracz dostal nowe tlo dzwiekowe od razu po wejsciu.

    [Export] public string HumStreamPath = "res://Assets/Audio/ambient_fluorescent_hum.wav";
    [Export] public string RumbleStreamPath = "res://Assets/Audio/ambient_low_rumble.wav";
    [Export] public float HumVolumeDb = -18f;
    [Export] public float RumbleCalmVolumeDb = -34f;
    [Export] public float RumbleThreatVolumeDb = -20f;
    [Export] public float StartThreatLevel = 0f;
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

        AudioMoodDirector audioDirector = GetTree().GetFirstNodeInGroup("audio_director") as AudioMoodDirector;
        audioDirector?.SetAmbientProfile(
            HumStreamPath,
            RumbleStreamPath,
            HumVolumeDb,
            RumbleCalmVolumeDb,
            RumbleThreatVolumeDb,
            StartThreatLevel
        );
    }
}
