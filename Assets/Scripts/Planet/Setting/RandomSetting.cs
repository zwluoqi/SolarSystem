using System;
using UnityEngine;

namespace Planet.Setting
{
    [CreateAssetMenu()]
    public class RandomSetting : ScriptableObject
    {

        [SerializeField]
        public RandomData randomData;

    }

    [Serializable]
    public class RandomData
    {
        [Range(-1,10)]
        public float frequencyAddPercent = 0;
        [Range(-1,10)]
        public float amplitudeAddPercent = 0;

        public Vector3 offsetRange;

        [Range(-1,1)]
        public float latitudeTinyColorOffset;

        public Color startColor = Color.black;
        public Color endColor= Color.white;
    }
}