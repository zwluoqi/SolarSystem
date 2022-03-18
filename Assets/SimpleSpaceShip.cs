using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SimpleSpaceShip:MonoBehaviour
{
    public Rigidbody _rigidbody;
    public float rotSpeed = 60;
    public float rotSmoothSpeed = 5;
    public float engineAcceleration = 20;
    public Vector3 initSpeed ;
    
    public Vector3 astronAcceleration;
    public Vector3 inputDir;
    public bool grounding;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
        _rigidbody.AddForce( initSpeed, ForceMode.VelocityChange);
    }

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

    private void OnCollisionEnter(Collision other)
    {
        grounding = true;
        Debug.LogWarning("OnCollisionEnter:"+other.gameObject.name);
    }
    
    private void OnCollisionExit(Collision other)
    {
        grounding = false; 
        Debug.LogWarning("OnCollisionExit:"+other.gameObject.name);
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        var astrons = GameObject.FindObjectsOfType<Astronomical>();
        astronAcceleration = Vector3.zero;
        foreach (var astronomical in astrons)
        {
            var sqrtDistance = Vector3.SqrMagnitude(astronomical._rigidbody.position - _rigidbody.position);
            var forceDir = (astronomical._rigidbody.position - _rigidbody.position).normalized;
            var acceleration = forceDir * GlobalDefine.G  * astronomical.Mass / sqrtDistance;
            astronAcceleration += acceleration;
        }
        //引力加速度
        _rigidbody.AddForce(astronAcceleration, ForceMode.Acceleration);
        
        
        var up = _rigidbody.rotation * Vector3.up;
        var right = _rigidbody.rotation * Vector3.right;
        var forward = _rigidbody.rotation * Vector3.forward;
        
        //飞船旋转
        if (!grounding)
        {
            var xRot = Quaternion.AngleAxis(xMouseRot, up);
            var yRot = Quaternion.AngleAxis(yMouseRot, right);
            var targetRot = xRot * yRot * _rigidbody.rotation;
            var smoothTargetRot =
                Quaternion.Slerp(_rigidbody.rotation, targetRot, GlobalDefine.deltaTime * rotSmoothSpeed);
            _rigidbody.MoveRotation(smoothTargetRot);
        }


        //飞船动力
        // if (!grounding)
        {
            up = _rigidbody.rotation * Vector3.up;
            right = _rigidbody.rotation * Vector3.right;
            forward = _rigidbody.rotation * Vector3.forward;
            var moveDir = forward * inputDir.z + right * inputDir.x + up * inputDir.y;
            moveDir = moveDir.normalized;
            _rigidbody.AddForce(moveDir*engineAcceleration, ForceMode.Acceleration);
        }

        var maxAccelerationAstronomical =
            SolarSystemSimulater.Inst.GetMaxAccelerationAstron(this._rigidbody.position, astrons);

        if (maxAccelerationAstronomical.Item1 != null)
        {
            if (maxAccelerationAstronomical.Item1 != SolarSystemSimulater.Inst.centerTrans)
            {
                ChangeInertialFrameOfReference(maxAccelerationAstronomical.Item1);
            }
            
            // if (maxAccelerationAstronomical.Item1 != SolarSystemSimulater.Inst.centerTrans)
            // {
            //     //如果加速度最大的天体不是惯性天体，如果距离大于2000m则切换
            //     var sqrtDistance =
            //         Vector3.SqrMagnitude(maxAccelerationAstronomical.Item1._rigidbody.position - _rigidbody.position);
            //     if (sqrtDistance > 2000 * 2000)
            //     {
            //         ChangeInertialFrameOfReference(maxAccelerationAstronomical.Item1);
            //     }
            // }
            // else
            // {
            //     var nearestAstronomical = SolarSystemSimulater.Inst.GetNearestDistanceAstron(this._rigidbody.position, astrons);
            //     //最近的天体不是惯性天体，如果距离少于500m则切换
            //     if (nearestAstronomical.Item1 != maxAccelerationAstronomical.Item1)
            //     {
            //         if (nearestAstronomical.Item2 < 500 * 500)
            //         {
            //             ChangeInertialFrameOfReference(maxAccelerationAstronomical.Item1);
            //         }
            //     }
            // }
            // var nearestAstronomicalDistance =
            //     Vector3.SqrMagnitude(maxAccelerationAstronomical.Item1._rigidbody.position - _rigidbody.position);
            // if (nearestAstronomicalDistance < 2000 * 2000)
            // {
            //     if (SolarSystemSimulater.Inst.centerTrans != nearestAstronomical)
            //     {
            //         ChangeInertialFrameOfReference(nearestAstronomical);
            //     }
            // }
            // else
            // {
            //     if (SolarSystemSimulater.Inst.centerTrans != SolarSystemSimulater.Inst.defaultTrans)
            //     {
            //         ChangeInertialFrameOfReference(SolarSystemSimulater.Inst.defaultTrans);
            //     }
            // }
        }
    }

    private void ChangeInertialFrameOfReference(Astronomical nearestAstronomical)
    {
        var BeforeVelocity = SolarSystemSimulater.Inst.centerTrans.GetCurrentVelocity(); 
        //被天体捕获,调整惯性参考系
        SolarSystemSimulater.Inst.centerTrans = nearestAstronomical;
        var AfterVelocity = SolarSystemSimulater.Inst.centerTrans.GetCurrentVelocity();
        //调整相对速度
        _rigidbody.AddForce((BeforeVelocity - AfterVelocity), ForceMode.VelocityChange);
        Debug.LogError("调整惯性参考系到:"+nearestAstronomical.name);
    }

    public float yMouseRot;
    public float xMouseRot;
    void HandleInput()
    {

        var z = GetInputAxis(KeyCode.S, KeyCode.W);
        var x = GetInputAxis(KeyCode.A, KeyCode.D);
        var y = GetInputAxis(KeyCode.E, KeyCode.Q);
        inputDir = new Vector3(x, y, z);

        if (Input.GetMouseButton(0))
        {
            xMouseRot = Input.GetAxis("Mouse X") * rotSpeed;
            yMouseRot = -Input.GetAxis("Mouse Y") * rotSpeed;
        }
        else
        {
            yMouseRot = 0;
            xMouseRot = 0;
        }
    }


}