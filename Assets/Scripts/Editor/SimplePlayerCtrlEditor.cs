using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimplePlayerCtrl))]
public class SimplePlayerCtrlEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        var my = target as SimplePlayerCtrl;
        Handles.color = Color.yellow;
        Handles.DrawLine(my.transform.position,my.transform.position+my.AstronAcceleration*100);
    }
}