extends SceneTree

var level_paths := [
	"res://Scenes/Levels/Level01_LiminalLobby.tscn",
	"res://Scenes/Levels/Level02_HabitableZone.tscn",
	"res://Scenes/Levels/Level03_UtilityHalls.tscn",
	"res://Scenes/Levels/Level04_ElectricalStation.tscn",
	"res://Scenes/Levels/Level05_AbandonedOffice.tscn",
]

func _initialize() -> void:
	call_deferred("_run")

func _run() -> void:
	var failures := 0

	for path in level_paths:
		var packed_scene := load(path)
		if packed_scene == null:
			push_error("Missing scene: %s" % path)
			failures += 1
			continue

		var level = packed_scene.instantiate()
		root.add_child(level)
		await process_frame

		if level.find_child("PlayerSpawn", true, false) == null:
			push_error("Missing PlayerSpawn in: %s" % path)
			failures += 1

		if level.find_child("ExitTrigger", true, false) == null:
			push_error("Missing ExitTrigger in: %s" % path)
			failures += 1

		level.queue_free()
		await process_frame

	if failures == 0:
		print("RouteSanityTest OK")

	quit(failures)
