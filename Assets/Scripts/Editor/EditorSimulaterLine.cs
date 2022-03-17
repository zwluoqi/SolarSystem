using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SolarSystemSimulater))]
public class EditorSimulaterLine : UnityEditor.Editor
{
    public void OnSceneGUI()
    {
        if (SolarSystemSimulater.Inst != null)
        {
            Simulater.Inst.Update();
        }
        
    }
}