extends Node2D

const conv_speed = 200.
const newspaper_chance = 0.25
var newspaper_going: bool = true

@onready var bag = $Bag
@onready var conv = $conv1
@onready var machine = $Money_Machine
@onready var newspaper = $NewspaperNode

func chance(threshold: float, value: float) -> bool:
	return value <= threshold

func _ready():
	bag.position = Vector2(conv.position.x,  - bag.texture.get_height() / 2 - randf_range(150, 1200))
	bag.texture = load("res://Core/Scenes/MainMenu/MenuAnimations/Sprites/potato_bag.png")
	newspaper.position = Vector2(conv.position.x, -100)

func _process(delta):
	bag.position.y += conv_speed * delta 
	if newspaper_going:
		newspaper.position.y += conv_speed * delta
		if newspaper.position.y > get_viewport_rect().size.y + 200:
			newspaper.position.y = -100
	if bag.position.y - machine.position.y >= 10:
		bag.texture = load("res://Core/Scenes/MainMenu/MenuAnimations/Sprites/money_bag.png")
		machine.texture = load("res://Core/Scenes/MainMenu/MenuAnimations/Sprites/machine_enabled.png")
		await get_tree().create_timer(1).timeout
		machine.texture = load("res://Core/Scenes/MainMenu/MenuAnimations/Sprites/machine_disabled.png")
	if bag.position.y > get_viewport_rect().size.y + bag.texture.get_height() / 2:
			bag.position.y =  - bag.texture.get_height() / 2 - randf_range(150, 1200)	
			bag.texture = load("res://Core/Scenes/MainMenu/MenuAnimations/Sprites/potato_bag.png")	
			if chance(newspaper_chance, randf()) and newspaper_going == false:
				newspaper_going = true
