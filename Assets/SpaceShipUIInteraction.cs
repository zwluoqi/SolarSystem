using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpaceShipUIInteraction : MonoBehaviour
{

    public TMP_Text info;

    public Image horitalSpeed;

    public Image verticalSpeed;

    public Astronomical selectAstron;
    public SimpleSpaceShip spaceShip;

    private void Start()
    {
        TestCollider2D.onGlobalClick += delegate(GameObject go)
        {
            selectAstron = go.GetComponent<Astronomical>();
        };
    }

    // Update is called once per frame
    void Update()
    {
        if (selectAstron != null)
        {
            info.gameObject.SetActive(true);
            var distacen = (selectAstron._rigidbody.position - spaceShip._rigidbody.position).magnitude - selectAstron.Radius-spaceShip.transform.localScale.x*0.5;
            var selectAstronSpeed = SolarSystemSimulater.Inst.GetRelativeSpeed(selectAstron);
            //天体相对飞船的速度
            var crossSpeed = selectAstronSpeed - spaceShip._rigidbody.velocity;
            StringBuilder stringBuilder = new StringBuilder();
            if (distacen > 1000)
            {
                stringBuilder.AppendLine("距离:" + (int) (distacen / 1000) + "KM");
            }
            else
            {
                stringBuilder.AppendLine("距离:" + (int) (distacen) + "M");
            }
            if (crossSpeed.magnitude > 1000)
            {
                stringBuilder.AppendLine("速度:" + (int) (crossSpeed.magnitude / 1000) + "KM/S");
            }
            else
            {
                stringBuilder.AppendLine("速度:" + (int) (crossSpeed.magnitude) + "M/S");
            }

            var spaceShipUp = spaceShip._rigidbody.rotation * Vector3.up;
            var spaceShipRight = spaceShip._rigidbody.rotation * Vector3.right;
            var crossSpeedUp = Vector3.Project(crossSpeed, spaceShipUp);
            var up = Vector3.Angle(crossSpeed, spaceShipUp) < 90;
            var crossSpeedRight = Vector3.Project(crossSpeed, spaceShipRight);
            var right = Vector3.Angle(crossSpeed, spaceShipRight) < 90;
            horitalSpeed.rectTransform.sizeDelta = new Vector2(crossSpeedRight.magnitude*10, 2);
            horitalSpeed.transform.localScale = new Vector3(right ? 1 : -1, 1, 1);
            verticalSpeed.rectTransform.sizeDelta = new Vector2(2, crossSpeedUp.magnitude*10);
            verticalSpeed.transform.localScale = new Vector3(1, up?-1:1, 1);
            info.text = stringBuilder.ToString();
        }
        else
        {
            info.gameObject.SetActive(false);
        }
    }
}
