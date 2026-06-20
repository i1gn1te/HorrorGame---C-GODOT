using Godot;

public partial class WallStoryTrigger : Area3D
{
    // Ten trigger dodajemy przy obrazach, mapach i napisach na scianie.
    // Gracz nie musi wciskac przycisku: wystarczy, ze podejdzie blisko.
    // Dzieki temu fabula moze byc opowiadana przez przestrzen, a nie przez dlugie dialogi.

    [Export] public string StoryText = "Obraz wyglada, jakby byl zrobiony juz po twoim wejsciu.";
    [Export] public float FearAmount = 8f;
    [Export] public float ThreatLevel = 0.2f;
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

        // Strach jest osobnym paskiem od zdrowia.
        // Mozesz go pozniej wykorzystac np. do drzenia kamery albo halucynacji.
        PlayerFear fear = body.GetNodeOrNull<PlayerFear>("PlayerFear");
        fear?.AddFear(FearAmount);

        // AudioMoodDirector miesza warstwe basu i stingerow.
        AudioMoodDirector audioDirector = GetTree().GetFirstNodeInGroup("audio_director") as AudioMoodDirector;
        audioDirector?.SetThreatLevel(ThreatLevel);
        audioDirector?.PlayStoryWhisper();

        GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
        gameManager?.ShowMessage(StoryText, 4f);
    }
}
