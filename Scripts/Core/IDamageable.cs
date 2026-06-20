using Godot;

public interface IDamageable
{
    // Każdy obiekt, który może dostać obrażenia, powinien mieć tę metodę.
    // Dzięki temu gracz może bić przeciwników, a później przeciwnicy będą mogli bić gracza.
    void TakeDamage(float amount, Node3D source);
}
