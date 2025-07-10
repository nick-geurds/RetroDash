using UnityEngine;

public class PlayerDamageProjectileBurst : MonoBehaviour
{
    private PlayerStats stats;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();
    }

    public void TriggerProjectileBurst()
    {
        if (!stats.fireProjectilesOnDamage || stats.damageProjectilePrefab == null || stats.damageProjectileCount <= 0)
            return;

        float angleStep = 360f / stats.damageProjectileCount;
        float spawnRadius = 0.6f;

        for (int i = 0; i < stats.damageProjectileCount; i++)
        {
            // Begin bij 90° (recht omhoog), dan verdeeld over de cirkel
            float angle = (90f + i * angleStep) * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

            Vector2 spawnPos = (Vector2)transform.position + dir * spawnRadius;

            GameObject proj = Instantiate(stats.damageProjectilePrefab, spawnPos, Quaternion.identity);
            Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = dir * stats.damageProjectileSpeed;
            }

            proj.transform.rotation = Quaternion.LookRotation(Vector3.forward, dir);
        }
    }

}
