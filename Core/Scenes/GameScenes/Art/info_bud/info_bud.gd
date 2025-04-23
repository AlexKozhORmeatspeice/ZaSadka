extends Control

@onready var rect = $Root/TextWind/ColorRect
@onready var mesh = $Root/MeshInstance2D
@onready var text = $Root/TextWind/RichTextLabel
@onready var info = $Root/TextWind/RichTextLabel2

@export var circle_color: Color
@export var heading_color: Color
@export var text_color: Color
@export var window_scale: Vector2 = Vector2(1., 1.)
@export var heading: String 
@export var line1: String
@export var line2: String
@export var line3: String
@export_range(0, 120, 1, "or_greater") var heading_size: int = 50
@export_range(0, 120, 1, "or_greater") var text_size: int = 40

func _ready():
	var cc = Vector4(circle_color.r, circle_color.g, circle_color.b, circle_color.a)
	var lines = line1 + "\n" + line2 + "\n" + line3
	info.text = lines
	
	mesh.material.set_shader_parameter("u_color", cc)
	rect.visible = false
	text.visible = false
	info.visible = false
	rect.scale = window_scale
	text.text = heading
	text.add_theme_font_size_override("normal_font_size", heading_size)
	text.add_theme_color_override("default_color", heading_color)
	info.add_theme_font_size_override("normal_font_size", text_size)


func _on_area_2d_mouse_entered() -> void:
	rect.visible = true
	text.visible = true
	info.visible = true


func _on_area_2d_mouse_exited() -> void:
	rect.visible = false
	text.visible = false
	info.visible = false
