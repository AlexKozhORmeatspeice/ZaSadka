shader_type canvas_item;
instance uniform bool enabled = true;

void vertex() {
}

void fragment() {
	if (!enabled)
	{
		COLOR.rgb = vec3((COLOR.r + COLOR.g + COLOR.b)/3. * .7);
	}
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
