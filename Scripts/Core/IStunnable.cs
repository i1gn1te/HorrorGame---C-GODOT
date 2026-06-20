public interface IStunnable
{
    // Ten interfejs oznacza przeciwnika, ktorego mozna zatrzymac FlashBurst.
    // Dzieki temu kazdy enemy moze miec inne AI, ale reaguje na blysk w podobny sposob.
    void Stun(float seconds);
}
