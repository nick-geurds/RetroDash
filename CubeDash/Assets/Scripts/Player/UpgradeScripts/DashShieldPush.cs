using System.Collections;
using UnityEngine;

public class DashShieldPush : MonoBehaviour
{
    public float moveDistance = 1f;
    public float moveDuration = 0.15f;
    public LayerMask enemyLayer;
    public float knockbackForce = 10f;
    public float lifetime = 0.2f;

    private Vector3 moveDir;

    public void Initialize(Vector3 direction)
    {
        moveDir = -direction.normalized;

        // Start beweging
        StartCoroutine(MoveBackwards());
    }

    private IEnumerator MoveBackwards()
    {
        Vector3 start = transform.position;
        Vector3 target = start + moveDir * moveDistance;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;
            transform.position = Vector3.Lerp(start, target, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = target;

        // Collision handling
        Vector2 boxSize = new Vector2(1.5f, 0.5f); // Pas dit aan zoals je wilt
        Collider2D[] hits = Physics2D.OverlapBoxAll(transform.position, boxSize, 0f, enemyLayer);
        foreach (Collider2D enemy in hits)
        {
            Vector2 knockDir = (enemy.transform.position - transform.position).normalized;
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(knockDir * knockbackForce, ForceMode2D.Impulse);
            }
        }

        Destroy(gameObject, lifetime);
    }

   
}
