// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/GroundTexture"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white"{}
        _Color("Tint", Color) = (1,1,1,1)
        
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha("Enable External Alpha", Float) = 0

        _CellSize("Cell Size", Range(0, 5)) = 1
        [IntRange]_Roughness("Roughness", Range(1, 8)) = 3
        _Persistance("Persistance", Range(0, 1)) = 0.4
        _MainColour("Main Colour", Color) = (1,1,1,1)
        _MainColour2("Main Colour2", Color) = (1,1,1,1)
        _MainColour3("Main Colour3", Color) = (1,1,1,1)
        _MainColour4("Main Colour4", Color) = (1,1,1,1)
        _MainColour5("Main Colour5", Color) = (1,1,1,1)
        _SubColour("Sub Colour", Color) = (1,1,1,1)
        _SubColour2("Sub Colour2", Color) = (1,1,1,1)
        _SubColour3("Sub Colour3", Color) = (1,1,1,1)
        _SubColour4("Sub Colour4", Color) = (1,1,1,1)
        _SubColour5("Sub Colour5", Color) = (1,1,1,1)


        _Pixelization("Pixelization", float) = 4.0
        _RandomMulti("Random Multiplier", float) = 2.1
    }

        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                //"CanUseSpriteAtlas" = "True"
            }

            Cull Off
            Lighting Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            Pass
            {
            CGPROGRAM
                #pragma target 2.0
                //#pragma multi_compile_instancing
                //#pragma multi_compile_local _ PIXELSNAP_ON
                //#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
                //#include "UnitySprites.cginc"
                #include "UnityCG.cginc"
                #include "HLSLSupport.cginc"
                #include "Assets/Scripts/Shader/TestSetup/ClassicNoise2D.hlsl"
                #include "noiseSimplex.cginc"


                #pragma vertex vert
                #pragma fragment frag

                //#include "Random.cginc"

                //global shader variables
                #define OCTAVES 2

                float _CellAmount;
                float _Roughness;
                float _Persistance;
                float2 _Period;

                sampler2D _MainTex;
                fixed4 _MainColour;
                fixed4 _MainColour2;
                fixed4 _MainColour3;
                fixed4 _MainColour4;
                fixed4 _MainColour5;
                fixed4 _SubColour;
                fixed4 _SubColour2;
                fixed4 _SubColour3;
                fixed4 _SubColour4;
                fixed4 _SubColour5;
                float _Pixelization;
                float _RandomMulti;
                float _CellSize;


                //the object data that's put into the vertex shader
                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                //the data that's used to generate fragments and can be read by the fragment shader
                struct v2f {
                    float4 position : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float4 worldPosition : TEXCOORD1;

                };

                float sampleLayeredNoise(float2 value) {
                    float noise = 0;
                    float frequency = 1;
                    float factor = 1 * _Persistance;

                    [unroll]
                    for (int i = 0; i < OCTAVES; i++) {
                        noise = noise + snoise(value * frequency *0.72354) * factor;
                        factor *= _Persistance;
                        frequency *= _Roughness;
                    }

                    return noise;
                }

                //the vertex shader
                v2f vert(appdata v) {
                    v2f o;
                    //convert the vertex positions from object space to clip space so they can be rendered

                    o.worldPosition = mul(unity_ObjectToWorld, v.vertex);
                    o.position = mul(UNITY_MATRIX_VP, o.worldPosition);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_TARGET{

                    //i.uv = round(i.uv * PixelScaling) / PixelScaling;
                    //pixelization
                    float pixelWidth = 1.0f / _Pixelization;
                    float pixelHeight = 1.0f / _Pixelization;

                    //pixelization of uvs
                    float2 uv = float2((int)(i.worldPosition.x / pixelWidth) * pixelWidth, (int)(i.worldPosition.y / pixelHeight) * pixelHeight);
                    
                    //for scaling noise                   
                    float2 value = uv / (_CellSize);
                    float noise = sampleLayeredNoise(value);

                    float dirtNoise = sampleLayeredNoise(value*0.5f) +0.5f;

                    fixed4 noiseColor;

                    //float modNoise = (fmod(noise, 0.1)*10) *noise);

                    noise = round(noise*10)/10;

                    fixed4 noise4 = fixed4(noise, noise, noise, 1);

                    //fixed4 noiseColor = (noise + _MainColour);
                    if (noise < 0) { //sub dirt
                        //noiseColor = (_SubColour*(1- dirtNoise)); //lighter
                        if (noise >= -1.5 && noise < -1) {
                            noiseColor = (_SubColour);
                        }
                        else if (noise >= -1 && noise <= -0.8) {
                            noiseColor = (_SubColour2);
                        }                         
                        else if (noise >= -0.8 && noise <= -0.5) {
                            noiseColor = (_SubColour3);
                        }                        
                        else if (noise >= -0.5 && noise <= -0.3) {
                            noiseColor = (_SubColour4);
                        }                        
                        else if (noise >= -0.3 && noise <= -0.01) {
                            noiseColor = (_SubColour5);
                        }                        

                    }
                    else if (noise >= 0 && noise < 0.2) { //stone
                        noiseColor = (_MainColour); //darker
                    }
                    else if (noise >= 0.2 && noise < 0.5){
                    noiseColor = (_MainColour2);
                    } else if (noise >= 0.5 && noise < 0.8){
                    noiseColor = (_MainColour3);
                    }else if (noise >= 0.8 && noise < 1.2){
                    noiseColor = (_MainColour4);
                    }else if (noise >= 1.2 && noise < 1.5){
                    noiseColor = (_MainColour5);
                    }
                    
                    //noiseColor.a = 1;
                    fixed4 testColour = noise;
                    //testColour.a = 1;

                    return noiseColor;
                }

            ENDCG
            }
        }
}
