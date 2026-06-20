using Godot;

public partial class PlayerFear : Node
{
    // Ten skrypt trzyma poziom strachu gracza.
    // Strach nie jest zdrowiem. To presja psychiczna: jumpscare, ciemność i hałas go podbijają.

    [Signal]
    public delegate void FearChangedEventHandler(float current, float maximum);

    [Export] public float MaxFear = 100f;
    [Export] public float CalmDownPerSecond = 4f;

    public float Fear { get; private set; }

    public override void _Ready()
    {
        EmitSignal(SignalName.FearChanged, Fear, MaxFear);
    }

    public override void _Process(double delta)
    {
        if (Fear <= 0f)
        {
            return;
        }

        Fear = Mathf.Max(0f, Fear - CalmDownPerSecond * (float)delta);
        EmitSignal(SignalName.FearChanged, Fear, MaxFear);
    }

    public void AddFear(float amount)
    {
        Fear = Mathf.Clamp(Fear + amount, 0f, MaxFear);
        EmitSignal(SignalName.FearChanged, Fear, MaxFear);
    }

    public void ResetFear()
    {
        Fear = 0f;
        EmitSignal(SignalName.FearChanged, Fear, MaxFear);
    }
}
