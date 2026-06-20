using Godot;

public partial class PlayerStats : Node
{
    // Ten skrypt trzyma liczby gracza: życie i staminę.
    // Inne skrypty pytają tutaj: "czy gracz ma siłę na unik albo atak?".

    [Signal]
    public delegate void HealthChangedEventHandler(float current, float maximum);

    [Signal]
    public delegate void StaminaChangedEventHandler(float current, float maximum);

    [Signal]
    public delegate void DiedEventHandler();

    [Export] public float MaxHealth = 100f;
    [Export] public float MaxStamina = 100f;
    [Export] public float StaminaRegenPerSecond = 24f;

    public float Health { get; private set; }
    public float Stamina { get; private set; }

    public override void _Ready()
    {
        ResetStats();
    }

    public void ResetStats()
    {
        // Reset przy starcie gry albo po śmierci gracza.
        Health = MaxHealth;
        Stamina = MaxStamina;

        EmitSignal(SignalName.HealthChanged, Health, MaxHealth);
        EmitSignal(SignalName.StaminaChanged, Stamina, MaxStamina);
    }

    public bool TrySpendStamina(float amount)
    {
        // Zwracamy false, jeżeli gracz nie ma dość staminy.
        if (Stamina < amount)
        {
            return false;
        }

        Stamina -= amount;
        EmitSignal(SignalName.StaminaChanged, Stamina, MaxStamina);
        return true;
    }

    public void RecoverStamina(double delta, bool canRecover)
    {
        // Stamina nie odnawia się w trakcie ataku albo uniku.
        if (!canRecover)
        {
            return;
        }

        float oldValue = Stamina;
        Stamina = Mathf.Min(MaxStamina, Stamina + StaminaRegenPerSecond * (float)delta);

        if (!Mathf.IsEqualApprox(oldValue, Stamina))
        {
            EmitSignal(SignalName.StaminaChanged, Stamina, MaxStamina);
        }
    }

    public void TakeDamage(float amount)
    {
        Health = Mathf.Max(0f, Health - amount);
        EmitSignal(SignalName.HealthChanged, Health, MaxHealth);

        if (Health <= 0f)
        {
            EmitSignal(SignalName.Died);
        }
    }

    public void Heal(float amount)
    {
        Health = Mathf.Min(MaxHealth, Health + amount);
        EmitSignal(SignalName.HealthChanged, Health, MaxHealth);
    }

    public void RestoreStamina(float amount)
    {
        Stamina = Mathf.Min(MaxStamina, Stamina + amount);
        EmitSignal(SignalName.StaminaChanged, Stamina, MaxStamina);
    }
}
