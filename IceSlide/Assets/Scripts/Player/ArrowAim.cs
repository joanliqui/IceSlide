using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAim : MonoBehaviour
{
    [SerializeField] PlayerMovement1 player;
    [SerializeField] float rotationSpeed = 1f;
     Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }
    void Update()
    {
        CalculateRotation();
    }

    void CalculateRotation()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(player.MousePos);
        mousePos.z = 0;
        transform.up = (mousePos - transform.position).normalized;
    }
}
