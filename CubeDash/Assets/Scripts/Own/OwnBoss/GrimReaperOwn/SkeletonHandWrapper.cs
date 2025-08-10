using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonHandWrapper : MonoBehaviour
{
    public int numberToSpawn = 4;
    public GameObject objectToSpawn;

    public List<GameObject> spawnPoints = new List<GameObject>();

    private GameObject currentSpawnPoint;

    private DamageHandlerSimple damageHandler;
    private BossStateMachineSimple bossStateMachine;

    private void Start()    
    {
        StartCoroutine(spawn());
        bossStateMachine = FindFirstObjectByType<BossStateMachineSimple>();

        bossStateMachine.activeAttacks.Add(gameObject);
    }

    private IEnumerator spawn()
    {
        int randomChance = Random.Range(numberToSpawn - 1, 0);
        float rotation = 0;
        float newRotation = 90;
        for (int i = 0; i < numberToSpawn; i++)
        {
            currentSpawnPoint = spawnPoints[i];

            GameObject spawnedObject = Instantiate(objectToSpawn);
            spawnedObject.transform.position = currentSpawnPoint.transform.position;
            spawnedObject.transform.rotation = Quaternion.Euler(0, 0, rotation);

            damageHandler = spawnedObject.GetComponentInChildren<DamageHandlerSimple>();

            if (randomChance == i)
            {
                damageHandler.isHittable = true;
            }

            rotation += newRotation;
        }
        yield return new WaitForSeconds(3);

        bossStateMachine.activeAttacks.Remove(gameObject);
        Destroy(gameObject);
            


    }


}
