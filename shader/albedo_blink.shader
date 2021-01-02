shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_burley, specular_schlick_ggx;
// uniform float alphaScissors = 0.8;
uniform float speed = 15.0;
uniform float range = 1;
uniform vec4 blinkColor : hint_color = vec4(3.0, 0.05, 0.1, 1.0);
uniform bool blink = false;
uniform vec3 uv1_scale = vec3(1.0, 1.0, 1.0);
uniform vec3 uv1_offset;
uniform sampler2D blinkTexture;


vec4 createBlinkEffect(vec4 tex, float time)
{
	return max(tex * (range * cos(time * speed)), 0.0) * blinkColor;
}

void vertex()
{
	UV = UV * uv1_scale.xy + uv1_offset.xy;
}

void fragment()
{
	vec4 tex = texture(blinkTexture, UV);
	vec4 albedo = blink ? createBlinkEffect(tex, TIME) : vec4(0.0);
	ALBEDO = albedo.rgb;
	ALPHA = min((albedo.r + albedo.g + albedo.b) / 3.0, 1.0);
	// ALPHA_SCISSOR = alphaScissors;
}