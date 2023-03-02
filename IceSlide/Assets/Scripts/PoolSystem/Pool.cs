using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    [SerializeField] GameObject poolPrefab;
    [SerializeField] int initialPoolSize;

    private Queue<GameObject> objectPool;

    private void Start()
    {
        objectPool = new Queue<GameObject>();
        for (int i = 0; i < initialPoolSize; i++)
        {
            AddObject();
        }
    }
    public GameObject Get()
    {
        if (objectPool.Count == 0)
        {
            AddObject();
        }

        return objectPool.Dequeue();
    }

    private void AddObject()
    {
        GameObject obj = Instantiate(poolPrefab);
        obj.SetActive(false);
        objectPool.Enqueue(obj);

        obj.GetComponent<IPooleable>().Pool = this;
        
    }

    public void ReturnToPool(GameObject objectToReturn)
    {
        objectToReturn.gameObject.SetActive(false);
        this.objectPool.Enqueue(objectToReturn);

    }
}
