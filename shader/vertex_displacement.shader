shader_type spatial;
render_mode blend_mix,depth_draw_always,cull_disabled,diffuse_burley,specular_schlick_ggx;
uniform vec4 albedo : hint_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform float specular = 0.5;
uniform float metallic = 0;
uniform float roughness : hint_range(0,1) = 1;
uniform float point_size : hint_range(0,128) = 1;
uniform float normal_scale : hint_range(-16,16) = 1;
uniform float rim : hint_range(0,1) = 1;
uniform float rim_tint : hint_range(0,1) = 1;
uniform float ao_light_affect = 1;
uniform vec4 metallic_texture_channel = vec4(1.0, 0.0, 0.0, 0.0);
uniform vec4 roughness_texture_channel = vec4(1.0, 0.0, 0.0, 0.0);
uniform vec4 ao_texture_channel = vec4(1.0, 0.0, 0.0, 0.0);
uniform vec3 uv1_scale = vec3(1.0, 1.0, 1.0);
uniform vec3 uv1_offset = vec3(0.0, 0.0, 0.0);
uniform vec3 displacement = vec3(0.0, 1.0, 0.0);
uniform sampler2D texture_albedo : hint_albedo;
uniform sampler2D texture_metallic : hint_white;
uniform sampler2D texture_roughness : hint_white;
uniform sampler2D texture_normal : hint_normal;
uniform sampler2D texture_rim : hint_white;
uniform sampler2D texture_ambient_occlusion : hint_white;
uniform sampler2D texture_displacement : hint_white;


void vertex()
{
	vec4 displacement_tex = texture(texture_displacement, UV.xy);
	// vec4 vertex = vec4(displacement.xyz * displacement_tex.r / uv1_scale, 1.0);
	// vec2 xz = SRC_VERTEX.xz;
	// VERTEX += (MODELVIEW_MATRIX * vertex).xyz;
	VERTEX += displacement.xyz * displacement_tex.r / uv1_scale;
	UV = UV * uv1_scale.xy + uv1_offset.xy;
}

void fragment()
{
	vec2 base_uv = UV;
	ALBEDO = albedo.rgb * texture(texture_albedo, base_uv).rgb;
	ALPHA = albedo.a;
	
	METALLIC = dot(texture(texture_metallic, base_uv),
			metallic_texture_channel) * metallic;
	SPECULAR = specular;
	
	NORMALMAP = texture(texture_normal,base_uv).rgb;
	NORMALMAP_DEPTH = normal_scale;

	ROUGHNESS = dot(texture(texture_roughness, base_uv),
			roughness_texture_channel) * roughness;

	// Rim
	vec2 rim_tex = texture(texture_rim,base_uv).xy;
	RIM = rim * rim_tex.x;
	RIM_TINT = rim_tint * rim_tex.y;
	
	// Ambient Occlusion
	AO = dot(texture(texture_ambient_occlusion, base_uv), ao_texture_channel);
	AO_LIGHT_AFFECT = ao_light_affect;
}
