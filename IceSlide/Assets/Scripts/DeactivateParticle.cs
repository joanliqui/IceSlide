using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateParticle : MonoBehaviour
{
    private void Awake()
    {
        gameObject.SetActive(false);
    }
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
