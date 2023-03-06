using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace USCSL
{
    public static class Extensions
    {
        #region Float Extensions

        public static float Map(this float s, float min, float max, float newMin, float newMax)
        {
            return newMin + (s - min) * (newMax - newMin) / (max - min);
        }

        #endregion

        #region Vector2 Extensions

        public static Vector2 Rotate(this Vector2 v, float degrees)
        {
            degrees *= -1;
            float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
            float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

            float tx = v.x;
            float ty = v.y;
            v.x = (cos * tx) - (sin * ty);
            v.y = (sin * tx) + (cos * ty);
            return v;
        }

        public static float Angle(this Vector2 v, Vector2 to = default)
        {
            return Vector2.SignedAngle(v, to);
        }

        public static Vector2Int FloorToVector2Int(this Vector2 v)
        {
            Vector2Int flooredVector = new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
            return flooredVector;
        }

        #endregion

        #region float3 Extensions

        public static float3 MultiplyComponents(this float3 v1, float3 v2)
        {
            return new float3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
        }

        public static float GetXZDistance(float3 v1, float3 v2)
        {
            return math.distance(v1.MultiplyComponents(new float3(1, 0, 1)), v2.MultiplyComponents(new float3(1, 0, 1)));
        }

        public static float GetXZMagnitude(this float3 v1)
        {
            return math.length(v1.MultiplyComponents(new float3(1, 0, 1)));
        }

        public static float3 GetRandomPosition(float range)
        {
            return new float3(Random.Range(-range, range), 0, Random.Range(-range, range));
        }

        public static float3 GetRandomPosition(float2 range)
        {
            return new float3(Random.Range(-range.x, range.x), 0, Random.Range(-range.y, range.y));
        }

        #endregion

        #region Mesh Extensions

        public static Mesh DeepCopy(this Mesh m)
        {
            Mesh newMesh = new Mesh();
            newMesh.vertices = m.vertices;
            newMesh.triangles = m.triangles;
            newMesh.uv = m.uv;
            newMesh.normals = m.normals;
            newMesh.colors = m.colors;
            newMesh.tangents = m.tangents;
            return newMesh;
        }

        #endregion

        #region CharacterController Extensions

        public static bool FindGround(this CharacterController c, out RaycastHit hitResult, float maxDistance = Single.MaxValue)
        {
            return Physics.CapsuleCast(new Vector3(0, c.radius, 0), new Vector3(0, c.height - c.radius, 0), c.radius,
                Vector3.down, out hitResult, maxDistance);
        }

        #endregion

        #region Array Extensions

        public static int GetRandomIndex<T>(this T[] a)
        {
            return Random.Range(0, a.Length);
        }

        public static T GetRandomValue<T>(this T[] a)
        {
            return a[Random.Range(0, a.Length)];
        }

        #endregion

        #region List Extensions

        public static int GetRandomIndex<T>(this List<T> a)
        {
            return Random.Range(0, a.Count);
        }

        public static T GetRandomValue<T>(this List<T> a)
        {
            return a[Random.Range(0, a.Count)];
        }

        #endregion

        #region Layer

        public static void ChangeLayerRecursive(this GameObject gameObject, int layer)
        {
            gameObject.layer = layer;
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                ChangeLayerRecursive(gameObject.transform.GetChild(i).gameObject, layer);
            }
        }

        #endregion
    }
}
