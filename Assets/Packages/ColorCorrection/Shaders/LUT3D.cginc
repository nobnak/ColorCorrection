#ifndef __INCLUDE_LUT3D__
#define __INCLUDE_LUT3D__

float _ColorGrading_Scale;
float _ColorGrading_Offset;
sampler3D _ColorGrading_Lut3D;

float3 ColorGrade3D(float3 c) {
    return tex3D(_ColorGrading_Lut3D, c.rgb * _ColorGrading_Scale + _ColorGrading_Offset).rgb;
}
float4 ColorGrade3D(float4 c) {
    return float4(ColorGrade3D(c.rgb), c.a);
}

#endif
