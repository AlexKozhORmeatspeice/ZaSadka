shader_type canvas_item;

uniform vec4 u_color = vec4(0.);


void fragment() {
	COLOR = u_color;
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
