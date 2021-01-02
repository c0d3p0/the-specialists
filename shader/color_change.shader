shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_burley,specular_schlick_ggx,unshaded;
uniform vec4 color1 : hint_color = vec4(2.0, 0.0, 0.0, 1.0);
uniform vec4 color2 : hint_color = vec4(2.0, 2.0, 0.0, 1.0);
uniform vec4 color3 : hint_color = vec4(0.0, 2.0, 0.0, 1.0);
uniform vec4 color4 : hint_color = vec4(0.0, 2.0, 2.0, 1.0);
uniform vec4 color5 : hint_color = vec4(0.0, 0.0, 2.0, 1.0);
uniform vec4 color6 : hint_color = vec4(2.0, 0.0, 2.0, 1.0);
uniform float alpha_override : hint_range(0.0, 1.0) = 1.0;
uniform float duration = 0.24;

float getAlpha(float alpha)
{
	return alpha_override * alpha;
}

vec4 createEffect(float time)
{
	int colorIndex = int(mod(round(fract(time) / (duration / 6.0)), 6));
	
	if(colorIndex == 0)
		return color1;
	else if(colorIndex == 1)
		return color2;
	else if(colorIndex == 2)
		return color3;
	else if(colorIndex == 3)
		return color4;
	else if(colorIndex == 4)
		return color5;
	else if(colorIndex == 5)
		return color6;
}

void fragment()
{
	vec4 effect = createEffect(TIME);
	float alpha = getAlpha(effect.a);
	ALBEDO = effect.rgb;
	ALPHA = alpha < 0.0 ? 0.0 : alpha;
}
