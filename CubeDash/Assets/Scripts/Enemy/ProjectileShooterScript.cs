using NUnit.Framework;
using System;
using UnityEditor.Search;
using UnityEngine;

public class ProjectileShooterScript : MonoBehaviour
{
    public GameObject[] possibleSpawnPoints;
    public GameObject traiangleProj;
    public float shootInterval;
    private float intervalTimer;
    public bool normalShot = true;


    private void Update()
    {
        intervalTimer += Time.deltaTime;

        if (normalShot)
        {
            if (intervalTimer > shootInterval)
            {

                for (int i = 0; i < possibleSpawnPoints.Length; i++)
                {
                    Instantiate(traiangleProj, possibleSpawnPoints[i].transform.position, possibleSpawnPoints[i].transform.rotation);
                }
                intervalTimer = 0;
            }
        }
    }




}
