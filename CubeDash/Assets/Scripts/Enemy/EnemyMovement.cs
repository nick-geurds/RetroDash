using System.Collections;
using UnityEngine;

public class EnemyMovement : MonoBehaviour, IStunnable
{
    private GameObject player;
    private SpriteRenderer spriteRenderer;
    private Color orgColor;

    [Header("Movement Settings")]
    public float moveSpeed = 0.3f;
    public bool canDash = false;
    private bool canDashAgain = false;

    [Header("Dash Settings")]
    public float dashInterval = 2f;
    public float dashVelocity = 3f;
    public float dashTimeElapsed = 0.2f;
    public float dashBuiltUp = 0.2f;
    private float dashTimer;
    private Coroutine dashRoutine;

    [Header("Stun Settings")]
    private bool isStunned = false;
    private float stunTimer = 0f;

    public LayerMask obstacle;
    public ParticleSystem dashParticles;

    private Vector3 startDashPos;
    private Vector3 targetDashPos;

    private void Start()
    {
        player = GameObject.Find("Player");
        spriteRenderer = GetComponent<SpriteRenderer>();
        orgColor = spriteRenderer.color;
    }

    private void LateUpdate()
    {
        if (isStunned)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                isStunned = false;
            }
            return; // Geen beweging of dash tijdens stun
        }

        HandleMovement();
        HandleDash();
    }

    private void HandleMovement()
    {
        Vector3 desiredNextPos = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);
        Vector3 clampedNextPos = ArenaBounds.Instance.ClampPosition(desiredNextPos);

        if (clampedNextPos == desiredNextPos)
        {
            transform.position = clampedNextPos;
        }
    }

    private void HandleDash()
    {
        if (!canDash) return;

        dashTimer += Time.deltaTime;

        if (dashTimer > dashInterval && canDashAgain)
        {
            startDashPos = transform.position;

            Vector3 direction = (player.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, dashVelocity, obstacle);

            if (hit.collider != null)
            {
                targetDashPos = hit.point;
            }
            else
            {
                targetDashPos = transform.position + direction * dashVelocity;
            }

            targetDashPos = ArenaBounds.Instance.ClampPosition(targetDashPos);

            dashParticles.Play();
            dashRoutine = StartCoroutine(EnemyDash());

            dashTimer = 0f;
            canDashAgain = false;
        }
    }

    private IEnumerator EnemyDash()
    {
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(dashBuiltUp / 3);
        spriteRenderer.color = orgColor;
        yield return new WaitForSeconds(dashBuiltUp / 3);
        spriteRenderer.color = Color.white;
        yield return new WaitForSeconds(dashBuiltUp / 3);
        spriteRenderer.color = orgColor;

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / dashTimeElapsed;
            Vector3 lerpPos = Vector3.Lerp(startDashPos, targetDashPos, t);
            transform.position = ArenaBounds.Instance.ClampPosition(lerpPos);
            yield return null;
        }

        dashParticles.Stop();
        transform.position = ArenaBounds.Instance.ClampPosition(targetDashPos);
        dashTimer = 0f;

        dashRoutine = null;
    }

    public void Stun(float duration)
    {
        if (isStunned) return; // Geen dubbele stun nodig

        isStunned = true;
        stunTimer = duration;
        Debug.Log($"{gameObject.name} is gestunned voor {duration} seconden.");
    }
}
