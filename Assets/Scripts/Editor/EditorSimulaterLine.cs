using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SolarSystemSimulater))]
public class EditorSimulaterLine : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        // var my = target as SolarSystemSimulater;
        // EditorGUILayout.FloatField("引力常数", my.G);
        //
    }

    public void OnSceneGUI()
    {
        if (SolarSystemSimulater.Inst != null)
        {
            Simulater.Inst.Update();
        }
        
    }
}