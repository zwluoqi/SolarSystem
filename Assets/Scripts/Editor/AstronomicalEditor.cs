using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Astronomical))]
[CanEditMultipleObjects]
public class AstronomicalEditor : UnityEditor.Editor
{
    public Astronomical my;

    private void OnEnable()
    {
        my = (Astronomical)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (!Application.isPlaying)
        {
            my.CurrentVelocity = my.InitVelocity;
        }

        
    }


    public void OnSceneGUI()
    {
        //第一个参数为在场景中显示的位置(以物体的中心位置为基准)
        //第二个参数为显示的名字
        //用于在场景中显示设置的名字
        Handles.Label(my.transform.position+new Vector3(0,1,0),my.name);

        if (SolarSystemSimulater.Inst != null)
        {
            Simulater.Inst.Update();
        }
        
    }
    
}
