using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private Queue<GameObject> poolArray;
    [SerializeField] GameObject poolPrefab;
    [SerializeField] int initialObjects;
    private void Start()
    {
        poolArray = new Queue<GameObject>();
        AddShots(initialObjects);
    }
    public GameObject Get()
    {
        if (poolArray.Count == 0)
        {
            AddShots(1);
        }

        return poolArray.Dequeue();
    }

    private void AddShots(int n)
    {
        for (int i = 0; i < n; i++)
        {
            GameObject obj = Instantiate(poolPrefab);
            obj.gameObject.SetActive(false);
            poolArray.Enqueue(obj);

            obj.GetComponent<IPooleable>().Pool = this;
        }
    }

    public void ReturnToPool(GameObject objectPool)
    {
        objectPool.gameObject.SetActive(false);
        poolArray.Enqueue(objectPool);

    }
}
