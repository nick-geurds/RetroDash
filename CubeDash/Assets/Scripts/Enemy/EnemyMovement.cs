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

        gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, player.transform.position, moveSpeed * Time.deltaTime);

        if (canDash)
        {
           

            dashTimer += Time.deltaTime;

            if (dashTimer > dashInterval)
            {
                canDashAgain = true;
                if (canDashAgain)
                {
                    startDashPos = gameObject.transform.position;

                    Vector3 direction = (player.transform.position - transform.position).normalized;
                    targetDashPos = transform.position + direction * dashVelocity;

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
            transform.position = Vector3.Lerp(startDashPos, targetDashPos, t);

            yield return null;
        }

        dashParticles.Stop();
        dashTimer = 0f;

        transform.position = targetDashPos;
    }
}
