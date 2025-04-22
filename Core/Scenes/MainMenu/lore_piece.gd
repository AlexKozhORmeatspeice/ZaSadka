extends Node2D

@onready var np = $NP_opened
@onready var cr = $ColorRect

func open():
	cr.mouse_filter = 0
	visible = true
func close():
	cr.mouse_filter = 2
	visible = false

func _ready():
	np.position = Vector2(get_viewport_rect().size.x / 2, get_viewport_rect().size.y / 2)
	close()
	


func _on_button_pressed() -> void:
	close()
