#ifndef __INCLUDE_LUT__
#define __INCLUDE_LUT__

#define DIM 16.0
static const float4 _ColorGrading_Scale = float4((DIM - 1) / (DIM * DIM), (DIM - 1) / DIM, 1 / DIM, 0);
static const float4 _ColorGrading_Offset = float4(1 / (2 * DIM * DIM), 1 / (2 * DIM), 0, 0);

float2 ColorGrade_UV(float3 c) {
    return float2(dot(c.xz, _ColorGrading_Scale.xz), (1 - c.y) * _ColorGrading_Scale.y) + _ColorGrading_Offset.xy;
}
float3 ColorGrade(sampler2D lut, float3 c) {
    return tex2D(lut, ColorGrade_UV(c.rgb)).rgb;
}
float4 ColorGrade(sampler2D lut, float4 c) {
    return float4(ColorGrade(lut, c.rgb), c.a);
}

#endif
