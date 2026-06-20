using Godot;

public partial class HealthPickup : Area3D
{
    // Prosty przedmiot leczący.
    // Później można go zamienić na apteczkę, fiolkę albo dziwny obiekt z Backroomsowego klimatu.

    [Export] public float HealAmount = 35f;
    [Export] public float StaminaAmount = 20f;
    [Export] public string PickupMessage = "Cialo przypomina sobie, jak oddychac.";

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

        PlayerStats stats = body.GetNodeOrNull<PlayerStats>("PlayerStats");

        if (stats == null)
        {
            return;
        }

        stats.Heal(HealAmount);
        stats.RestoreStamina(StaminaAmount);

        GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
        gameManager?.ShowMessage(PickupMessage, 2.5f);

        QueueFree();
    }
}
