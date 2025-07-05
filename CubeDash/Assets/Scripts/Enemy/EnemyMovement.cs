using System.Collections;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    private GameObject player;
    private SpriteRenderer spriteRenderer;
    private Color orgColor;

    [Header("MovementSettings")]
    public float moveSpeed = .3f;
    public bool canDash = false;
    private bool canDashAgain = false;

    [Header("Dash Settings")]
    public float dashInterval = 2f;
    public float dashVelocity = 3f;
    public float dashTimeElapsed = .2f;
    public float dashBuiltUp = .2f;
    private float dashTimer;

    public LayerMask obstacle;
    public ParticleSystem dashParticles;

    private Vector3 targetPos;
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
        // Bereken gewenste volgende positie richting speler
        Vector3 desiredNextPos = Vector3.MoveTowards(transform.position, player.transform.position, moveSpeed * Time.deltaTime);

        // Clamp de gewenste positie binnen de arena boundaries
        Vector3 clampedNextPos = ArenaBounds.Instance.ClampPosition(desiredNextPos);

        // Check of de clamped positie gelijk is aan de gewenste positie
        if (clampedNextPos == desiredNextPos)
        {
            // Beweging is binnen de arena, dus zet positie
            transform.position = clampedNextPos;
        }
        else
        {
            // Enemy zou buiten de arena bewegen, dus niet verplaatsen.
            // Je kunt hier eventueel extra logica toevoegen om langs muren te patrouilleren of andere acties te doen.
        }

        // DASH LOGICA (onveranderd), maar zorg dat dash target ook binnen arena blijft

        if (canDash)
        {
            dashTimer += Time.deltaTime;

            if (dashTimer > dashInterval)
            {
                if (canDashAgain)
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

                    // Clamp dash target binnen arena
                    targetDashPos = ArenaBounds.Instance.ClampPosition(targetDashPos);

                    dashParticles.Play();

                    StartCoroutine(EnemyDash());

                    dashTimer = 0;
                    canDashAgain = false;
                }
            }
        }
    }

    IEnumerator EnemyDash()
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
    }

}
