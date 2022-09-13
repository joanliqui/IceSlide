using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof (PlayerMovement1))]

public class AngleViewEditor : Editor
{
    private void OnSceneGUI()
    {
        PlayerMovement1 player = (PlayerMovement1)target;
        Handles.color = Color.red;

        Vector3 angle1;
        Vector3 angle2;

       // Handles.DrawLine(player.transform.position, player.transform.position + viewAngleA * fov.ViewYellowRadius);
        //Handles.DrawLine(player.transform.position, player.transform.position + viewAngleB * fov.ViewYellowRadius);
    }
}
