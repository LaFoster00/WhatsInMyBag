using System.Collections;
using System.Collections.Generic;
using Ludiq;
using Unity.Mathematics;
using UnityEngine;

namespace GuiUtility
{
    public static class GuiUtility
    {
        public static float3 GetScreenPointAboveObject(GameObject gameObject)
        {
            Collider collider = gameObject.GetComponent<Collider>();
            if (collider)
            {
                return GetScreenPointAboveObject(collider);
            }

            return float3.zero;
        }
        
        public static float3 GetScreenPointAboveObject(Collider collider)
        {
            Camera mainCamera = Camera.main;
            if (!mainCamera) return float3.zero;
            
            float3[] points = BoundsToPoints(collider.bounds);
            float3[] screenPoints = new float3[points.Length];
            for (int i = 0; i < screenPoints.Length; i++)
            {
                screenPoints[i] = mainCamera.WorldToScreenPoint(points[i]);
            }

            float3 highestPoint = screenPoints[0];
            for (int i = 0; i < screenPoints.Length; i++)
            {
                highestPoint = highestPoint.y < screenPoints[i].y ? screenPoints[i] : highestPoint;
            }

            return new float3(mainCamera.WorldToScreenPoint(collider.bounds.center).x, highestPoint.yz);
        }

        public static float3[] BoundsToPoints(Bounds bounds)
        {
            float3[] points = new float3[4];
            float3 center = bounds.center;
            float x = bounds.extents.x;
            float y = bounds.extents.y;
            float z = bounds.extents.z;
            points[0] = center + new float3( x,  y,  z);
            points[1] = center + new float3( x,  y, -z);
            points[2] = center + new float3(-x,  y,  z);
            points[3] = center + new float3(-x,  y, -z);
            return points;
        }
    }
}