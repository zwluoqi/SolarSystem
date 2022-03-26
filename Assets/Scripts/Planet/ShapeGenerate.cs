using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public enum NoiseType
    {
            CNOISE,
            SNOISE,
    }
    public class ShapeGenerate
    {
        public ShapeSettting ShapeSettting;

        public ShapeGenerate(ShapeSettting shapeSettting)
        {
            this.ShapeSettting = shapeSettting;
        }
        //
        public float Execulate(Vector3 normalPos)
        {
            //(0,1);
            float value;
            switch (ShapeSettting._noiseSetting.noiseType)
            {
                case NoiseType.CNOISE:
                value = Unity.Mathematics.noise.cnoise(normalPos);
                    break;;
                case NoiseType.SNOISE:
                    value = Unity.Mathematics.noise.snoise(normalPos);
                    break;;
                default:
                        value = 1;
                        break;
            }
            
            float noise =  (value+ShapeSettting._noiseSetting.offset) *ShapeSettting._noiseSetting.strength;
            return noise;
        }
    }
}