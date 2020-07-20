#version 450

// IN
layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 Uv;
layout(location = 2) in vec3 Normal;

// IN UNIFORM
layout(set = 0, binding = 1) uniform TransformationBuffer
{
    mat4 Transformation;
};

layout(set = 0, binding = 2) uniform ProjectionBuffer
{
    mat4 Projection;
};

layout(set = 0, binding = 3) uniform ViewBuffer
{
    mat4 View;
};

// OUT
layout(location = 0) out vec2 fsin_Uv;
layout(location = 1) out vec3 fsin_SurfaceNormal;
layout(location = 2) out vec3 fsin_ToCamera;

void main()
{
    vec4 worldPosition = Transformation * vec4(Position, 1);
    gl_Position = Projection * View * worldPosition;
    fsin_Uv = Uv;
    fsin_SurfaceNormal = (Transformation * vec4(Normal, 0)).xyz;
    fsin_ToCamera = (inverse(View) * vec4(0,0,0,1)).xyz - worldPosition.xyz;
}
