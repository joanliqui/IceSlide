using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Turret))]
public class TurretEditor : Editor
{
    private void OnSceneGUI()
    {
        Turret turret = (Turret)target;

        Handles.color = Color.red;
        Handles.DrawWireArc(turret.transform.position, Vector3.forward, Vector3.up, 360, turret.LowViewRange);
        Handles.DrawWireArc(turret.transform.position, Vector3.forward, Vector3.up, 360, turret.BigViewRange);

        Handles.color = Color.yellow;

        Vector3 angleA = MyMaths.DirectionFromAngle(turret.MaxRotationAngle, turret.transform);
        Vector3 angleB = MyMaths.DirectionFromAngle(-turret.MaxRotationAngle, turret.transform);

        Handles.DrawLine(turret.transform.position, turret.transform.position + angleA * turret.LowViewRange);
        Handles.DrawLine(turret.transform.position, turret.transform.position + angleB * turret.LowViewRange);
    }
}
