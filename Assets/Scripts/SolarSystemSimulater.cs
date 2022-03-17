using System;
using UnityEngine;


public class SolarSystemSimulater:MonoBehaviour
{
    public Astronomical[] _astronomicals;
    [Range(1,1000)]
    public int simulaterSpeed = 1;
    [Range(1,100)]
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
        return selectAstron.CurrentVelocity - centerTrans.CurrentVelocity;
    }
    
}