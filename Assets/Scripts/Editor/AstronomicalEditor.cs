using System.Collections.Generic;
using Codice.Client.BaseCommands.ChangeLists;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Astronomical))]
[CanEditMultipleObjects]
public class AstronomicalEditor : UnityEditor.Editor
{
    public Astronomical my;
    private GUISkin guiSkin;

    private void OnEnable()
    {
        my = (Astronomical)target;
        guiSkin = Resources.Load<GUISkin>("solar");
        guiSkin.label.alignment = TextAnchor.LowerLeft;
        guiSkin.label.normal.textColor = Color.white;
    }

    public static float angle = 0;
    // public static float distance = 1000;
    // public static float speed = 0;
    public bool show = false;
    public bool showSpeed = false;
    public bool showNormalSpeed = false;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
        // show = EditorGUILayout.Toggle("三体稳定运动", show);
        showSpeed = EditorGUILayout.Toggle("切线速度调整", showSpeed);
        // showNormalSpeed = EditorGUILayout.Toggle("正常速度调整", showNormalSpeed);
        // EditorGUILayout.Vector3Field("引力加速度:",my.GetCurAcceleration());
        EditorGUILayout.Vector3Field("当前速度:",my.GetCurrentVelocity());
        EditorGUILayout.FloatField("公转周期:",my.GetCircleTime());

        //
        var astronomicals = GameObject.FindObjectsOfType<Astronomical>();
        //
        // List<Astronomical> tmps = new List<Astronomical>();
        // foreach (var astronomical in astronomicals)
        // {
        //     if (astronomical != this)
        //     {
        //         tmps.Add(astronomical);
        //     }
        // }
        //
        // if (show)
        // {
        //     angle = EditorGUILayout.FloatField("角度", angle);
        //     distance = EditorGUILayout.FloatField("距离", distance);
        //
        //
        //     var dir = Vector3.up;
        //     var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        //     var targetDir = rotation * dir;
        //
        //     
        //     tmps[0].transform.position = my.transform.position - targetDir.normalized * distance;
        //     tmps[1].transform.position = my.transform.position + targetDir.normalized * distance;
        //
        // }
        // else
        if (showSpeed)
        {
        
            var (nearest, acc, maxAcc) =
                SolarSystemSimulater.Inst.GetMaxAccelerationAstron(my.transform.position, astronomicals);
        
            var tangentSpeed = EditorGUILayout.FloatField("切线速度", my.InitVelocity.magnitude);
            angle = EditorGUILayout.FloatField("角度", angle);
            
        
            var rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            var dir2near = nearest.transform.position - my.transform.position;
        
            var rotationDir = rotation * dir2near;
            
            var tangentDir = Vector3.Cross( Vector3.forward,rotationDir);
        
            
            
            my.InitVelocity = tangentDir.normalized * tangentSpeed;
            
            // my.InitVelocity = speed;
            // tmps[0].InitVelocity = -my.InitVelocity * 0.5f;
            // tmps[1].InitVelocity = -my.InitVelocity * 0.5f;
        }
        // else if (showNormalSpeed)
        // {
        //     // my.InitVelocity = speed;
        //     tmps[0].InitVelocity = -my.InitVelocity * 0.5f;
        //     tmps[1].InitVelocity = -my.InitVelocity * 0.5f;
        // }
        //
        //
        //
        centerObj = (Astronomical)EditorGUILayout.ObjectField(centerObj, typeof(Astronomical));
        if (GUILayout.Button("公转速度"))
        {
            var sun = centerObj;
            if (sun == null)
            {
                return;
            }
            SerializedObject serializedRendererFeaturesEditor = this.serializedObject;
            SerializedProperty nameProperty = serializedRendererFeaturesEditor.FindProperty("InitVelocity");

            
            nameProperty.vector3Value = sun.GetACircleInitVelocity(my.transform.position);
            
            serializedRendererFeaturesEditor.ApplyModifiedProperties();
            serializedObject.ApplyModifiedProperties();
        }
    }

    public Astronomical centerObj;
    
    public void OnSceneGUI()
    {
        //第一个参数为在场景中显示的位置(以物体的中心位置为基准)
        //第二个参数为显示的名字
        //用于在场景中显示设置的名字
        
        Handles.Label(my.transform.position+new Vector3(0,my.Radius,0),my.name,guiSkin.label);
        Handles.color = Color.red;
        Handles.DrawLine(my.transform.position,my.GetCurrentVelocity().normalized*my.Radius*1.5f+my.transform.position);

        if (SolarSystemSimulater.Inst.showGravity)
        {
            var gravityCircleMin = my.surfaceGravity * SolarSystemSimulater.Inst.minGravityScale;
            var gravityCircleMax = my.surfaceGravity * SolarSystemSimulater.Inst.maxGravityScale;
            //Mass = surfaceGravity * Radius * Radius / GlobalDefine.G;
            var circleMin = my.Radius * Mathf.Sqrt(my.surfaceGravity / gravityCircleMin);
            var circleMax = my.Radius * Mathf.Sqrt(my.surfaceGravity / gravityCircleMax);

            Handles.color =my._color;
            Handles.DrawWireDisc(my.transform.position, Vector3.forward, circleMin);
            Handles.DrawWireDisc(my.transform.position, Vector3.forward, circleMax);
        }
    }
    
}
