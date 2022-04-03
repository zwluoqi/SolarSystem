using UnityEngine;

namespace Planet.Setting
{
    [CreateAssetMenu]
    public class ShapeSettting:ScriptableObject
    {
        
        [Range(0.1f,10000)]
        public float radius=1;
        public ComputeShader computeShader;
        public NoiseSetting _noiseSetting;
    }
}