extends Node2D

@onready var rect = $ColorRect
@onready var text = $RichTextLabel
@onready var info = $RichTextLabel2

func _on_area_2d_mouse_entered() -> void:
	rect.visible = true
	text.visible = true
	info.visible = true


func _on_area_2d_mouse_exited() -> void:
	rect.visible = false
	text.visible = false
	info.visible = false
