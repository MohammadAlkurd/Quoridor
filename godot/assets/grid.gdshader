shader_type canvas_item;
uniform float a :hint_range(0.1, 100.0, 0.1);
uniform float b :hint_range(0.1, 1.0, 0.1);
uniform float c :hint_range(0.1, 100.0, 0.1);
uniform float d :hint_range(0.1, 100.0, 0.1);
uniform float e :hint_range(0.1, 1.0, 0.05);
uniform float f :hint_range(0.1, 1.0, 0.05);
void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float g = UV.x +e;
	float h = UV.y +f;
	COLOR = COLOR + sin(g*a*1.0/cos(g*b)*cos(g*h*a*b) * 1.0/sin(g*h)* cos(h * d))*1.0/c;
	COLOR = COLOR - sin(h*a*1.0/cos(h*b)*cos(g*h*a*b) *1.0/sin(g*h)* cos(g * d))*1.0/c;
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
