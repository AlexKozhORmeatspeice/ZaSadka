shader_type canvas_item;

uniform vec3 maincol: source_color = vec3(0.106, 0.165, 0.247);
uniform vec3 dopcol: source_color = vec3(0.21, 0.196, 0.327);

void fragment() {
	vec4 col = texture(TEXTURE, UV);
	float factor = UV.x * sin(UV.y + TIME) + UV.y * sin(UV.x + TIME);
	col.rgb = maincol * factor + dopcol * (1. - factor);
	COLOR = col;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
