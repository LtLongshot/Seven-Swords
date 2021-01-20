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

        _CellAmount("Cell Amount", Range(1, 32)) = 2
        _Period("Repeat every X cells", Vector) = (4, 4, 0, 0)
        [IntRange]_Roughness("Roughness", Range(1, 8)) = 3
        _Persistance("Persistance", Range(0, 1)) = 0.4
        _MainColour("Main Colour", Color) = (1,1,1,1)


        _Pixelization("Pixelization", float) = 4.0
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


                #pragma vertex vert
                #pragma fragment frag

                //#include "Random.cginc"

                //global shader variables
                #define OCTAVES 4 

                float _CellAmount;
                float _Roughness;
                float _Persistance;
                float2 _Period;

                fixed4 _MainColour;
                float _Pixelization;

                //the object data that's put into the vertex shader
                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                //the data that's used to generate fragments and can be read by the fragment shader
                struct v2f {
                    float4 position : SV_POSITION;
                    float2 uv : TEXCOORD0;
                };

                float easeIn(float interpolator) {
                    return interpolator * interpolator;
                }

                float easeOut(float interpolator) {
                    return 1 - easeIn(1 - interpolator);
                }

                float easeInOut(float interpolator) {
                    float easeInValue = easeIn(interpolator);
                    float easeOutValue = easeOut(interpolator);
                    return lerp(easeInValue, easeOutValue, interpolator);
                }

                float2 modulo(float2 divident, float2 divisor) {
                    float2 positiveDivident = divident % divisor + divisor;
                    return positiveDivident % divisor;
                }

                float rand2dTo1d(float2 value, float2 dotDir = float2(12.9898, 78.233)) {
                    float2 smallValue = cos(value);
                    float random = dot(smallValue, dotDir);
                    random = frac(sin(random) * 41238.311);
                    return random;
                }

                float2 rand2dTo2d(float2 value) {
                    return float2(
                        rand2dTo1d(value, float2(12.989, 78.233)),
                        rand2dTo1d(value, float2(39.346, 11.135))
                        );
                }


                float perlinNoise(float2 value, float2 period) {
                    float2 cellsMimimum = floor(value);
                    float2 cellsMaximum = ceil(value);

                    cellsMimimum = modulo(cellsMimimum, period);
                    cellsMaximum = modulo(cellsMaximum, period);


                    //generate random directions
                    float2 lowerLeftDirection = rand2dTo2d(float2(cellsMimimum.x, cellsMimimum.y)) * 2 - 1;
                    float2 lowerRightDirection = rand2dTo2d(float2(cellsMaximum.x, cellsMimimum.y)) * 2 - 1;
                    float2 upperLeftDirection = rand2dTo2d(float2(cellsMimimum.x, cellsMaximum.y)) * 2 - 1;
                    float2 upperRightDirection = rand2dTo2d(float2(cellsMaximum.x, cellsMaximum.y)) * 2 - 1;

                    float2 fraction = frac(value);

                    //get values of cells based on fraction and cell directions
                    float lowerLeftFunctionValue = dot(lowerLeftDirection, fraction - float2(0, 0));
                    float lowerRightFunctionValue = dot(lowerRightDirection, fraction - float2(1, 0));
                    float upperLeftFunctionValue = dot(upperLeftDirection, fraction - float2(0, 1));
                    float upperRightFunctionValue = dot(upperRightDirection, fraction - float2(1, 1));

                    float interpolatorX = easeInOut(fraction.x);
                    float interpolatorY = easeInOut(fraction.y);

                    //interpolate between values
                    float lowerCells = lerp(lowerLeftFunctionValue, lowerRightFunctionValue, interpolatorX);
                    float upperCells = lerp(upperLeftFunctionValue, upperRightFunctionValue, interpolatorX);

                    float noise = lerp(lowerCells, upperCells, interpolatorY);
                    return noise;
                }

                float sampleLayeredNoise(float2 value) {
                    float noise = 0;
                    float frequency = 1;
                    float factor = 1;

                    [unroll]
                    for (int i = 0; i < OCTAVES; i++) {
                        noise = noise + perlinNoise(value * frequency + i * 0.72354, _Period * frequency) * factor;
                        factor *= _Persistance;
                        frequency *= _Roughness;
                    }

                    return noise;
                }

                //the vertex shader
                v2f vert(appdata v) {
                    v2f o;
                    //convert the vertex positions from object space to clip space so they can be rendered
                    o.position = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                fixed4 frag(v2f i) : SV_TARGET{

                    //i.uv = round(i.uv * PixelScaling) / PixelScaling;
                    //pixelization
                    float pixelWidth = 1.0f / _Pixelization;
                    float pixelHeight = 1.0f / _Pixelization;

                    half2 uv = half2((int)(i.uv.x / pixelWidth) * pixelWidth, (int)(i.uv.y / pixelHeight) * pixelHeight);

                    float2 value = uv * _CellAmount;
                    //get noise and adjust it to be ~0-1 range
                    float noise = sampleLayeredNoise(value) + 0.5;
                    noise = clamp(noise, 0.25, 0.75);


                    fixed4 color = (_MainColour*noise);
                    color.a = 1;
                    return color;
                }

            ENDCG
            }
        }
}
