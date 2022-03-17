using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Astronomical : MonoBehaviour
{
    
    public float Radius = 1500;
    
    public float Mass =1;
    public Vector3 InitVelocity;
    public Vector3 CurrentVelocity;
    public Color _color = Color.red;
    public float surfaceGravity = 10;
    private Vector3 savePos;
    private Vector3 saveVelocity;

    private MaterialPropertyBlock _materialPropertyBlock;

    private TrailRenderer _trailRenderer;
    
    private void Start()
    {
        this.CurrentVelocity = InitVelocity;
        _materialPropertyBlock = new MaterialPropertyBlock();
        _trailRenderer = (new GameObject()).AddComponent<TrailRenderer>();
        _trailRenderer.transform.SetParent(this.transform);
        _trailRenderer.transform.localPosition = Vector3.zero;
        _trailRenderer.transform.localScale = Vector3.one;
        _trailRenderer.widthMultiplier = Radius;
        _trailRenderer.startColor = _color * 0.7f;
        _trailRenderer.endColor = _color * 0.3f;
        _trailRenderer.sharedMaterial = SolarSystemSimulater.Inst.lineMaterial;
    }
    
    public void UpdateVelocity(Astronomical[] astronomicals, float fixedTime)
    {
        foreach (var astronomical in astronomicals)
        {
            if (astronomical != this)
            {
                var sqrtDistance = Vector3.SqrMagnitude(astronomical.transform.position - transform.position);
                if (sqrtDistance == 0)
                {
                    continue;
                }
                var forceDir = (astronomical.transform.position - transform.position).normalized;
                var force = forceDir * GlobalDefine.G * Mass * astronomical.Mass / sqrtDistance;
                var acceleration = force / Mass;
                CurrentVelocity += acceleration * fixedTime;
            }
        }
    }

    public void UpdatePosition(float fixedTime)
    {
        transform.position += CurrentVelocity * fixedTime;
    }

    private void OnValidate()
    {
        this.transform.localScale = new Vector3(this.Radius,this.Radius,this.Radius);
        if (_materialPropertyBlock == null)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }
        GetComponent<Renderer>().GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetColor("_BaseColor",_color);
        GetComponent<Renderer>().SetPropertyBlock(_materialPropertyBlock);
        Mass = surfaceGravity * Radius * Radius / GlobalDefine.G;
    }
}
