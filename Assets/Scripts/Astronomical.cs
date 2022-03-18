using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class Astronomical : MonoBehaviour
{
    public Rigidbody _rigidbody;
    public float Radius = 1500;
    
    public float Mass =1;

    public Vector3 InitVelocity;

    public Color _color = Color.red;
    public float surfaceGravity = 10;
    
    private Vector3 curAcceleration;
    private Vector3 currentVelocity;

    private MaterialPropertyBlock _materialPropertyBlock;

    private TrailRenderer _trailRenderer;

    public bool move = true;
    
    private void Start()
    {
        this._rigidbody = GetComponent<Rigidbody>();
        this._rigidbody.useGravity = false;
        this._rigidbody.isKinematic = true;
        this.currentVelocity = InitVelocity;
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

    public Vector3 GetCurrentVelocity()
    {
        return currentVelocity;
    }
    public Vector3 GetCurAcceleration()
    {
        return curAcceleration;
    }
    
    public void UpdateVelocity(Astronomical[] astronomicals, float fixedTime)
    {
        if (!move)
        {
            return;
        }
        curAcceleration = Vector3.zero;
        foreach (var astronomical in astronomicals)
        {
            if (astronomical != this)
            {
                var sqrtDistance = Vector3.SqrMagnitude(astronomical._rigidbody.position - _rigidbody.position);
                var forceDir = (astronomical._rigidbody.position - _rigidbody.position).normalized;
                var force = forceDir * GlobalDefine.G * Mass * astronomical.Mass / sqrtDistance;
                var acceleration = force / Mass;
                curAcceleration += acceleration;
                currentVelocity += acceleration * fixedTime;
            }
        }
    }

    public void UpdatePosition(float fixedTime)
    {
        if (!move)
        {
            return;
        }

        if (SolarSystemSimulater.Inst.centerTrans == this)
        {
            //原点,相对位置不变
        }
        else
        {
            Vector3 relativeVelocity = currentVelocity;
            if (SolarSystemSimulater.Inst.centerTrans != null)
            {
                 relativeVelocity -=  SolarSystemSimulater.Inst.centerTrans.GetCurrentVelocity();
            }

            _rigidbody.position += relativeVelocity * fixedTime;
        }
    }

    private void OnValidate()
    {
        this.transform.localScale = new Vector3(this.Radius, this.Radius, this.Radius) * 2;
        if (_materialPropertyBlock == null)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
        }
        GetComponent<Renderer>().GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetColor("_BaseColor",_color);
        GetComponent<Renderer>().SetPropertyBlock(_materialPropertyBlock);
        Mass = surfaceGravity * Radius * Radius / GlobalDefine.G;
        _rigidbody = GetComponent<Rigidbody>();
        if (!Application.isPlaying)
        {
            currentVelocity = InitVelocity;
            UpdateVelocity(GameObject.FindObjectsOfType<Astronomical>(),0);
        }
    }

    [ContextMenu("三体L1点")]
    void SetThreeL1Post()
    {
        var astronomicals = GameObject.FindObjectsOfType<Astronomical>();
        List<Astronomical> tmps = new List<Astronomical>();
        foreach (var astronomical in astronomicals)
        {
            if (astronomical != this)
            {
                tmps.Add(astronomical);
            }
        }

        var p1 = tmps[0].transform.position;
        var p2 = tmps[1].transform.position;
        int minIndex = 0;
        if (tmps[0].Mass > tmps[1].Mass)
        {
            minIndex = 1;
        }
        else
        {
            minIndex = 0;
        }
        var M1 = tmps[0].Mass > tmps[1].Mass ? tmps[0].Mass : tmps[1].Mass;
        var M2 = tmps[0].Mass > tmps[1].Mass ? tmps[1].Mass : tmps[0].Mass;
        var R = Vector3.Distance(p1, p2);

        var r = R * Mathf.Pow(M2 / (3 * M1), 1 / 3);
        this.transform.position = (p2 - p1).normalized * r + tmps[minIndex].transform.position;
    }

    [ContextMenu("三体L4点")]
    void SetThreeL4Post()
    {
        var astronomicals = GameObject.FindObjectsOfType<Astronomical>();
        List<Astronomical> tmps = new List<Astronomical>();
        foreach (var astronomical in astronomicals)
        {
            if (astronomical != this)
            {
                tmps.Add(astronomical);
            }
        }

        var p1 = tmps[0].transform.position;
        var p2 = tmps[1].transform.position;
        var z = p1.z;
        var center = (p1 + p2) / 2;
        var dir = (p2 - p1);
        var up = Vector3.Cross(dir.normalized, Vector3.forward);
        var p3 = center + up * dir.magnitude * Mathf.Sin(60*Mathf.Deg2Rad);
        this.transform.position = p3;
        
        
    }
}
