using System;
using Planet.Setting;
using UnityEngine;

namespace Planet
{
    public class FaceGenerate
    {
        public MeshFilter MeshFilter;
        public Vector3 Normal;
        public int Resolution;

        private Vector3 axisA;
        private Vector3 axisB;
        private ShapeGenerate shapeGenerate;
        private ColorGenerate colorGenerate;

        private ComputeBuffer _bufferVertices;
        private ComputeBuffer _bufferTriangles;
        
        private Vector3[] vertices;
        private int[] triangles;
        private readonly int ResolutionID = Shader.PropertyToID("Resolution");
        private readonly int NormalID = Shader.PropertyToID("Normal");
        private readonly int axisAID = Shader.PropertyToID("axisA");
        private readonly int axisBID = Shader.PropertyToID("axisB");
        private readonly int verticesID = Shader.PropertyToID("vertices");
        
        public FaceGenerate(ShapeGenerate shapeGenerate,
            ColorGenerate colorGenerate,
            MeshFilter meshFilter,Vector3 normal)
        {
            this.shapeGenerate = shapeGenerate;
            this.colorGenerate = colorGenerate;
            MeshFilter = meshFilter;
            Normal = normal;
            //unity 左手坐标系
            axisA = new Vector3(normal.y,normal.z,normal.x);
            axisB = Vector3.Cross(normal, axisA);
        }

        public void Update(int resolution)
        {
            var _computeShader = shapeGenerate.ShapeSettting.computeShader;

            if (Resolution != resolution)
            {
                Resolution = resolution;
                vertices = new Vector3[(Resolution ) * (Resolution )];
                var multiple = (Resolution - 1) * (Resolution - 1);
                triangles = new int[multiple*2*3];
                _bufferVertices = new ComputeBuffer(vertices.Length,3*4);
                _bufferTriangles = new ComputeBuffer(multiple*2*3,4);
                _bufferVertices.SetData(vertices);

            }

            UpdateShape();
            UpdateColor();
        }

        public void UpdateShape()
        {
            var _computeShader = shapeGenerate.ShapeSettting.computeShader;
            // _bufferTriangles.SetData(triangles);
            _computeShader.SetInt(ResolutionID, Resolution);
            _computeShader.SetVector(NormalID, Normal);
            _computeShader.SetVector(axisAID, axisA);
            _computeShader.SetVector(axisBID, axisB);
            //获取内核函数的索引
            var kernelVertices = _computeShader.FindKernel("CSMainVertices");
            _computeShader.SetBuffer(kernelVertices,verticesID,_bufferVertices);
            // var kernelTriangle = _computeShader.FindKernel("CSMainTriangle");
            _computeShader.Dispatch(kernelVertices, Resolution / 8, Resolution / 8, 1);
            // _computeShader.Dispatch(kernelTriangle, Resolution*Resolution / 8, 1 , 1);

            int indicIndex = 0;
            for (int y = 0; y < Resolution; y++)
            {
                for (int x = 0; x < Resolution; x++)
                {
                    var index = x + y * (Resolution);
                    Vector2 percent = new Vector2(x, y) / (Resolution - 1);
                    var pos = Normal+2*axisB * (percent.x - 0.5f) + 2*axisA * (percent.y - 0.5f);
                    vertices[index] = shapeGenerate.Execulate(pos.normalized);
                    
                    if (x < Resolution - 1 && y < Resolution - 1)
                    {
                        //逆时针
                        triangles[indicIndex++] = index;
                        triangles[indicIndex++] = index+Resolution;
                        triangles[indicIndex++] = index+1+Resolution;
                        
                        triangles[indicIndex++] = index+1+Resolution;
                        triangles[indicIndex++] = index+1;
                        triangles[indicIndex++] = index;
                    }
                }
            }

            var mesh = MeshFilter.sharedMesh;
            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.UploadMeshData(false);
        }

        public void UpdateColor()
        {
            var sharedMaterial = MeshFilter.GetComponent<MeshRenderer>().sharedMaterial;
            sharedMaterial.SetColor("_BaseColor",colorGenerate.Execute());
        }
    }
}