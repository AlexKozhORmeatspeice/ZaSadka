extends Sprite2D
#consts 
const conv_speed = 200.
const bag_chance = 0.3

@onready var conv2 = $"../conv2"


func _ready():
	position = Vector2(get_viewport_rect().size.x * 0.6,  -(float)(texture.get_height()) / 2.0)
	conv2.position = Vector2(get_viewport_rect().size.x * 0.6,  (float)(texture.get_height()) / 2.0)
	
#fuck yeah simulator
func _process(delta) -> void:
	position.y += conv_speed * delta
	
	if conv2.position.y >  get_viewport_rect().size.y + (float)(texture.get_height()) / 2.0:
		conv2.position = Vector2(get_viewport_rect().size.x * 0.6,  -(float)(texture.get_height()) / 2.0)
		
	if position.y > get_viewport_rect().size.y + texture.get_height() / 2:
		position.y = -(float)(texture.get_height()) / 2.0
