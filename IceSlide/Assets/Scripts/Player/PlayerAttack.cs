using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
   // [SerializeField] float attackLengh = 1;
    //[SerializeField] float attackWidth = 0.5f;
    private Vector2 attackArea;
    [SerializeField] float radius = 1;
    [SerializeField] Transform attackPos;
    [SerializeField] Transform ejeAttack;
    [SerializeField] Color gizmoColor;
    InputManager input;

    [SerializeField] GameObject particulaAttack;

    private Camera cam;

    private void Start()
    {
        //attackArea = new Vector2(attackLengh, attackWidth);
        cam = Camera.main;
        input = GetComponent<InputManager>();
        particulaAttack.SetActive(false);
    }
    public void Attack()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(attackPos.position, radius);
        if(cols.Length > 0)
        {
            for (int i = 0; i < cols.Length; i++)
            {
                if(cols[i].TryGetComponent<IDamagable>(out IDamagable enemy))
                {
                    enemy.Damaged();
                }
            }
        }
    
        
    }

    private void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(input.MousePose);
        mousePos.z = 0;
        ejeAttack.up = (mousePos - ejeAttack.position).normalized;
    }

    private void OnDrawGizmos()
    {
        
        //Gizmos.color = gizmoColor;
        //Gizmos.DrawSphere(attackPos.position, radius);
    }

}
