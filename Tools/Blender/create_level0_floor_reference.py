import bpy
from pathlib import Path

# Uruchom ten skrypt w Blenderze, jesli chcesz automatycznie stworzyc
# referencyjna podloge Level 0. Tutorial reczny jest w:
# Docs/BLENDER_LEVEL0_FLOOR_FROM_ZERO.md

PROJECT_ROOT = Path(__file__).resolve().parents[2]
TEXTURE_DIR = PROJECT_ROOT / "Assets" / "Graphics" / "Level0"


def clear_scene():
    bpy.ops.object.select_all(action="SELECT")
    bpy.ops.object.delete()


def create_floor():
    bpy.ops.mesh.primitive_plane_add(size=1, location=(0, 0, 0))
    floor = bpy.context.object
    floor.name = "level0_floor_real_carpet"
    floor.dimensions = (18, 26, 0)
    bpy.ops.object.transform_apply(location=False, rotation=False, scale=True)
    return floor


def make_material():
    material = bpy.data.materials.new("M_Level0_RealCarpet")
    material.use_nodes = True

    nodes = material.node_tree.nodes
    links = material.node_tree.links
    principled = nodes.get("Principled BSDF")

    albedo = nodes.new("ShaderNodeTexImage")
    albedo.name = "level0_real_carpet_albedo"
    albedo.image = bpy.data.images.load(str(TEXTURE_DIR / "level0_real_carpet_albedo.png"))
    links.new(albedo.outputs["Color"], principled.inputs["Base Color"])

    roughness = nodes.new("ShaderNodeTexImage")
    roughness.name = "level0_real_carpet_roughness"
    roughness.image = bpy.data.images.load(str(TEXTURE_DIR / "level0_real_carpet_roughness.png"))
    roughness.image.colorspace_settings.name = "Non-Color"
    links.new(roughness.outputs["Color"], principled.inputs["Roughness"])

    normal_image = nodes.new("ShaderNodeTexImage")
    normal_image.name = "level0_real_carpet_normal"
    normal_image.image = bpy.data.images.load(str(TEXTURE_DIR / "level0_real_carpet_normal.png"))
    normal_image.image.colorspace_settings.name = "Non-Color"

    normal_map = nodes.new("ShaderNodeNormalMap")
    normal_map.inputs["Strength"].default_value = 0.45
    links.new(normal_image.outputs["Color"], normal_map.inputs["Color"])
    links.new(normal_map.outputs["Normal"], principled.inputs["Normal"])

    return material


def main():
    clear_scene()
    floor = create_floor()
    floor.data.materials.append(make_material())

    blender_dir = PROJECT_ROOT / "Models" / "Level0" / "blender"
    blender_dir.mkdir(parents=True, exist_ok=True)
    bpy.ops.wm.save_as_mainfile(filepath=str(blender_dir / "level0_floor_real_carpet.blend"))


if __name__ == "__main__":
    main()
