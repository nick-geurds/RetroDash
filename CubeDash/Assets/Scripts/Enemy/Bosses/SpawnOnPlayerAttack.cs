using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnPlayerAttack : BaseAttack
{
    public string nameOfAttack;

    public override AttackType attackType => AttackType.Special;

    [Header("Spawn Config")]
    public GameObject objectToSpawn;
    public float yOffset = 2f;
    public bool lastOnlyHittable = true;
    public bool useRandomSpawnPoints = false; // <-- toggle voor random spawns

    [Header("Optional Spawn Points")]
    public List<Transform> spawnPoints;  // <-- stel deze in als je random wilt spawnen

    public override IEnumerator ExecuteAttack()
    {
        if (useRandomSpawnPoints && spawnPoints != null && spawnPoints.Count > 0)
        {
            yield return StartCoroutine(ExecuteAttackAtRandomSpawnPoints());
        }
        else
        {
            yield return StartCoroutine(ExecuteAttackAbovePlayer());
        }
    }

    private IEnumerator ExecuteAttackAbovePlayer()
    {
        for (int i = 0; i < repeatCount; i++)
        {
            Vector2 spawnPos = new Vector2(player.position.x, player.position.y + yOffset);
            GameObject obj = Instantiate(objectToSpawn, spawnPos, Quaternion.identity);

            SetHittableIfPossible(obj, i);

            yield return new WaitForSeconds(delayBetweenAttacks);
        }
    }

    private IEnumerator ExecuteAttackAtRandomSpawnPoints()
    {
        for (int i = 0; i < repeatCount; i++)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            GameObject obj = Instantiate(objectToSpawn, randomPoint.position, Quaternion.identity);

            SetHittableIfPossible(obj, i);

            yield return new WaitForSeconds(delayBetweenAttacks);
        }
    }

    private void SetHittableIfPossible(GameObject obj, int i)
    {
        var hittableScript = obj.GetComponent<ISpawnHittable>();
        if (hittableScript != null)
        {
            bool isLast = (i == repeatCount - 1);
            hittableScript.SetWillBeHittable(!lastOnlyHittable || isLast);
        }
    }
}
