// Modified version of https://www.shadertoy.com/view/tddXWH

shader_type canvas_item;
render_mode unshaded;

uniform vec2 resolution = vec2(1.0, 1.0);
uniform vec4 effectColor : hint_color = vec4(0.4, 0.8, 1.0, 1.0);
uniform float width = 0.6;

const float PI = 3.14159265359;


vec2 rotate(vec2 p, float a)
{
	return vec2(p.x * cos(a) - p.y * sin(a), p.x * sin(a) + p.y * cos(a));
}

float rand(float n)
{
    return fract(sin(n) * 43758.5453123);
}
float randV2(vec2 n)
{
    return fract(sin(dot(n, vec2(591.32,391.32))));
}
float randV3(vec3 n)
{
    return fract(sin(dot(n, vec3(591.32,391.32,623.54))));
}

vec2 randV2In(in vec2 p)
{
	return fract(vec2(sin(p.x * 591.32 + p.y * 154.077), cos(p.x * 391.32 + p.y * 49.077)));
}

const float voronoiRandK = 0.8;

vec3 voronoi3(in vec2 x, out vec4 cellCenters)
{
	vec2 p = floor(x);
	vec2 f = fract(x);

	vec2 i1 = vec2(0.0);
	vec2 i2 = vec2(0.0);
	vec3 res = vec3(8.0);

	for(int j = -1; j <= 1; j ++)
	{
		for(int i = -1; i <= 1; i ++)
		{
			vec2 b = vec2(float(i), float(j));
			vec2 r = vec2(b) - f + randV2In(p + b) * voronoiRandK;

			//float d = max(abs(r.x), abs(r.y));
			float d = (abs(r.x) + abs(r.y));

			if (d < res.x)
			{
				res.z = res.y;
				res.y = res.x;
				res.x = d;
				i2 = i1;
				i1 = p + b;
			}
			else if (d < res.y)
			{
				res.z = res.y;
				res.y = d;
				//r2 = r;
				i2 = p + b;
			}
			else if (d < res.z)
			{
				res.z = d;
			}
		}
	}

	cellCenters = vec4(i1,i2);
	return res;
}

float cubicPulse( float c, float w, float x )
{
	x = abs(x - c);

	if(x > w)
		return 0.0;
		
	x /= w;
	return 1.0 - x * x * (3.0 - 2.0 * x);
}

float orderedRand(float x, float y)
{
	return rand(dot(x > y ? vec2(y,x) : vec2(x,y), vec2(123.23,532.12)));
}

vec4 createEffect(vec2 baseUV, float time)
{
	vec2 uv = (baseUV / resolution.xy - 0.5) * 2.0;
	vec2 suv = uv;
	uv.x *= resolution.x / resolution.y;

	uv = rotate(uv, sin(time * 0.10));
	uv.x += time * 0.3;

	//  first wire
	float scale = 4.0;
//	float width = 0.6;
	vec4 cellCenters;
	vec3 vr = voronoi3(uv * scale + 10.0, cellCenters);
	float d = vr.y - vr.x;
	
	//connections between cell corners
	if(vr.z - vr.y < width && vr.y - vr.x < width)
		d = max(width - (vr.z - vr.y), d);

	vec2 cellHashes = vec2(randV2(cellCenters.xy), randV2(cellCenters.zw));
	float wire = cubicPulse(width, 0.06, d);

	//  light
	float lightX = (rotate(uv, PI / 8.0).x + time * 0.5) * 5.0;
	float lightHash1 = rand(floor(lightX));
	float lightValue1 = fract(lightX);
	lightX = (rotate(uv, PI * 5.0 / 8.0).x + time * 0.2) * 5.0;
	float lightHash2 = rand(floor(lightX) + 0.5);
	float lightValue2 = fract(lightX);
	lightX = (rotate(uv, PI * 9.0 / 8.0).x + time * 0.2) * 5.0;
	float lightHash3 = rand(floor(lightX) + 0.5);
	float lightValue3 = fract(lightX);
	lightX = (rotate(uv, PI * 13.0 / 8.0).x + time * 0.2) * 5.0;
	float lightHash4 = rand(floor(lightX) + 0.5);
	float lightValue4 = fract(lightX);
	float light = 0.0;
	float lightFrequency = 0.002;

	if(randV3(vec3(cellHashes.xy,lightHash1)) < lightFrequency)
		light = wire * cubicPulse(0.5, 0.25, lightValue1) * 3.0;

	if(randV3(vec3(cellHashes.xy,lightHash2)) < lightFrequency)
		light += wire * cubicPulse(0.5, 0.25, lightValue2) * 3.0;

	if(randV3(vec3(cellHashes.xy,lightHash3)) < lightFrequency)
		light += wire * cubicPulse(0.5, 0.25, lightValue3) * 3.0;

	if(randV3(vec3(cellHashes.xy, lightHash4)) < lightFrequency)
		light += wire * cubicPulse(0.5, 0.25, lightValue4) * 3.0;

	//  second parallel wire
	if((cellHashes.x - cellHashes.y) > 0.0)
	{
		float w = cubicPulse(width - 0.1, 0.06, d);
		wire += w;
	}

	//  background wire layer
	scale *= 0.4;
	vec3 vr2 = voronoi3(uv * scale + 30.0, cellCenters);
	d = vr2.y - vr2.x;

	//connections between cell corners
	if(vr2.z - vr2.y < width && vr2.y - vr2.x < width)
		d = max(width - (vr2.z - vr2.y), d);

	cellHashes = vec2(randV2(cellCenters.xy), randV2(cellCenters.zw));
	float backWire = cubicPulse(width, 0.06, d);

	if((cellHashes.x - cellHashes.y) > 0.0)
	{
		float w = cubicPulse(width-0.1, 0.06, d);
		backWire += w;
	}

	wire = max(wire, backWire * 0.3);

	//  some background noise
	wire += vr.x * 0.3 + 0.3;

	//  apply light
	wire = wire * 0.4 + light;
	vec3 col = clamp(effectColor.rgb * wire, vec3(0.0), vec3(1.0));
	col *= 0.7;

	return vec4(col, 1.0);
}

void fragment()
{
	COLOR = createEffect(UV, TIME);
}
