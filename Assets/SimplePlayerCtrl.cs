using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Rigidbody))]
public class SimplePlayerCtrl : MonoBehaviour
{

    public Rigidbody _rigidbody;
    public float rotSpeed = 60;
    public float rotSmoothSpeed = 5;
    public float engineStrenth = 100;
    public Vector3 initSpeed ;
    
    public Vector3 AstronAcceleration;
    public Vector3 inputDir;
    public Quaternion inputRot;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.AddForce( initSpeed, ForceMode.VelocityChange);
    }

    // private void OnCollisionEnter(Collision other)
    // {
    //     _rigidbody.velocity = other.gameObject.GetComponent<Astronomical>().CurrentVelocity;
    //     
    // }
    //
    // private void OnCollisionStay(Collision other)
    // {
    //     _rigidbody.velocity = other.gameObject.GetComponent<Astronomical>().CurrentVelocity;
    //     _rigidbody.Sleep();
    // }

    public int GetInputAxis(KeyCode l, KeyCode r)
    {
        if (Input.GetKey(l))
        {
            return -1;
        }else if (Input.GetKey(r))
        {
            return 1;
        }

        return 0;
    }


    private void FixedUpdate()
    {
        HandleInput();
        Move();
    }

    private void Move()
    {
        var astrons = GameObject.FindObjectsOfType<Astronomical>();
        AstronAcceleration = Vector3.zero;
        foreach (var astronomical in astrons)
        {
            var sqrtDistance = Vector3.SqrMagnitude(astronomical.transform.position - transform.position);
            var forceDir = (astronomical.transform.position - transform.position).normalized;
            var acceleration = forceDir * GlobalDefine.G  * astronomical.Mass / sqrtDistance;
            // var acceleration = force / _rigidbody.mass;
            // CurrentVelocity += acceleration * fixedTime;
            AstronAcceleration += acceleration;
        }
        _rigidbody.AddForce(AstronAcceleration, ForceMode.Acceleration);


        var engineDir = transform.TransformVector(inputDir);
        _rigidbody.AddForce(engineDir * engineStrenth, ForceMode.Acceleration);
        _rigidbody.MoveRotation(inputRot);
    }

    void HandleInput()
    {
        var z = GetInputAxis(KeyCode.S, KeyCode.W);
        var x = GetInputAxis(KeyCode.A, KeyCode.D);
        var y = GetInputAxis(KeyCode.E, KeyCode.Q);
        inputDir = new Vector3(x, y, z);

        var yAbsRot =  Input.GetAxis("Mouse X") * rotSpeed;
        var xAbsRot = Input.GetAxis("Mouse Y") * rotSpeed;

        var yRot = Quaternion.AngleAxis(yAbsRot, transform.up);
        // var xRot = Quaternion.AngleAxis(xAbsRot, transform.right);

        var targetRot =  yRot * transform.rotation;
        inputRot = Quaternion.Slerp(transform.rotation,targetRot,GlobalDefine.deltaTime*rotSmoothSpeed);
    }
}
