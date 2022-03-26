using System;
using UnityEngine;

namespace Planet.Setting
{

    [System.Serializable]
    public class NoiseSetting
    {
        public float strength = 1;
        public float offset = 0;
        public NoiseType noiseType = 0;
    }
}