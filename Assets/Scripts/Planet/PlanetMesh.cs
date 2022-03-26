using System;
using System.Collections;
using System.Collections.Generic;
using Planet;
using Planet.Setting;
using UnityEngine;

public class PlanetMesh : MonoBehaviour
{
    [Range(2,128)]
    public int resolution = 4;

    [Range(0.1f,10000)]
    public float radius=1;

    public ShapeSettting ShapeSettting;
    
    Vector3[] faceNormal = {Vector3.forward, Vector3.back, Vector3.right, Vector3.left, Vector3.up, Vector3.down};
    private FaceGenerate[] faceGenerates;
    private MeshFilter[] _meshFilterss;

    ShapeGenerate ShapeGenerate;
    public void Generate()
    {
        if (_meshFilterss == null)
        {
            _meshFilterss = new MeshFilter[6];
            for (int i = 0; i < 6; i++)
            {
                var meshRenderer = (new GameObject(i+"")).AddComponent<MeshRenderer>();
                meshRenderer.transform.localPosition = Vector3.zero;
                var meshFilter = meshRenderer.gameObject.AddComponent<MeshFilter>();
                var meshCollider = meshFilter.gameObject.AddComponent<MeshCollider>();
                
                meshFilter.sharedMesh = new Mesh();
                meshCollider.sharedMesh = meshFilter.sharedMesh; 
                meshRenderer.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                _meshFilterss[i] = meshFilter;
            }
        }
        
        faceGenerates = new FaceGenerate[6];
        ShapeGenerate = new ShapeGenerate(ShapeSettting);
        for (int i = 0; i < 6; i++)
        {
            faceGenerates[i] = new FaceGenerate(ShapeGenerate,_meshFilterss[i].mesh,faceNormal[i],resolution,radius);
            faceGenerates[i].Update();
        }
    }

    [NonSerialized]
    public bool shpaeSetttingsFoldOut;
    public void OnShapeSetttingUpdated()
    {
        for (int i = 0; i < 6; i++)
        {
            faceGenerates[i].Update();
        }
    }

    private void OnValidate()
    {
        Generate();
    }
}
