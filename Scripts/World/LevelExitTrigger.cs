using Godot;

public partial class LevelExitTrigger : Area3D
{
    // Ten trigger kończy poziom.
    // Jeżeli podasz ścieżkę do następnej sceny, Godot ją wczyta.

    [Export] public string NextScenePath = "";
    [Export] public string MissingSceneMessage = "Tu później zacznie się następny poziom.";

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

        if (string.IsNullOrWhiteSpace(NextScenePath) || !ResourceLoader.Exists(NextScenePath))
        {
            GameBootstrap missingManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
            missingManager?.ShowMessage(MissingSceneMessage, 3f);
            return;
        }

        GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;

        if (gameManager != null)
        {
            gameManager.LoadLevel(NextScenePath);
        }
        else
        {
            GetTree().ChangeSceneToFile(NextScenePath);
        }
    }
}
