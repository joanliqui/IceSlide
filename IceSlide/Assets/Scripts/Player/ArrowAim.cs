using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAim : MonoBehaviour
{
    [SerializeField] PlayerMovement1 player;
    Transform arrow;
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] LineRenderer line;
    Camera cam;
    [SerializeField] LayerMask groundLayer;

    Vector3 mousePos;
    Vector3 originalArrowPos;
    bool isBulletTime;
    private void Start()
    {
        cam = Camera.main;
        isBulletTime = false;
        arrow = transform.GetChild(0);
        originalArrowPos = arrow.localPosition;
    }
    void Update()
    {
        CalculateRotation();

        if (player.IsBulletTime)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, mousePos, 1000, groundLayer);
            if(hit.collider != null)
            {
                Debug.Log(hit.transform.name);
                arrow.position = hit.point;
            }
            else
            {
                arrow.position = mousePos;
            }

            if (line.enabled == false)
            {
                line.enabled = true;
            }

        }
        else //Habria que mirar para que solo se pusiera en el frame en el que sueltas el click en su posicion, no a cada frame
        {
            arrow.localPosition = originalArrowPos;
            if(line.enabled == true)
            {
                line.enabled = false;
            }
        }
    }

    void CalculateRotation()
    {
        mousePos = cam.ScreenToWorldPoint(player.MousePos);
        mousePos.z = 0;
        line.SetPosition(0, transform.position);
        line.SetPosition(1, arrow.position);
        transform.up = (mousePos - transform.position).normalized;
    }
}
