using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAim : MonoBehaviour
{
    [SerializeField] PlayerMovement1 player;
    Transform arrow;
    [SerializeField] LineRenderer line;
    Camera cam;
    [SerializeField] LayerMask groundLayer;
    [Header("MaterialBlinking")]
    [SerializeField] float timeBtwBlinks;
    [SerializeField] Color blinkColor;
    [SerializeField] int percentageToBlink = 75;
    Color baseColor;
    private Material mat;

    Vector3 mousePos;
    Vector3 originalArrowPos;
    private bool oneMatBlink = false;

    float distance;
    private void Start()
    {
        cam = Camera.main;
        arrow = transform.GetChild(0);
        originalArrowPos = arrow.localPosition;
        mat = line.material;
        baseColor = mat.color;
        player.onStayBulletTime += SetLineRendererColor;
    }
    void Update()
    {
        CalculateRotation();

        if (player.IsBulletTime)
        {
            distance = (transform.position - mousePos).magnitude;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, MyMaths.CalculateDirectionVectorNormalized(transform.position, mousePos), distance, groundLayer);
            if(hit.collider != null)
            {
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
        SetLineRenderer();
        transform.up = (mousePos - transform.position).normalized;
    }

    void SetLineRenderer()
    {
        line.SetPosition(0, transform.position);
        line.SetPosition(1, arrow.position);
    }
    public void SetLineRendererColor(float percent)
    {
        print(percent);
        if(oneMatBlink == false)
        {
            if(percent > percentageToBlink)
            {
                StartCoroutine(BlinkLineRendererCoroutine());
                oneMatBlink = true;
            }
        }
    }

    IEnumerator BlinkLineRendererCoroutine()
    {
        yield return new WaitForSeconds(timeBtwBlinks);
        mat.color = blinkColor;
        yield return new WaitForSeconds(timeBtwBlinks);
        mat.color = baseColor;
        yield return new WaitForSeconds(timeBtwBlinks);
        mat.color = blinkColor;
        yield return new WaitForSeconds(timeBtwBlinks);
        mat.color = baseColor;
        yield return new WaitForSeconds(timeBtwBlinks);
        mat.color = blinkColor;
        yield return new WaitForSeconds(timeBtwBlinks);
        mat.color = baseColor;
        oneMatBlink = false;

    }

    private void OnDrawGizmos()
    {
        if (player.IsBulletTime)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawLine(transform.position, mousePos);
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, MyMaths.CalculateDirectionVectorNormalized(transform.position, mousePos) * distance);
        }
    }
}
