using Godot;

public partial class SilentBeatTrigger : Area3D
{
    // Mały fundament pod narrację i horror.
    // Gdy gracz wejdzie w strefę, wypisujemy krótki tekst do konsoli.
    // Później w tym miejscu możesz odpalać dźwięki, miganie świateł albo dialogi.

    [Export] public string BeatText = "Coś jest nie tak z tym miejscem.";
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

        GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
        gameManager?.ShowMessage(BeatText, 4f);
    }
}
