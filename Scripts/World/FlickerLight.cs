using Godot;

public partial class FlickerLight : OmniLight3D
{
    // Proste migotanie swiatla.
    // Daje klimat jarzeniowek i dziwnego, niestabilnego miejsca.

    [Export] public float BaseEnergy = 1.0f;
    [Export] public float FlickerStrength = 0.35f;
    [Export] public float FlickerSpeed = 9f;
    [Export] public float RandomDropChance = 0.012f;

    private float _timeOffset;
    private RandomNumberGenerator _rng = new RandomNumberGenerator();

    public override void _Ready()
    {
        _rng.Randomize();
        _timeOffset = _rng.RandfRange(0f, 100f);
    }

    public override void _Process(double delta)
    {
        float wave = Mathf.Sin((float)Time.GetTicksMsec() / 1000f * FlickerSpeed + _timeOffset);
        float randomDrop = _rng.Randf() < RandomDropChance ? _rng.RandfRange(0.2f, 0.55f) : 1f;
        LightEnergy = (BaseEnergy + wave * FlickerStrength) * randomDrop;
    }
}
