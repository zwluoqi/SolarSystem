using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;


public class Simulater:IDisposable
{

    public static Simulater Inst = new Simulater();

    private int iterNumbers;
    private int simulaterSpeed;
    private AstronomicalData center;
    private Dictionary<int,AstronomicalData> cacheAstron = new Dictionary<int,AstronomicalData>(128);

    private int astronomicalListCount = 0;
    private AstronomicalData[] astronomicalDatas = new AstronomicalData[128];
    
    private void SetPosition(AstronomicalData astronomical, int index)
    {
        astronomical.cachePrePoss[index] = astronomical.cacheCurPoss;
    }

    public void UpdateVelocity(AstronomicalData self, float fixedTime)
    {
        for (int asIndex = 0; asIndex < astronomicalListCount; asIndex++)
        {
            var astronomical = astronomicalDatas[asIndex];
            if (astronomical != self)
            {
                var sqrtDistance = Vector3.SqrMagnitude(astronomical.cacheCurPoss- self.cacheCurPoss);
                var forceDir = (astronomical.cacheCurPoss- self.cacheCurPoss).normalized;
                var force = forceDir * GlobalDefine.G * self.Mass * astronomical.Mass / sqrtDistance;
                var acceleration = force / self.Mass;
                self.cacheCurVelocity += acceleration * fixedTime;
            }
        }
    }

    public void UpdatePosition(AstronomicalData self,float fixedTime)
    {
        if (center == null)
        {
            self.cacheCurPoss += self.cacheCurVelocity * fixedTime;
        }
        else
        {
            if (center == self)
            {
                
            }
            else
            {
                var relativeVolocity = self.cacheCurVelocity-
                                       center.cacheCurVelocity;
                self.cacheCurPoss += relativeVolocity * fixedTime;
            }
        }
    }

    private Thread _thread;
    private bool threadRunning;

    public Simulater()
    {
        _thread = new Thread(StartThread);
        threadRunning = true;
        _thread.Start();
    }

    void StartThread()
    {
        while (threadRunning)
        {
            ThreadUpdate();
            Thread.Sleep(60);
        }
        Debug.LogError("thread done");
    }
    
     ~Simulater()
     {
         lock (_threadObj)
         {
             threadRunning = false;
         }

         Debug.LogError("Simulater ~");
         // _thread.Abort();
    }

     public void Dispose()
     {
         lock (_threadObj)
         {
             threadRunning = false;
         }

         Debug.LogError("Simulater Dispose");
     }


     public object _threadObj = new object();

    
    public void Update()
    {
        if (!SolarSystemSimulater.Inst.debugShow)
        {
            return;
        }
        var _astronomicals = GameObject.FindObjectsOfType<Astronomical>();
        
        lock (_threadObj)
        {
            foreach (var astronomical in _astronomicals)
            {
                var id = astronomical.GetInstanceID();
                
                if (!cacheAstron.TryGetValue(id,out var tmp))
                {
                    tmp = new AstronomicalData();
                    cacheAstron[id] = tmp;
                    astronomicalDatas[astronomicalListCount++] = tmp;
                }
                tmp.initMass = astronomical.Mass;
                tmp.initCurPoss = astronomical.transform.position;
                tmp.initCurVelocity = astronomical.CurrentVelocity;
            }

            if (SolarSystemSimulater.Inst.centerTrans != null)
            {
                center = cacheAstron[SolarSystemSimulater.Inst.centerTrans.GetInstanceID()];
            }
            else
            {
                center = null;
            }

            iterNumbers = SolarSystemSimulater.Inst.iterNumbers;
            simulaterSpeed = SolarSystemSimulater.Inst.simulaterSpeed;
            for (int i = 0; i < _astronomicals.Length; i++)
            {
                Handles.color = _astronomicals[i]._color;
                Handles.DrawPolyLine(astronomicalDatas[i].cacheToMainPrePoss);
            }
        }
        
        // Profiler.BeginSample("DrawPolyLine");
        //
        //
        // Profiler.EndSample();
    }
    
    public class AstronomicalData
    {
        public float Mass;
        public Vector3 cacheCurPoss;
        public Vector3 cacheCurVelocity;
        public float initMass;
        public Vector3 initCurPoss;
        public Vector3 initCurVelocity;
        
        public Vector3[] cachePrePoss = new Vector3[1024];
        public Vector3[] cacheToMainPrePoss = new Vector3[1024];
    }
    
    public void ThreadUpdate()
    {
        lock (_threadObj)
        {
            for (int asIndex = 0; asIndex < astronomicalListCount; asIndex++)
            {
                var astronomical = astronomicalDatas[asIndex];
                astronomical.Mass = astronomical.initMass;
                astronomical.cacheCurPoss = astronomical.initCurPoss;
                astronomical.cacheCurVelocity = astronomical.initCurVelocity;
            }
        }
        
        for (int i = 0; i < 1024; i++)
        {
            for (int j = 0; j < iterNumbers; j++)
            {
                for (int asIndex = 0; asIndex < astronomicalListCount; asIndex++)
                {
                    var astronomical = astronomicalDatas[asIndex];
                    UpdateVelocity(astronomical, simulaterSpeed * GlobalDefine.deltaTime);
                }

                for (int asIndex = 0; asIndex < astronomicalListCount; asIndex++)
                {
                    var astronomical = astronomicalDatas[asIndex];
                    UpdatePosition(astronomical, simulaterSpeed * GlobalDefine.deltaTime);
                }
            }

            for (int asIndex = 0; asIndex < astronomicalListCount; asIndex++)
            {
                var astronomical = astronomicalDatas[asIndex];
                SetPosition(astronomical, i);
            }
        }

        lock (_threadObj)
        {
            for (int asIndex = 0; asIndex < astronomicalListCount; asIndex++)
            {
                var astronomical = astronomicalDatas[asIndex];
                Array.Copy(astronomical.cachePrePoss, astronomical.cacheToMainPrePoss,astronomical.cachePrePoss.Length);
            }
        }
    }

}
    
