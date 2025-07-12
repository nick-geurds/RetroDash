using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public GameObject chainPrefab;  // De keten prefab die we willen poolen
    private Queue<GameObject> pool = new Queue<GameObject>();

    public int poolSize = 10;  // Het aantal objecten in de pool

    private void Start()
    {
        // Vul de pool met ketens
        for (int i = 0; i < poolSize; i++)
        {
            GameObject chain = Instantiate(chainPrefab);
            chain.SetActive(false);  // Zet het object inactief
            pool.Enqueue(chain);  // Voeg het object toe aan de pool
        }
    }

    // Verkrijg een keten uit de pool
    public GameObject GetPooledChain()
    {
        if (pool.Count > 0)
        {
            GameObject chain = pool.Dequeue();
            chain.SetActive(true);  // Zet het object actief
            return chain;
        }
        else
        {
            // Maak een nieuwe keten als er geen meer beschikbaar zijn in de pool
            GameObject chain = Instantiate(chainPrefab);
            return chain;
        }
    }

    // Zet een keten terug in de pool
    public void ReturnChainToPool(GameObject chain)
    {
        chain.SetActive(false);  // Zet het object inactief
        pool.Enqueue(chain);  // Voeg het object terug toe aan de pool
    }
}
