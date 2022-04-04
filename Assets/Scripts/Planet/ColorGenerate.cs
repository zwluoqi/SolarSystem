using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class ColorGenerate
    {
        public ColorSettting ColorSettting;
        private Texture2D texture2D;
        public LayerNoiseGenerate layerNoiseGenerate = new LayerNoiseGenerate();
        public void UpdateConfig(ColorSettting colorSettting)
        {
            this.ColorSettting = colorSettting;
            if (texture2D == null || colorSettting.resolution*2 != texture2D.width || colorSettting.LatitudeSettings.Length != texture2D.height)
            {
                texture2D = new Texture2D(colorSettting.resolution*2, colorSettting.LatitudeSettings.Length, TextureFormat.RGBA32, 1,false);
                texture2D.wrapMode = TextureWrapMode.Clamp;//不然在边界采样值会有问题,因为如多是爽
            }
            this.layerNoiseGenerate.UpdateConfig(colorSettting.noiseEnable,colorSettting.noiseSetting,colorSettting.noiseLayers);
        }
        
        public Color Execute()
        {
            return ColorSettting.tinyColor;
        }

        public Texture2D GenerateTexture2D()
        {
            Color[] colors = new Color[ColorSettting.resolution*2*ColorSettting.LatitudeSettings.Length];
            int colorIndex = 0;
            for (int latitude = 0; latitude < ColorSettting.LatitudeSettings.Length; latitude++)
            {
                if (ColorSettting.LatitudeSettings[latitude].gradient.colorKeys.Length == 2)
                {
                    ColorSettting.LatitudeSettings[latitude].gradient.colorKeys = ColorSettting.gradient.colorKeys;
                }
                for (int i = 0; i < ColorSettting.resolution*2; i++)
                {
                    if (i < ColorSettting.resolution)
                    {
                        colors[colorIndex++] = ColorSettting.ocean.Evaluate(1.0f*(i) / ColorSettting.resolution);
                    }
                    else
                    {
                        var gradientColor = ColorSettting.LatitudeSettings[latitude].gradient.Evaluate(1.0f*(i - ColorSettting.resolution) / ColorSettting.resolution);
                        colors[colorIndex++] = Color.Lerp(gradientColor,ColorSettting.LatitudeSettings[latitude].tinyColor
                            , ColorSettting.LatitudeSettings[latitude].tinyPercent);
                    }
                }
            }
            texture2D.SetPixels(colors);
            texture2D.Apply(true);
            return texture2D;
        }

        public float FormatHeight(Vector3 vertex,float height)
        {
            height = (height + 1) * 0.5f;
            int latitudeIndex = 0;
            for (int i = 0; i < ColorSettting.LatitudeSettings.Length; i++)
            {
                // var noise = this.layerNoiseGenerate.Exculate(vertex);
                var noise = Vector2.zero;
                var noiseHeight = noise.y + ColorSettting.LatitudeSettings[i].startHeight;
                if (height < noiseHeight)
                {
                    break;
                }
                else
                {
                    latitudeIndex = i;
                }
            }

            float offset = .5f / ColorSettting.LatitudeSettings.Length;
            return (latitudeIndex*1.0f) / (ColorSettting.LatitudeSettings.Length) + offset;
        }
    }
}