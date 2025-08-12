void WorldPosFromEyeDepth_float(float2 UV, float EyeDepth, out float3 WorldPos)
{
    // NDC xy and clip at z=1
    float2 ndc = UV * 2.0 - 1.0;
    float4 clip = float4(ndc, 1.0, 1.0);

    // view ray (inverse projection)
    float4 viewH = mul(UNITY_MATRIX_I_P, clip);
    float3 viewPos = viewH.xyz / max(viewH.w, 1e-6);

    // march along the ray by EyeDepth
    viewPos *= EyeDepth;

    // view -> world
    float4 worldH = mul(UNITY_MATRIX_I_V, float4(viewPos, 1.0));
    WorldPos = worldH.xyz;
}
