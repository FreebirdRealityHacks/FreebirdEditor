Shader "DepthMask3"
{
  SubShader {
    Tags {"Queue" = "Geometry-1" }
    Pass {
      ColorMask 0
      ZWrite On
      Cull Off
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #include "UnityCG.cginc"
 
      float4 vert (float4 vertex : POSITION) : SV_POSITION { return UnityObjectToClipPos(vertex); }
      void frag() {}
      ENDCG
    }
  }
}