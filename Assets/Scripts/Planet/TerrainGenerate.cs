using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class TerrainGenerate:System.IDisposable
    {
        private Texture2D texture2D;
        private Material sharedMaterial;

        private FaceGenerate[] faceGenerates;

        readonly Vector3[] faceNormal = {Vector3.up,Vector3.forward, Vector3.left, Vector3.back, Vector3.right,  Vector3.down};
        private VertexGenerate vertexGenerate;
        private ColorGenerate colorGenerate;


        public TerrainGenerate(VertexGenerate _vertexGenerate,ColorGenerate _colorGenerate)
        {
            this.vertexGenerate = _vertexGenerate;
            this.colorGenerate = _colorGenerate;
            faceGenerates = new FaceGenerate[6];
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i] = new FaceGenerate(faceNormal[i]);
            }
        }

        public void UpdateMeshFilter(MeshFilter[] meshFilterss,int resolution)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateMeshFilter(meshFilterss[i],resolution);
            }
        }
        
        public bool UpdateLod(Vector3 cameraPos)
        {
#if UNITY_EDITOR
            int nearestIndex = -1;
            int farestIndex = -1;
            float nearestDistance = float.MaxValue;
            float farestDistance = float.MinValue;
                
            for (int i = 0; i < 6; i++)
            {
                var pos = faceGenerates[i].GetPlanePos(vertexGenerate);
                var dir = cameraPos - pos;
                if (dir.magnitude < nearestDistance)
                {
                    nearestDistance = dir.magnitude;
                    nearestIndex = i;
                }

                if (dir.magnitude > farestDistance)
                {
                    farestDistance = dir.magnitude;
                    farestIndex = i;
                }
            }

            bool refreshShape = false;
            for (int i = 0; i < 6; i++)
            {
                if (nearestIndex == i)
                {
                    refreshShape |=faceGenerates[i].UpdateLODLevel(0);
                }
                else if (farestIndex == i)
                {
                    refreshShape |=faceGenerates[i].UpdateLODLevel(4);
                }
                else
                {
                    refreshShape |=faceGenerates[i].UpdateLODLevel(2);
                }
            }

            return refreshShape;
#endif
        }

        public void UpdateMesh(int resolution ,PlanetSettingData planetSettingData)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].Update(resolution,vertexGenerate,planetSettingData);
            }
            UpdateColor(planetSettingData);
        }
        
        public void UpdateShape( PlanetSettingData planetSettingData)
        {
            System.DateTime start = System.DateTime.Now;
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateShape(vertexGenerate,planetSettingData);
            }

            var start2 = System.DateTime.Now;
            var spaceShape = start2 - start;
            Debug.LogWarning("shape:"+spaceShape.TotalMilliseconds+"ms");
            UpdateColor(planetSettingData);
            var spaceColor = System.DateTime.Now - start2;
            Debug.LogWarning("color:"+spaceColor.TotalMilliseconds+"ms");
        }
        
        public void UpdateColor( PlanetSettingData planetSettingData)
        {
            // Material sharedMaterial;
            InitShareMaterial(colorGenerate.ColorSettting,planetSettingData);
            colorGenerate.GenerateTexture2D(ref texture2D,planetSettingData);
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].FormatHeight(planetSettingData,colorGenerate);
            }

            UpdateMaterialProperty(colorGenerate.ColorSettting,colorGenerate.WaterRenderSetting,planetSettingData);
        }

        private void InitShareMaterial(ColorSettting colorSettting,PlanetSettingData planetSettingData)
        {
            if (sharedMaterial == null)
            {
                if (planetSettingData.ocean)
                {
                    sharedMaterial = Object.Instantiate(colorSettting.oceanMaterial);
                }
                else
                {
                    sharedMaterial = Object.Instantiate(colorSettting.material);
                }

            }
            
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].UpdateMaterial(sharedMaterial);
            }
        }

        public void UpdateMaterialProperty(ColorSettting colorSettting,WaterRenderSetting waterRenderSetting, PlanetSettingData planetSettingData)
        {
            #if UNITY_EDITOR
            InitShareMaterial(colorSettting,planetSettingData);
            sharedMaterial.color = colorSettting.tinyColor;
            sharedMaterial.mainTexture = texture2D;
            sharedMaterial.SetFloat("radius",planetSettingData.radius);
            
            MinMax depth = new MinMax();
            for (int i = 0; i < 6; i++)
            {
                depth.AddValue(faceGenerates[i].depth);
            }
            sharedMaterial.SetVector("_minmax",new Vector4(depth.min,depth.max,0,0));

            UpdateWaterRender(waterRenderSetting);
            #endif
        }
        


        public void UpdateWaterRender(WaterRenderSetting waterRenderSettting)
        {
            if (waterRenderSettting.waterLayers.Length != 0)
            {
                sharedMaterial.SetVectorArray("waves",waterRenderSettting.ToWaveVec4s());    
            }
            sharedMaterial.SetInt("waveLen",waterRenderSettting.waterLayers.Length);
            sharedMaterial.SetFloat("_alphaMultiplier",waterRenderSettting.alphaMultiplier);
            sharedMaterial.SetFloat("_waterSmoothness",waterRenderSettting.waterSmoothness);
        }
        
        
        
        public void Dispose()
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].Dispose();
            }
            Object.Destroy(texture2D);
        }

        public void OnDrawGizmos(float radius)
        {
            for (int i = 0; i < 6; i++)
            {
                faceGenerates[i].OnDrawGizmos(radius);
            }
        }
    }
}