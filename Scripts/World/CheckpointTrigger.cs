using Godot;

public partial class CheckpointTrigger : Area3D
{
    // Checkpoint zapisuje miejsce powrotu po śmierci.
    // Na razie aktywuje się automatycznie, gdy gracz wejdzie w obszar.

    [Export] public string CheckpointMessage = "Powietrze na moment staje sie stabilne.";
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
        gameManager?.SetCheckpoint(GlobalPosition, CheckpointMessage);
    }
}
