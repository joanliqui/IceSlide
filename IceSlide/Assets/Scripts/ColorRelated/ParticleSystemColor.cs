using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemColor : MonoBehaviour, IPooleable
{
    ParticleSystem ps;
    ParticleSystem.MainModule main;
    private Pool myPool;

    List<ParticleSystem> otherPs = new List<ParticleSystem>();
    List<ParticleSystem.MainModule> otherMainModules = new List<ParticleSystem.MainModule>();
    public GameObject GameObject => throw new System.NotImplementedException();

    public Pool Pool
    {
        get
        {
            return myPool;
        }
        set
        {
            if (myPool == null) myPool = value;
            else
            {
                throw new System.Exception("El Objeto ya tiene una Pool asociada");
            }
        }
    }

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        main = ps.main;
        foreach (Transform item in transform)
        {
            if(item.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
            {
                otherPs.Add(ps);
                otherMainModules.Add(ps.main);
            }
        }
    }

    public void SetParticleSystemColor(Color color)
    {
        main.startColor = color;
        if(otherPs.Count > 0)
        {
            otherMainModules.ForEach(p => p.startColor = color);
        }
    }

    public void PlaceAndPlay(Vector3 pos)
    {
        transform.position = pos;
        ps.Play();

        if (otherPs.Count > 0)
        {
            otherPs.ForEach(p => p.Play());
        }


        StartCoroutine(ReturnToPool());
    }

    IEnumerator ReturnToPool()
    {
        yield return new WaitForSeconds(main.startLifetime.constant);
        myPool.ReturnToPool(this.gameObject);
    }
}
