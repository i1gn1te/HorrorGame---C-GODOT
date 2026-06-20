using Godot;

public partial class LockedDoor : Area3D
{
    // Obszar przy drzwiach.
    // Jeśli gracz ma wymagany klucz, drzwi znikają i wyłączamy ich kolizję.

    [Export] public string RequiredKeyId = "level01_exit_key";
    [Export] public string DoorNodePath = "";
    [Export] public string LockedMessage = "Drzwi nie reaguja. Brakuje czegos malego i zimnego.";
    [Export] public string OpenMessage = "Mechanizm puszcza bez dzwieku.";

    private bool _isOpen;

    public override void _Ready()
    {
        BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node3D body)
    {
        if (_isOpen || !body.IsInGroup("player"))
        {
            return;
        }

        GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;

        if (gameManager == null || !gameManager.HasKey(RequiredKeyId))
        {
            gameManager?.ShowMessage(LockedMessage, 3f);
            return;
        }

        OpenDoor();
        gameManager.ShowMessage(OpenMessage, 3f);
    }

    private void OpenDoor()
    {
        _isOpen = true;

        if (string.IsNullOrWhiteSpace(DoorNodePath))
        {
            return;
        }

        Node door = GetNodeOrNull(new NodePath(DoorNodePath));

        if (door is Node3D doorNode)
        {
            doorNode.Visible = false;
            doorNode.Set("use_collision", false);
        }
    }
}
