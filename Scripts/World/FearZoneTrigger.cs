using Godot;

public partial class FearZoneTrigger : Area3D
{
    // Prosty trigger strachu.
    // Używaj go w miejscach, gdzie światło gaśnie, słychać hałas albo przestrzeń robi się zbyt ciasna.

    [Export] public float FearAmount = 18f;
    [Export] public string Message = "";
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

        PlayerFear fear = body.GetNodeOrNull<PlayerFear>("PlayerFear");
        fear?.AddFear(FearAmount);

        if (!string.IsNullOrWhiteSpace(Message))
        {
            GameBootstrap gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;
            gameManager?.ShowMessage(Message, 3f);
        }
    }
}
