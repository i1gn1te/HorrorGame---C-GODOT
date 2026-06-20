using Godot;

public partial class GameHud : CanvasLayer
{
    // Prosty HUD tworzony z kodu.
    // Na razie pokazuje zdrowie i staminę gracza, czyli podstawę pod horror akcji.

    private ProgressBar _healthBar;
    private ProgressBar _staminaBar;
    private ProgressBar _fearBar;
    private Label _hintLabel;
    private Label _messageLabel;
    private Label _keyLabel;
    private PlayerStats _playerStats;
    private PlayerFear _playerFear;
    private GameBootstrap _gameManager;
    private float _messageTimer;

    public override void _Ready()
    {
        BuildHud();

        // Czekamy jedną klatkę, bo gracz może być tworzony razem z tą sceną.
        CallDeferred(MethodName.FindPlayerStats);
        CallDeferred(MethodName.FindGameManager);
    }

    public override void _Process(double delta)
    {
        // Komunikaty znikają same po kilku sekundach.
        if (_messageTimer <= 0f)
        {
            return;
        }

        _messageTimer -= (float)delta;

        if (_messageTimer <= 0f)
        {
            _messageLabel.Text = "";
        }
    }

    private void BuildHud()
    {
        MarginContainer margin = new MarginContainer();
        margin.SetAnchorsPreset(Control.LayoutPreset.TopLeft);
        margin.OffsetLeft = 18;
        margin.OffsetTop = 18;
        margin.OffsetRight = 380;
        margin.OffsetBottom = 150;
        AddChild(margin);

        VBoxContainer box = new VBoxContainer();
        box.AddThemeConstantOverride("separation", 8);
        margin.AddChild(box);

        Label title = new Label();
        title.Text = "PROTOTYP: LIMINAL SOULS";
        box.AddChild(title);

        _healthBar = CreateBar("Zdrowie");
        box.AddChild(_healthBar);

        _staminaBar = CreateBar("Stamina");
        box.AddChild(_staminaBar);

        _fearBar = CreateBar("Strach");
        box.AddChild(_fearBar);

        _hintLabel = new Label();
        _hintLabel.Text = "WASD ruch | Mysz rozgladanie | Shift sprint | Spacja unik | LPM reakcja | Esc kursor";
        box.AddChild(_hintLabel);

        _keyLabel = new Label();
        _keyLabel.Text = "Klucze: 0";
        box.AddChild(_keyLabel);

        _messageLabel = new Label();
        _messageLabel.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        _messageLabel.CustomMinimumSize = new Vector2(520, 44);
        box.AddChild(_messageLabel);
    }

    private ProgressBar CreateBar(string labelText)
    {
        ProgressBar bar = new ProgressBar();
        bar.MinValue = 0;
        bar.MaxValue = 100;
        bar.Value = 100;
        bar.ShowPercentage = false;
        bar.CustomMinimumSize = new Vector2(320, 20);
        bar.TooltipText = labelText;
        return bar;
    }

    private void FindPlayerStats()
    {
        Node player = GetTree().GetFirstNodeInGroup("player");

        if (player == null)
        {
            _hintLabel.Text = "Brak gracza w scenie. Sprawdź grupę: player.";
            return;
        }

        _playerStats = player.GetNodeOrNull<PlayerStats>("PlayerStats");

        if (_playerStats == null)
        {
            _hintLabel.Text = "Gracz nie ma noda PlayerStats.";
            return;
        }

        _playerStats.HealthChanged += OnHealthChanged;
        _playerStats.StaminaChanged += OnStaminaChanged;

        OnHealthChanged(_playerStats.Health, _playerStats.MaxHealth);
        OnStaminaChanged(_playerStats.Stamina, _playerStats.MaxStamina);

        _playerFear = player.GetNodeOrNull<PlayerFear>("PlayerFear");

        if (_playerFear != null)
        {
            _playerFear.FearChanged += OnFearChanged;
            OnFearChanged(_playerFear.Fear, _playerFear.MaxFear);
        }
    }

    private void FindGameManager()
    {
        _gameManager = GetTree().GetFirstNodeInGroup("game_manager") as GameBootstrap;

        if (_gameManager == null)
        {
            return;
        }

        _gameManager.MessageShown += OnMessageShown;
        _gameManager.KeyCountChanged += OnKeyCountChanged;
    }

    private void OnHealthChanged(float current, float maximum)
    {
        _healthBar.MaxValue = maximum;
        _healthBar.Value = current;
    }

    private void OnStaminaChanged(float current, float maximum)
    {
        _staminaBar.MaxValue = maximum;
        _staminaBar.Value = current;
    }

    private void OnFearChanged(float current, float maximum)
    {
        _fearBar.MaxValue = maximum;
        _fearBar.Value = current;
    }

    private void OnMessageShown(string message, float seconds)
    {
        _messageLabel.Text = message;
        _messageTimer = seconds;
    }

    private void OnKeyCountChanged(int keyCount)
    {
        _keyLabel.Text = $"Klucze: {keyCount}";
    }
}
