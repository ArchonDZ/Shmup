using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[System.Serializable]
public class Segment
{
    public Transform point0;
    public Transform point1;
    public Transform point2;
    public Transform point3;
}

#if UNITY_EDITOR
[CustomEditor(typeof(Bezier))]
public class BezierEditor : Editor
{
    Bezier bezier;

    void OnEnable()
    {
        bezier = FindObjectOfType<Bezier>();
    }

    void OnSceneGUI()
    {
        Draw(bezier?.segments);
    }

    void Draw(Segment[] segments)
    {
        for (int i = 0; i < segments.Length; i++)
        {
            Handles.DrawLine(segments[i].point0.position, segments[i].point1.position);
            Handles.DrawLine(segments[i].point2.position, segments[i].point3.position);
            Handles.DrawBezier(segments[i].point0.position, segments[i].point3.position, segments[i].point1.position, segments[i].point2.position, Color.red, null, 2);
        }
    }
}
#endif

public class Bezier : MonoBehaviour
{
    public Segment[] segments;

    public Vector3 GetFirstPoint() => GetPoint(segments[0].point0.position, segments[0].point1.position, segments[0].point2.position, segments[0].point3.position, 0);
    public Vector3 GetLastPoint()
    {
        int lastSegmentIndex = segments.Length - 1;
        Segment lastSegment = segments[lastSegmentIndex];

        return GetPoint(lastSegment.point0.position, lastSegment.point1.position, lastSegment.point2.position, lastSegment.point3.position, 1);
    }

    public Vector3 GetPath(float time)
    {
        time = Mathf.Clamp01(time);
        float generalTime = time / (1.0f / segments.Length);
        float segmentTime = generalTime % 1.0f;
        int index = (int)(generalTime - segmentTime);

        if (index >= segments.Length)
        {
            segmentTime = 1;
            index--;
        }

        Vector3 result = GetPoint(segments[index].point0.position, segments[index].point1.position, segments[index].point2.position, segments[index].point3.position, segmentTime);

        return result;
    }

    private Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float time)
    {
        time = Mathf.Clamp01(time);
        float oneMinusT = 1.0f - time;

        Vector3 result = Mathf.Pow(oneMinusT, 3) * p0 +
            3.0f * Mathf.Pow(oneMinusT, 2) * time * p1 +
            3.0f * oneMinusT * Mathf.Pow(time, 2) * p2 +
            Mathf.Pow(time, 3) * p3;

        return result;
    }
}
