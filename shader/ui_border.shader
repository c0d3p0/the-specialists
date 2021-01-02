shader_type canvas_item;
render_mode blend_mix;

uniform sampler2D ui_texture : hint_black_albedo;
uniform vec4 color_1 : hint_color = vec4(0.72, 0.89, 2.0, 1.0);
uniform vec4 color_2 : hint_color = vec4(2.0, 2.0, 2.0, 1.0);
uniform vec4 color_3 : hint_color = vec4(1.0, 1.0, 1.0, 0.0);
uniform vec2 resolution = vec2(1.0, 1.0);


vec4 createEffect(vec2 base_uv, float time)
{
	vec2 uv = base_uv.xy / resolution.xy;
    float t = abs(cos(time));
    vec4 effect;

    if (uv.x > t)
		effect = color_2;
    else
		effect = color_1;
    
    if (uv.x > t + 0.01)
     	effect = color_3;
	
	return effect * texture(ui_texture, base_uv);
}

void fragment()
{
	COLOR = createEffect(UV, TIME);
}