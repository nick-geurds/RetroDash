using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnAttackBase : BaseAttack
{
    public bool spawnAtPlayer = true;
    public Vector2 spawnOffset = Vector2.zero;

    public bool useRandomSpawnPoints = false;
    public List<Transform> spawnPoints;

    protected Vector2 GetSpawnPosition()
    {
        if (useRandomSpawnPoints && spawnPoints != null && spawnPoints.Count > 0)
        {
            return spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }
        else if (spawnAtPlayer)
        {
            return (Vector2)player.position + spawnOffset;
        }
        else
        {
            return spawnOffset; // Of een andere vaste positie
        }
    }
}
