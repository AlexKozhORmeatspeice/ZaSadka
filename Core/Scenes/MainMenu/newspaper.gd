extends Node2D

@onready var sprite = $Newspaper
@onready var area = $Area2D
@onready var lore = $"../../../Lore piece"
@onready var root = $".."

func handle_LMB():
	self.visible = false
	lore.open()
	root.newspaper_going = false

func _ready() -> void:
	sprite.material.set_shader_parameter("enabled", false)
	sprite.material.set_shader_parameter("line_thickness", 10.)
func _on_area_2d_mouse_entered() -> void:
	sprite.material.set_shader_parameter("enabled", true)
func _on_area_2d_mouse_exited() -> void:
	sprite.material.set_shader_parameter("enabled", false)


func _on_area_2d_input_event(viewport: Node, event: InputEvent, shape_idx: int) -> void:
	if event is InputEventMouseButton:
		if event.button_index == MOUSE_BUTTON_LEFT and event.pressed:
			handle_LMB()
			print("Newspaper found")
