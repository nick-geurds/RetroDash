using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject prefab;
    public int initialSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public GameObject Get()
    {
        if (pool.Count == 0)
        {
            GameObject newObj = Instantiate(prefab);
            newObj.SetActive(false);
            pool.Enqueue(newObj);
        }

        GameObject objToUse = pool.Dequeue();
        objToUse.SetActive(true);
        return objToUse;
    }

    public void Return(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }

    public List<GameObject> GetAvailableObjects(int count)
    {
        List<GameObject> result = new List<GameObject>();

        foreach (var obj in pool) // of hoe je pool heet
        {
            if (!obj.activeInHierarchy)
                result.Add(obj);

            if (result.Count >= count)
                break;
        }

        return result;
    }
}
