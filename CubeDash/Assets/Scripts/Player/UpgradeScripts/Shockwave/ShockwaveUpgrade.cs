using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveUpgrade : MonoBehaviour
{
    public LayerMask enemyLayer;

    public GameObject shockwavePrefab; // optioneel, visuele feedback
    public float expandDuration = 0.3f;

    private bool isShocking = false;

    private PlayerStats stats;

    private void Awake()
    {
        stats = GetComponent<PlayerStats>();  
    }

    public void TriggerShockwave()
    {
        if (isShocking || !stats.enableShockWave) return;
        StartCoroutine(DoShockwave());
    }

    private IEnumerator DoShockwave()
    {
        isShocking = true;

        // Optioneel visueel effect
        if (shockwavePrefab != null)
        {
            GameObject wave = Instantiate(shockwavePrefab, transform.position, Quaternion.identity);
            ShockwaveVisual visual = wave.GetComponent<ShockwaveVisual>();
            if (visual != null)
            {
                visual.Initialize(stats.shockwaveRadius, expandDuration);
            }
        }

        // Alle vijanden binnen de shockwave radius detecteren
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, stats.shockwaveRadius, enemyLayer);

        Debug.Log($"Shockwave detecteert {hits.Length} vijanden.");

        HashSet<GameObject> stunnedEnemies = new HashSet<GameObject>();

        foreach (var col in hits)
        {
            GameObject enemyRoot = col.transform.root.gameObject;
            if (stunnedEnemies.Contains(enemyRoot)) continue;

            IStunnable stunnable = enemyRoot.GetComponentInChildren<IStunnable>();
            if (stunnable != null)
            {
                stunnable.Stun(stats.shockwaveStunDuration);
                stunnedEnemies.Add(enemyRoot);
                Debug.Log($"Stun toegepast op {enemyRoot.name}");
            }
        }

        // Wacht tot animatie klaar is (indien aanwezig)
        yield return new WaitForSeconds(expandDuration);

        isShocking = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 1, 0.3f);
        Gizmos.DrawWireSphere(transform.position, stats.shockwaveRadius);
    }
}
