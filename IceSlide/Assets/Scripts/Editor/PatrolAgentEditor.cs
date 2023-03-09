using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PatrolAgent)), CanEditMultipleObjects]
public class PatrolAgentEditor : Editor
{
    private Vector3[] points;

    private void OnSceneGUI()
    {
        PatrolAgent pa = target as PatrolAgent;

        if (pa.PatrolPath == null || pa.PatrolPath.Length <= 0)
            return;

        points = new Vector3[pa.PatrolPath.Length];

        for (int i = 0; i < pa.PatrolPath.Length; i++)
        {
            Handles.color = Color.blue;
            if(i == 0)
            {
                Handles.color = Color.yellow;
            }
            Handles.SphereHandleCap(i, pa.PatrolPath[i].position, Quaternion.identity, 0.4f, EventType.Repaint);
            pa.PatrolPath[i].position = Handles.PositionHandle(pa.PatrolPath[i].position, Quaternion.identity);

            points[i] = pa.PatrolPath[i].position;
        }

        Handles.color = Color.white;
        Handles.DrawPolyLine(points);
    }
}
