using Godot;
using System.Collections.Generic;

public partial class GameBootstrap : Node
{
    // Ten skrypt jest głównym menedżerem gry.
    // Tu trzymamy input, checkpoint, klucze, zmianę poziomów i krótkie komunikaty dla HUD.

    [Signal]
    public delegate void MessageShownEventHandler(string message, float seconds);

    [Signal]
    public delegate void KeyCountChangedEventHandler(int keyCount);

    [Export] public string LevelRootPath = "LevelRoot";
    [Export] public string PlayerPath = "Player";

    private readonly HashSet<string> _keys = new HashSet<string>();
    private Vector3 _checkpointPosition;
    private string _currentLevelPath = "res://Scenes/Levels/Level01_LiminalLobby.tscn";
    private Node3D _levelRoot;
    private Node3D _player;

    public override void _Ready()
    {
        AddToGroup("game_manager");
        DisplayServer.WindowSetTitle("Gra3D - Liminal Souls Prototype");

        _levelRoot = GetNodeOrNull<Node3D>(LevelRootPath);
        _player = GetNodeOrNull<Node3D>(PlayerPath);

        if (_player != null)
        {
            _checkpointPosition = _player.GlobalPosition;
        }

        AddKeyboardAction("move_forward", Key.W);
        AddKeyboardAction("move_back", Key.S);
        AddKeyboardAction("move_left", Key.A);
        AddKeyboardAction("move_right", Key.D);
        AddKeyboardAction("sprint", Key.Shift);
        AddKeyboardAction("dodge", Key.Space);
        AddKeyboardAction("lock_on", Key.Tab);
        AddKeyboardAction("interact", Key.E);
        AddKeyboardAction("pause", Key.Escape);

        AddMouseAction("attack_light", MouseButton.Left);
        AddMouseAction("attack_heavy", MouseButton.Right);

        // Na starcie chowamy kursor, żeby mysz sterowała kamerą.
        Input.MouseMode = Input.MouseModeEnum.Captured;

        ShowMessage("Obudziles sie w miejscu, ktore nie powinno istniec.", 4f);
        EmitSignal(SignalName.KeyCountChanged, _keys.Count);
    }

    public void ShowMessage(string message, float seconds = 3f)
    {
        // Wszystkie skrypty mogą wołać tę metodę, żeby pokazać tekst na HUD.
        GD.Print(message);
        EmitSignal(SignalName.MessageShown, message, seconds);
    }

    public void AddKey(string keyId, string displayName)
    {
        // Klucze trzymamy jako tekstowe identyfikatory, np. "level01_exit_key".
        // To proste i wygodne na początku projektu.
        if (_keys.Add(keyId))
        {
            ShowMessage($"Znaleziono: {displayName}.", 3f);
            EmitSignal(SignalName.KeyCountChanged, _keys.Count);
        }
    }

    public bool HasKey(string keyId)
    {
        return _keys.Contains(keyId);
    }

    public void SetCheckpoint(Vector3 position, string message = "Checkpoint zapisany.")
    {
        _checkpointPosition = position;
        ShowMessage(message, 2.5f);
    }

    public void RespawnPlayer()
    {
        // Respawn jest prosty: przenosimy gracza do ostatniego checkpointu i resetujemy statystyki.
        if (_player == null || !IsInstanceValid(_player))
        {
            _player = GetNodeOrNull<Node3D>(PlayerPath);
        }

        if (_player == null)
        {
            return;
        }

        _player.GlobalPosition = _checkpointPosition;

        PlayerStats stats = _player.GetNodeOrNull<PlayerStats>("PlayerStats");
        stats?.ResetStats();

        ShowMessage("Wracasz do ostatniego stabilnego miejsca.", 3f);
    }

    public void LoadLevel(string scenePath)
    {
        // Zmiana poziomu bez niszczenia gracza i HUD.
        // Main zostaje, wymieniamy tylko dziecko w LevelRoot.
        if (_levelRoot == null || !IsInstanceValid(_levelRoot))
        {
            _levelRoot = GetNodeOrNull<Node3D>(LevelRootPath);
        }

        if (_levelRoot == null || string.IsNullOrWhiteSpace(scenePath) || !ResourceLoader.Exists(scenePath))
        {
            ShowMessage("Nie moge wczytac nastepnego poziomu.", 3f);
            return;
        }

        foreach (Node child in _levelRoot.GetChildren())
        {
            _levelRoot.RemoveChild(child);
            child.QueueFree();
        }

        PackedScene nextLevelScene = GD.Load<PackedScene>(scenePath);
        Node nextLevel = nextLevelScene.Instantiate();
        _levelRoot.AddChild(nextLevel);
        _currentLevelPath = scenePath;

        MovePlayerToLevelSpawn(nextLevel);
        ShowMessage("Przestrzen przelamuje sie i uklada inaczej.", 4f);
    }

    public void ReloadCurrentLevel()
    {
        LoadLevel(_currentLevelPath);
    }

    private void MovePlayerToLevelSpawn(Node level)
    {
        if (_player == null || !IsInstanceValid(_player))
        {
            _player = GetNodeOrNull<Node3D>(PlayerPath);
        }

        if (_player == null)
        {
            return;
        }

        Node3D spawn = FindNodeByName<Node3D>(level, "PlayerSpawn");

        if (spawn != null)
        {
            _player.GlobalPosition = spawn.GlobalPosition;
            _checkpointPosition = spawn.GlobalPosition;
        }
    }

    private T FindNodeByName<T>(Node root, string nodeName) where T : Node
    {
        // Małe wyszukiwanie po drzewie sceny.
        // Używamy go do znalezienia PlayerSpawn w świeżo wczytanym poziomie.
        if (root.Name == nodeName && root is T typedRoot)
        {
            return typedRoot;
        }

        foreach (Node child in root.GetChildren())
        {
            T found = FindNodeByName<T>(child, nodeName);

            if (found != null)
            {
                return found;
            }
        }

        return null;
    }

    private void AddKeyboardAction(string actionName, Key key)
    {
        // Jeżeli akcja nie istnieje w projekcie, tworzymy ją z kodu.
        // Dzięki temu możesz uruchomić prototyp bez ręcznego ustawiania Input Map w edytorze.
        if (!InputMap.HasAction(actionName))
        {
            InputMap.AddAction(actionName);
        }

        InputEventKey keyEvent = new InputEventKey
        {
            PhysicalKeycode = key
        };

        if (!InputMap.ActionHasEvent(actionName, keyEvent))
        {
            InputMap.ActionAddEvent(actionName, keyEvent);
        }
    }

    private void AddMouseAction(string actionName, MouseButton button)
    {
        if (!InputMap.HasAction(actionName))
        {
            InputMap.AddAction(actionName);
        }

        InputEventMouseButton mouseEvent = new InputEventMouseButton
        {
            ButtonIndex = button
        };

        if (!InputMap.ActionHasEvent(actionName, mouseEvent))
        {
            InputMap.ActionAddEvent(actionName, mouseEvent);
        }
    }
}
