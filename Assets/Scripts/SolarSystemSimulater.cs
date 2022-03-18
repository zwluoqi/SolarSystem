using System;
using System.Collections.Generic;
using UnityEngine;


public class SolarSystemSimulater:MonoBehaviour
{
    public Astronomical[] _astronomicals;
    [Range(0.01f,1000)]
    public float simulaterSpeed = 1;
    [Range(1,1000)]
    public int iterNumbers = 1;
    
    [Range(1,1000)]
    public int runningIterNumber = 10;
    public Astronomical centerTrans;
    [NonSerialized]
    public Astronomical defaultTrans;

    public Material lineMaterial;
    
    public bool debugShow = true;
    public static SolarSystemSimulater _Inst;
    public static SolarSystemSimulater Inst
    {
        get
        {
            if (_Inst == null)
            {
                _Inst = FindObjectOfType<SolarSystemSimulater>();
            }

            return _Inst;
        }
    }
    
    private void Awake()
    {
        defaultTrans = centerTrans;
        Time.fixedDeltaTime = GlobalDefine.deltaTime;
    }
    
    private void FixedUpdate()
    {
        _astronomicals = FindObjectsOfType<Astronomical>();


        for (int i = 0; i < runningIterNumber; i++)
        {
            foreach (var astronomical in _astronomicals)
            {
                astronomical.UpdateVelocity(_astronomicals,  simulaterSpeed* GlobalDefine.deltaTime);
            }

            foreach (var astronomical in _astronomicals)
            {
                astronomical.UpdatePosition( simulaterSpeed*GlobalDefine.deltaTime);
            }
        }
    }

    public Vector3 GetRelativeSpeed(Astronomical selectAstron)
    {
        return selectAstron.GetCurrentVelocity() - centerTrans.GetCurrentVelocity();
    }

    /// <summary>
    /// 对某点加速度最大的天体
    /// 综合加速度
    /// 最大加速度
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="astrons"></param>
    /// <returns></returns>
    public (Astronomical,Vector3,Vector3) GetMaxAccelerationAstron(Vector3 pos, Astronomical[] astrons)
    {
        var astronAcceleration = Vector3.zero;
        var maxAccelerationValue = float.MinValue;
        var maxAcceleration = Vector3.zero;
        Astronomical nearestAstronomical = null;
        foreach (var astronomical in astrons)
        {
            var sqrtDistance = Vector3.SqrMagnitude(astronomical._rigidbody.position - pos);
            var forceDir = (astronomical._rigidbody.position - pos).normalized;
            var acceleration = forceDir * GlobalDefine.G  * astronomical.Mass / sqrtDistance;
            astronAcceleration += acceleration;
            if (acceleration.magnitude > maxAccelerationValue)
            {
                maxAccelerationValue = acceleration.magnitude;
                maxAcceleration = acceleration;
                nearestAstronomical = astronomical;
            }
        }

        return (nearestAstronomical,astronAcceleration,maxAcceleration);
    }
    
    
    /// <summary>
    /// 对某点距离最近的天体
    /// 距离平方
    /// 加速度
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="astrons"></param>
    /// <returns></returns>
    public (Astronomical,float,Vector3) GetNearestDistanceAstron(Vector3 pos, Astronomical[] astrons)
    {
        var minSqrtDistanceValue = float.MaxValue;
        var minDistanceAcceleration = Vector3.zero;
        Astronomical nearestAstronomical = null;
        foreach (var astronomical in astrons)
        {
            var sqrtDistance = Vector3.SqrMagnitude(astronomical._rigidbody.position - pos);
            if (sqrtDistance < minSqrtDistanceValue)
            {
                minSqrtDistanceValue = sqrtDistance;
                nearestAstronomical = astronomical;
            }
        }

        var forceDir = (nearestAstronomical._rigidbody.position - pos).normalized;
        var acceleration = forceDir * GlobalDefine.G  * nearestAstronomical.Mass / minSqrtDistanceValue;
        minDistanceAcceleration = acceleration;

        return (nearestAstronomical, minSqrtDistanceValue, minDistanceAcceleration);
    }
}