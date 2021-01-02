shader_type spatial;
render_mode blend_mix, depth_draw_opaque, cull_disabled, diffuse_burley, specular_schlick_ggx;
uniform float blinkSpeed = 15.0;
uniform float blinkRange = 1;
uniform float fadeInPercent;
uniform float dissolvePercent;
uniform vec4 blinkColor : hint_color = vec4(3.0, 0.05, 0.1, 1.0);
uniform vec4 fadeInColor : hint_color = vec4(2.4, 1.8, 0.7, 1.0);
uniform vec4 dissolveColor : hint_color = vec4(2.4, 1.8, 0.7, 1.0);
uniform vec4 dissolveEmissionColor : hint_color = vec4(0.1, 0.2, 0.9, 1.0);
uniform bool blink = false;
uniform bool fadeIn = false;
uniform bool dissolve = false;
uniform sampler2D dissolveTexture;


vec4 createBlinkEffect(vec2 base_uv, float time)
{
	return max(blinkColor * (blinkRange * cos(time * blinkSpeed)), 0.0);
}

vec4 createFadeInEffect(vec2 baseUV)
{
	return fadeInColor * (fadeInPercent / 100.0);
}

float createDissolveEffect(vec2 baseUV)
{
	// float time = (dissolvePercent / 100.0);
	float time = (dissolvePercent / 100.0) / 2.0;
	vec4 effectTexture = texture(dissolveTexture, baseUV);
	float effectDot = dot(effectTexture.rgb, vec3(0.3, 0.3, 0.3));
	return 0.3 + effectDot - time;
}

vec4 createEmissionEffect(float dissolveEffect)
{
	float effect = round(1.01 - dissolveEffect);
	return dissolveEmissionColor * effect;
}

void fragment()
{
	if(dissolve)
	{
		float dissolveEffect = createDissolveEffect(UV);
		ALBEDO = dissolveColor.rgb;
		ALPHA = round(dissolveEffect);
		EMISSION = createEmissionEffect(dissolveEffect).rgb;
	}
	else
	{
		vec4 albedo = blink ? createBlinkEffect(UV, TIME) :
				(fadeIn ? createFadeInEffect(UV) : vec4(0.0, 0.0, 0.0, 0.0));
		ALBEDO = albedo.rgb;
		ALPHA = albedo.a;
	}
}