using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleTarget : MonoBehaviour
{
    public float distance = 100;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        this.transform.LookAt(target);
        this.transform.position = target.position - Vector3.forward * distance;
    }

    private void OnValidate()
    {
        LateUpdate();
    }
}
