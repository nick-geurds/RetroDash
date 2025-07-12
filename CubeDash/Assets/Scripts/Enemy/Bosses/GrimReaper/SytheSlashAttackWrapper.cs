using System.Collections;
using UnityEngine;

public class SytheSlashAttackWrapper : BaseAttack
{
    public override AttackType attackType => AttackType.Special;

    public GameObject sytheSlashPrefab;
    public int slashCount = 3;
    public float delayBetweenSlashes = 0.3f;
    public float anticipationTime = 0.4f;
    public float yOffsetFromPlayer = -2f;

    public override void Initialize(Transform playerTransform)
    {
        base.Initialize(playerTransform);
    }

    public override IEnumerator ExecuteAttack()
    {
        // Herhaal de aanval een aantal keer
        for (int i = 0; i < repeatCount; i++)
        {
            // Zorg ervoor dat de aanval in de juiste positie wordt gespawned
            for (int j = 0; j < slashCount; j++)
            {
                Vector2 spawnPos = (Vector2)player.position + new Vector2(0, yOffsetFromPlayer);

                GameObject slash = Instantiate(sytheSlashPrefab, spawnPos, Quaternion.identity);

                // Verkrijg de SytheSlashAttack script van de prefab
                SytheSlashAttack sytheAttackScript = slash.GetComponent<SytheSlashAttack>();
                if (sytheAttackScript != null)
                {
                    sytheAttackScript.SetWillBeHittable(j == slashCount - 1);
                    sytheAttackScript.SetAnticipationTime(anticipationTime);
                }

                // Zet de Rigidbody2D als kinematic om ongewenste physics-effecten te voorkomen
                Rigidbody2D rb = slash.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.gravityScale = 0f;  // Zorg ervoor dat de sythe geen zwaartekracht heeft
                }

                yield return new WaitForSeconds(delayBetweenSlashes);
            }

            // Wacht een beetje extra tijd na de laatste slash
            yield return new WaitForSeconds(1f);
        }
    }
}
