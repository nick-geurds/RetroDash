using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectPoolSimple : MonoBehaviour
{
    public GameObject objectToPool;
    public Queue<GameObject> pool = new Queue<GameObject>();

    public GameObject GetObject()
    {
        if (pool.Count > 0)
        {
            GameObject obj = pool.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        return Instantiate(objectToPool);
    }

    public void ReturnObjectToPool(GameObject obj)
    {
        obj.SetActive(false);
        pool.Enqueue(obj);
    }
}
