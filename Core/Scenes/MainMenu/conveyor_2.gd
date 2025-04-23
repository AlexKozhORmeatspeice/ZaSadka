extends Node2D

const conv_speed = 200.
const bag_chance = 0.5

@onready var bag = $Bag
@onready var conv = $conv1
@onready var machine = $Money_Machine


func chance(x: float) -> bool:
	var f = randf()
	if f >= x:
		return true
	return false

func _ready():
	bag.position = Vector2(conv.position.x,  - bag.texture.get_height() / 2 - randf_range(10, 1200))
	bag.texture = load("res://Core/Scenes/MainMenu/MenuAnimations/Sprites/potato_bag.png")

func _process(delta):
	bag.position.y += conv_speed * delta 
	if bag.position.y - machine.position.y >= 1:
		bag.texture = load("res://Core/Scenes/MainMenu/MenuAnimations/Sprites/money_bag.png")
		machine.texture = load("res://Core/Scenes/MainMenu/MenuAnimations/Sprites/machine_enabled.png")
		await get_tree().create_timer(1).timeout
		machine.texture = load("res://Core/Scenes/MainMenu/MenuAnimations/Sprites/machine_disabled.png")
	if bag.position.y > get_viewport_rect().size.y + bag.texture.get_height() / 2:
			bag.position.y =  - bag.texture.get_height() / 2 - randf_range(10, 1200)	
			bag.texture = load("res://Core/Scenes/MainMenu/MenuAnimations/Sprites/potato_bag.png")		
