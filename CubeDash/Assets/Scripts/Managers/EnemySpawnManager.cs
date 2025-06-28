using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class Waves 
{
    public GameObject[] enemiesToSpawn;
    public float spawnInterval;
    public float numberOfEnemies;
    public float timeBetweenWaves;
}

public class EnemySpawnManager : MonoBehaviour
{
    public static List<GameObject> activeEnemies = new List<GameObject>();

    public Waves[] waves;

    private Waves waveIndex;
    private int currentWaveIndex;

    private GameObject player;

    private bool canSpawn = true;
    private bool waveGotUpped;

    private float spawnTimer;
    private float timeBetweenWaveTimer;
    private void Start()
    {
        player = GameObject.Find("Player");    
    }

    private void Update()
    {
        waveIndex = waves[currentWaveIndex];

        SpawnWave();
    }

    void SpawnWave()
    {
        spawnTimer += Time.deltaTime;
        
        if (canSpawn)
        {
            if (spawnTimer > waveIndex.spawnInterval)
            {
                GameObject randomEnemy = waveIndex.enemiesToSpawn[Random.Range(0, waves.Length)];

                float spawnFromPlayerX = player.transform.position.x + Random.Range(-10, 10);
                float spawnFromPlayerY = player.transform.position.y + Random.Range(-10, 10);

                Instantiate(randomEnemy, new Vector3(spawnFromPlayerX, spawnFromPlayerY, 0), Quaternion.identity);

                waveIndex.numberOfEnemies--;
                spawnTimer = 0;
            }

            if (waveIndex.numberOfEnemies == 0)
            {
                canSpawn = false;
            }

        }

        if (activeEnemies.Count == 0)
        {
            timeBetweenWaveTimer += Time.deltaTime;

            if (timeBetweenWaveTimer > waveIndex.timeBetweenWaves && !waveGotUpped)
            {
                currentWaveIndex++;
                timeBetweenWaveTimer = 0;
                waveGotUpped = true;
                canSpawn = true;

                if (waveGotUpped)
                {
                    waveGotUpped = false;
                }
            }
        }
    }

   


}
