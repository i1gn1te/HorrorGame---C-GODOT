using Godot;
using System.Collections.Generic;

public partial class RandomWallPatternApplier : Node
{
    // Ten skrypt losowo rozklada rozne warianty tapety na scianach.
    // Dzieki temu Level 0 nie wyglada jak jedna idealnie powtorzona tekstura.
    // Wystarczy dodac ten node do poziomu i ustawic TargetRootPath na node z geometra scian.

    [Export] public NodePath TargetRootPath = "../Blockout";
    [Export] public bool UseRandomSeed = true;
    [Export] public int FixedSeed = 1717;
    [Export] public bool ApplyToPillarsAndBlocks = true;

    private readonly string[] _defaultMaterialPaths =
    {
        "res://Materials/WallVariants/M_Level0_Wallpaper_01.tres",
        "res://Materials/WallVariants/M_Level0_Wallpaper_02.tres",
        "res://Materials/WallVariants/M_Level0_Wallpaper_03.tres",
        "res://Materials/WallVariants/M_Level0_Wallpaper_04.tres",
        "res://Materials/WallVariants/M_Level0_Wallpaper_05.tres",
        "res://Materials/WallVariants/M_Level0_Wallpaper_06.tres"
    };

    private readonly List<Material> _materials = new List<Material>();
    private RandomNumberGenerator _rng = new RandomNumberGenerator();

    public override void _Ready()
    {
        LoadMaterials();

        if (_materials.Count == 0)
        {
            GD.PushWarning("RandomWallPatternApplier: brak materialow tapety.");
            return;
        }

        if (UseRandomSeed)
        {
            _rng.Randomize();
        }
        else
        {
            _rng.Seed = (ulong)FixedSeed;
        }

        Node root = GetNodeOrNull<Node>(TargetRootPath);

        if (root == null)
        {
            GD.PushWarning("RandomWallPatternApplier: nie znaleziono TargetRootPath.");
            return;
        }

        ApplyRandomMaterials(root);
    }

    private void LoadMaterials()
    {
        _materials.Clear();

        foreach (string path in _defaultMaterialPaths)
        {
            if (!ResourceLoader.Exists(path))
            {
                continue;
            }

            Material material = GD.Load<Material>(path);

            if (material != null)
            {
                _materials.Add(material);
            }
        }
    }

    private void ApplyRandomMaterials(Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            ApplyRandomMaterials(child);
        }

        if (!ShouldApplyToNode(node))
        {
            return;
        }

        Material material = _materials[_rng.RandiRange(0, _materials.Count - 1)];

        if (node is CsgBox3D csgBox)
        {
            csgBox.Material = material;
        }
        else if (node is MeshInstance3D meshInstance)
        {
            meshInstance.SetSurfaceOverrideMaterial(0, material);
        }
    }

    private bool ShouldApplyToNode(Node node)
    {
        string nodeName = node.Name.ToString();

        if (nodeName.Contains("Wall"))
        {
            return true;
        }

        if (!ApplyToPillarsAndBlocks)
        {
            return false;
        }

        return nodeName.Contains("Pillar") || nodeName.Contains("Block") || nodeName.Contains("Nook");
    }
}
