extends SceneTree

var enemy_paths := [
	"res://Scenes/Enemies/WatcherMannequinEnemy.tscn",
	"res://Scenes/Enemies/PipeRunnerEnemy.tscn",
	"res://Scenes/Enemies/OfficeMimicEnemy.tscn",
	"res://Scenes/Enemies/CornerStalkerEnemy.tscn",
	"res://Scenes/Enemies/EnemyRosterShowcase.tscn",
]

func _initialize() -> void:
	call_deferred("_run")

func _run() -> void:
	var failures := 0

	for path in enemy_paths:
		var packed_scene := load(path)
		if packed_scene == null:
			push_error("Missing enemy scene: %s" % path)
			failures += 1
			continue

		var enemy = packed_scene.instantiate()
		root.add_child(enemy)
		await process_frame

		if enemy.find_child("Body", true, false) == null and not path.ends_with("EnemyRosterShowcase.tscn"):
			push_error("Missing Body mesh in: %s" % path)
			failures += 1

		enemy.queue_free()
		await process_frame

	if failures == 0:
		print("EnemySceneSanityTest OK")

	quit(failures)
