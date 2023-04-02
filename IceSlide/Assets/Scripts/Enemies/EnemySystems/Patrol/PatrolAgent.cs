using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAgent : MonoBehaviour
{
    [SerializeField] Transform[] patrolPath;
    private int destPoint = -1;

    public Transform[] PatrolPath { get => patrolPath;}

    private void Awake()
    {
        patrolPath[0].parent.name = "PatrolPath: " + transform.name;
        patrolPath[0].parent.parent = null;
    }

    public Vector3 GetNextPoint()
    {
        if (patrolPath.Length == 0) 
        {
            Debug.LogError("No hay patrolPoints en el PatrolAgent de " + transform.name);
            return Vector3.zero;
        } 

        destPoint++;
        if(destPoint > patrolPath.Length - 1)
        {
            destPoint = 0;
        }
        return patrolPath[destPoint].position;
    }
}
