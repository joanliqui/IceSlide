using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    Camera main;
    Camera playerCam;

    private void Start()
    {
        main = Camera.main;
        playerCam = main.GetComponentInChildren<Camera>();

        playerCam.orthographicSize = main.orthographicSize;
    }
}
