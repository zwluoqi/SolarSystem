// using System;
// using UnityEngine;
//
// [ExecuteAlways]
// [RequireComponent(typeof(Astronomical))]
// public class AstronomicalLineRender:MonoBehaviour
// {
//     public Astronomical _astronomical;
//     public LineRenderer _lineRenderer;
//     public Color color = Color.red;
//     public Vector3[] poss;
//     private void Start()
//     {
//         _astronomical = GetComponent<Astronomical>();
//         CreateLine();
//         
//     }
//
//     private void CreateLine()
//     {
//         poss = new Vector3[SolarSystemSimulater.Inst.simulaterNumber];
//         _lineRenderer = (new GameObject(_astronomical.name)).AddComponent<LineRenderer>();
//         _lineRenderer.widthMultiplier = 0.1f;
//         _lineRenderer.startColor = color;
//         _lineRenderer.endColor = color;
//         _lineRenderer.sharedMaterial = SolarSystemSimulater.Inst.line;
//         _lineRenderer.positionCount = poss.Length;
//         _lineRenderer.SetPositions(poss);
//     }
//
//     private void Update()
//     {
//         
//         if (_lineRenderer != null)
//         {
//             
//             _lineRenderer.startColor = color;
//             _lineRenderer.endColor = color;
//         }
//         else
//         {
//             CreateLine();
//             
//             _lineRenderer.startColor = color;
//             _lineRenderer.endColor = color;
//         }
//     }
//
//
//     public void SetPos(int i, Vector3 transformPosition)
//     {
//         poss[i] = transformPosition;
//     }
//
//     public void SetLinePositions()
//     {
//         _lineRenderer.SetPositions(poss);
//     }
//
//     public void ResetPosNum(int simulaterNumber)
//     {
//         if (simulaterNumber < 0)
//         {
//             return;
//         }
//         if (simulaterNumber != poss.Length)
//         {
//             poss = new Vector3[simulaterNumber];
//             _lineRenderer.positionCount = poss.Length;
//         }
//     }
// }