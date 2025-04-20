extends Node2D
const conv_speed = 0.5
const bag_chance = 0.3

func _on_play_button_pressed() -> void:
	get_tree().change_scene_to_file("res://Core/Scenes/GameScenes/Market.tscn")
	return


func _on_exit_button_pressed() -> void:
	get_tree().quit()
	return # Replace with function body.
