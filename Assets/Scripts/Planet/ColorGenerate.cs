using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class ColorGenerate
    {
        public ColorSettting colorSettting;
        public WaterRenderSetting waterRenderSetting;
        public RandomGenerate randomGenerate = new RandomGenerate();
        public LayerNoiseGenerate layerNoiseGenerate = new LayerNoiseGenerate();
        private Color[] colors;


        public void Update(ColorSettting _colorSettting,WaterRenderSetting _waterRenderSetting,RandomSetting _randomSetting)
        {
            this.colorSettting = _colorSettting;
            this.waterRenderSetting = _waterRenderSetting;
            this.randomGenerate.Update(_randomSetting);
            if (colors == null || colors.Length != colorSettting.resolution  * colorSettting.LatitudeSettings.Length)
            {
                colors = new Color[colorSettting.resolution  * colorSettting.LatitudeSettings.Length];
            }
            this.layerNoiseGenerate.UpdateConfig(colorSettting.noiseEnable,colorSettting.noiseSetting,colorSettting.noiseLayers);
        }
        
        public void GenerateTexture2D(ref Texture2D texture2D,PlanetSettingData planetSettingData)
        {
            if (texture2D == null || colorSettting.resolution != texture2D.width || colorSettting.LatitudeSettings.Length != texture2D.height)
            {
                if (texture2D != null)
                {
                    Object.DestroyImmediate(texture2D);
                }

                texture2D = new Texture2D(colorSettting.resolution , colorSettting.LatitudeSettings.Length,
                    TextureFormat.RGBA32, 1, false) {wrapMode = TextureWrapMode.Clamp};
                //不然在边界采样值会有问题,因为如多是爽
            }
            
            var oceanGradient = colorSettting.ocean;
            Gradient ocean = new Gradient();
            var oceanColorKeys = oceanGradient.colorKeys;
            for (int i = 0; i < oceanGradient.colorKeys.Length; i++)
            {
                oceanColorKeys[i].color = randomGenerate.Range(randomGenerate.randomData.startColor,
                    randomGenerate.randomData.endColor);
            }
            ocean.SetKeys(oceanColorKeys,oceanGradient.alphaKeys);
            
            int colorIndex = 0;
            for (int latitude = 0; latitude < colorSettting.LatitudeSettings.Length; latitude++)
            {
                var latitudeGradient = colorSettting.LatitudeSettings[latitude].gradient;
                Gradient gradient = new Gradient();
                var colorKeys = latitudeGradient.colorKeys;
                for (int i = 0; i < gradient.colorKeys.Length; i++)
                {
                    colorKeys[i].color = randomGenerate.Range(randomGenerate.randomData.startColor,
                        randomGenerate.randomData.endColor);
                }
                gradient.SetKeys(colorKeys,latitudeGradient.alphaKeys);
                var tinyColor =  randomGenerate.Range(randomGenerate.randomData.startColor,
                    randomGenerate.randomData.endColor);
                var tinyPercent = colorSettting.LatitudeSettings[latitude].tinyPercent*(1+randomGenerate.randomData.latitudeTinyColorOffset);
                
                for (int i = 0; i < colorSettting.resolution; i++)
                {
                    if (planetSettingData.ocean)
                    {
                        colors[colorIndex++] =
                            ocean.Evaluate(1.0f * (i) / colorSettting.resolution);
                    }
                    else
                    {
                        var gradientColor = gradient
                            .Evaluate(1.0f * (i ) / colorSettting.resolution);
                        colors[colorIndex++] = Color.Lerp(gradientColor,
                            tinyColor
                            , tinyPercent);

                    }
                }
            }
            texture2D.SetPixels(colors);
            texture2D.Apply(true);
        }

        public float UpdateColorFormatHeight(Vector3 vertex,float height)
        {
            height = (height + 1) * 0.5f;
            float latitudeIndex = 0;
            // var noise = this.layerNoiseGenerate.Exculate(vertex);
            var noise = Vector2.zero;
            float noiseHeight = height + noise.y;
            float blendRange = colorSettting.blendRange + 0.01f;
            for (int i = 0; i < colorSettting.LatitudeSettings.Length; i++)
            {

                float dist = noiseHeight - colorSettting.LatitudeSettings[i].startHeight;
                float weight = Mathf.InverseLerp(-blendRange, blendRange, dist);
                latitudeIndex *= (1.0f - weight);
                latitudeIndex += i + weight;
                // if (height >= noiseHeight)
                // {
                //     latitudeIndex = i;
                // }
            }

            return (latitudeIndex*1.0f) / Mathf.Max(1,colorSettting.LatitudeSettings.Length-1);
        }
    }
}