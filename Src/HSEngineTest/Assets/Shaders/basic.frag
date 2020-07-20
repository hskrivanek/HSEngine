#version 450

// IN
layout(location = 0) in vec2 fsin_Uv;
layout(location = 1) in vec3 fsin_SurfaceNormal;
layout(location = 2) in vec3 fsin_ToCamera;

// IN UNIFORM
layout(set = 1, binding = 1) uniform texture2D Texture;
layout(set = 1, binding = 2) uniform sampler Sampler;
layout(set = 1, binding = 3) uniform LightDirectionBuffer
{
    vec3 LightDirection;
};
layout(set = 1, binding = 4) uniform LightColorBuffer
{
    vec3 LightColor;
};
layout(set = 1, binding = 5) uniform ShineDamperBuffer
{
    float ShineDamper;
};
layout(set = 1, binding = 6) uniform ReflectivityBuffer
{
    float Reflectivity;
};

// OUT
layout(location = 0) out vec4 fsout_Color;

void main()
{
    vec3 normalUnit = normalize(fsin_SurfaceNormal);
    vec3 lightDirectionUnit = normalize(LightDirection);

    // diffuse
    float rawBrightness = dot(normalUnit, -lightDirectionUnit);
    float brightness = max(rawBrightness, 0.15f);
    vec3 diffuse = brightness * LightColor;

    // specular
    vec3 reflectionUnit = normalize(reflect(lightDirectionUnit, normalUnit));
    vec3 toCameraUnit = normalize(fsin_ToCamera);
    float specularFactor = max(dot(reflectionUnit, toCameraUnit),0.0f);
    float dampedFactor = pow(specularFactor, ShineDamper);
    vec3 specular = dampedFactor * Reflectivity * LightColor;

    fsout_Color = vec4(diffuse, 1) * texture(sampler2D(Texture, Sampler), fsin_Uv) + vec4(specular, 1);
}
