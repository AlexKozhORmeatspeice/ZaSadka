extends Sprite2D
#consts 
const conv_speed = 200.
const bag_chance = 0.3

@onready var conv2 = $"../conv2"


func _ready():
	position = Vector2(get_viewport_rect().size.x * 0.6,  -texture.get_height() / 2)
	conv2.position = Vector2(position.x,  texture.get_height() / 2)

	
#fuck yeah simulator
func _process(delta) -> void:
	position.y += conv_speed * delta
	conv2.position.y += conv_speed * delta
	if conv2.position.y >  get_viewport_rect().size.y + texture.get_height() / 2:
		conv2.position = Vector2(conv2.position.x,  -texture.get_height() / 2)

		
	if position.y > get_viewport_rect().size.y + texture.get_height() / 2:
		position.y = -(float)(texture.get_height()) / 2.0
