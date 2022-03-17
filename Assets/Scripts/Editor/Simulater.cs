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

    public int iterNumbers;
    public int simulaterSpeed;
    public int centerIndex;
    private Dictionary<int,AstronomicalData> cacheAstron = new Dictionary<int,AstronomicalData>(128);

    public int astronomicalListCount = 0;
    public AstronomicalData[] astronomicalDatas = new AstronomicalData[128];
    
    public SimulaterThread simulaterThread = new SimulaterThread();
    

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
            simulaterThread.ThreadUpdate(this);
            Thread.Sleep(60);
        }
        Debug.LogError("thread done");
    }
    
     ~Simulater()
     {
         lock (simulaterThread._threadObj)
         {
             threadRunning = false;
         }

         Debug.LogError("Simulater ~");
         // _thread.Abort();
    }

     public void Dispose()
     {
         lock (simulaterThread._threadObj)
         {
             threadRunning = false;
         }

         Debug.LogError("Simulater Dispose");
     }



    
    public void Update()
    {
        if (!SolarSystemSimulater.Inst.debugShow)
        {
            return;
        }
        var _astronomicals = GameObject.FindObjectsOfType<Astronomical>();
        
        lock (simulaterThread._threadObj)
        {
            astronomicalListCount = 0;
            var centerId = SolarSystemSimulater.Inst.centerTrans.GetInstanceID();
            foreach (var astronomical in _astronomicals)
            {
                var id = astronomical.GetInstanceID();
                if (id == centerId)
                {
                    centerIndex = astronomicalListCount;
                }
                
                if (!cacheAstron.TryGetValue(id,out var tmp))
                {
                    tmp = new AstronomicalData();
                    cacheAstron[id] = tmp;
                }
                tmp.initMass = astronomical.Mass;
                tmp.initCurPoss = astronomical._rigidbody.position;
                tmp.initCurVelocity = astronomical.CurrentVelocity;
                astronomicalDatas[astronomicalListCount++] = tmp;
                
                
                Handles.color = astronomical._color;
                Handles.DrawPolyLine(cacheAstron[id].cachePrePoss);
            }

            iterNumbers = SolarSystemSimulater.Inst.iterNumbers;
            simulaterSpeed = SolarSystemSimulater.Inst.simulaterSpeed;
        }
        
    }
    
   
    
    

}
    
