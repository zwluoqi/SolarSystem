using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Interac3DObject))]
[RequireComponent(typeof(SkyCell))]
[RequireComponent(typeof(Rigidbody))]
public class Astronomical : MonoBehaviour
{
    // public Rigidbody _rigidbody;
    public float Radius = 1500;
    
    public float Mass =1;

    public Vector3 InitVelocity;

    public Color _color = Color.red;
    public float surfaceGravity = 10;

    public Astronomical circleAstronomical;
    private Vector3 curAcceleration;
    private Vector3 currentVelocity;
    private float circleTime;

    private MaterialPropertyBlock _materialPropertyBlock;

    private TrailRenderer _trailRenderer;
    
    private void Start()
    {
        var _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;
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
    
    public float GetCircleTime()
    {
        return circleTime;
    }
    
    public void UpdateVelocity(Astronomical[] astronomicals, float fixedTime)
    {
        UpdateAcceleration(astronomicals);
        currentVelocity += curAcceleration * fixedTime;
    }

    private void UpdateAcceleration(Astronomical[] astronomicals)
    {
        curAcceleration = AstronomicalUtil.GetAccelerationByPosition(astronomicals, this.transform.position);
    }

    public void UpdatePosition(float fixedTime)
    {

        if (SolarSystemSimulater.Inst.centerTrans == this)
        {
            //原点,相对位置不变
            int i = 0;
        }
        else
        {
            Vector3 relativeVelocity = currentVelocity;
            if (SolarSystemSimulater.Inst.centerTrans != null)
            {
                 relativeVelocity -=  SolarSystemSimulater.Inst.centerTrans.GetCurrentVelocity();
            }

            transform.position += relativeVelocity * fixedTime;
        }
    }

    public void OnValidate()
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
        //4π²/GM
        // _rigidbody = GetComponent<Rigidbody>();
        if (!Application.isPlaying)
        {
            currentVelocity = InitVelocity;
        }

        var  astronomicals =  GameObject.FindObjectsOfType<Astronomical>();
        UpdateAcceleration(astronomicals);
        
        if (circleAstronomical != null)
        {
            var R = Vector3.Distance(circleAstronomical.transform.position, this.transform.position);
            circleTime = (R*R*R)*(4 * Mathf.PI * Mathf.PI )/ (GlobalDefine.G * circleAstronomical.Mass);
            circleTime = Mathf.Sqrt(circleTime);
        }
        else
        {
            circleTime = 0;
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

    
    /// <summary>
    /// 获取某点的稳定引力公转速度
    /// </summary>
    /// <param name="transformPosition"></param>
    /// <returns></returns>
    public Vector3 GetACircleInitVelocity(Vector3 transformPosition)
    {
        var distance = Vector3.Distance(this.transform.position, transformPosition);
        var normal = (this.transform.position - transformPosition).normalized;
        var randomVec = MathUtil.RandomVector();
        var tangentDir = Vector3.Cross(normal, randomVec).normalized;
        // var binTangentDir = Vector3.Cross(tangentDir, normal);
        // var speedDir = tangentDir * speedAxis.x + binTangentDir * speedAxis.y;
        var vector3Value = this.InitVelocity + tangentDir*Mathf.Sqrt((GlobalDefine.G * this.Mass / distance));
        return vector3Value;
    }
    
    public Vector3 GetUpCircleInitVelocity(Vector3 transformPosition,Vector3 speedAxis)
    {
        var distance = Vector3.Distance(this.transform.position, transformPosition);
        var normal = (this.transform.position - transformPosition).normalized;
        // var randomVec = MathUtil.RandomVector();
        var randomVec = speedAxis;
        var tangentDir = Vector3.Cross(normal, randomVec).normalized;
        
        var vector3Value = this.InitVelocity + tangentDir*Mathf.Sqrt((GlobalDefine.G * this.Mass / distance));
        return vector3Value;
    }
}
