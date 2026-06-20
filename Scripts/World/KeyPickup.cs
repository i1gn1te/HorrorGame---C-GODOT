using Godot;

public partial class KeyPickup : Area3D
{
    // Klucz do drzwi albo przejścia.
    // KeyId musi być taki sam jak RequiredKeyId w skrypcie LockedDoor.

    [Export] public string KeyId = "level01_exit_key";
    [Export] public string DisplayName = "Zimny klucz";

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (!body.IsInGroup("player"))
        {
            return;
        }

        GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
        gameManager?.AddKey(KeyId, DisplayName);

        QueueFree();
    }
}
