using Godot;

public partial class StandaloneScenePreview : Node3D
{
    [Export] public string PlayerScenePath = "res://Scenes/Player/Player.tscn";
    [Export] public string SpawnNodeName = "PlayerSpawn";

    public override void _Ready()
    {
        CallDeferred(MethodName.EnablePreviewIfStandalone);
    }

    public void EnablePreviewIfStandalone()
    {
        if (GetTree().GetFirstNodeInGroup("game_manager") != null ||
            GetTree().GetFirstNodeInGroup("player") != null)
        {
            return;
        }

        AddPreviewInputActions();

        PackedScene playerScene = GD.Load<PackedScene>(PlayerScenePath);
        if (playerScene == null)
        {
            GD.PushError($"Standalone preview cannot load player scene: {PlayerScenePath}");
            return;
        }

        Node3D player = playerScene.Instantiate<Node3D>();
        AddChild(player);

        Node3D spawn = FindNodeByName<Node3D>(this, SpawnNodeName);
        if (spawn != null)
        {
            player.GlobalTransform = spawn.GlobalTransform;
        }
    }

    private void AddPreviewInputActions()
    {
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
    }

    private void AddKeyboardAction(string actionName, Key key)
    {
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

    private T FindNodeByName<T>(Node root, string nodeName) where T : Node
    {
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
}
