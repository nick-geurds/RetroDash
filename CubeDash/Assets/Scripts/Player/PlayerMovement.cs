using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerStats stats;

    [HideInInspector] public Coroutine knockbackRoutine;

    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer playerSprite;
    public bool isForPhone = false;

    [Header("DashSettings")]
    public ParticleSystem dashParticles;
    public LayerMask obstacle;
    private bool canDash = true;
    private float dashTimer;
    public float knockBackForce = 3f;
    [HideInInspector] public bool cantKnockBack;

    [Header("anim Settings")]
    public float scaleAmount = 1.4f;
    public float scaleDur = .15f;

    public float touchThershold = .5f;

    private Vector3 startTouch;
    private Vector3 endTouch;

    private Vector3 dashStartPos;
    private Vector3 dashTargetPos;

    private Vector3 orgScale;
    private Color orgColor;

    [HideInInspector] public bool isDashing = false;
    private bool readyToDash = false;
    private bool didAnimUp = true;

    [HideInInspector] public bool canTakeDamage = true;

    [SerializeField] private float shortDashMultiplier = 0.5f; // Toegevoegd
    [SerializeField] private float shortDashThreshold = 1.0f;  // Swipe kleiner dan dit = korte dash

    private float lastDashTime;
    public float dashActiveGracePeriod = 0.1f;

    public bool IsRecentlyDashing()
    {
        return isDashing || Time.time - lastDashTime < 0.1f;
    }

    private PlayerDamageProjectileBurst projectileBurst;
    private ShockwaveUpgrade shockwaveUpgrade;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        stats = GetComponent<PlayerStats>();

        projectileBurst = GetComponent<PlayerDamageProjectileBurst>();
        shockwaveUpgrade = GetComponent<ShockwaveUpgrade>();

        orgColor = playerSprite.color;
        orgScale = transform.localScale;

        col = GetComponent<Collider2D>();

        cantKnockBack = false;
    }

    private void OnEnable()
    {
        PlayerStats.OnPlayerStatsInitialized += OnPlayerStatsInitialized;
    }

    private void OnDisable()
    {
        PlayerStats.OnPlayerStatsInitialized -= OnPlayerStatsInitialized;
    }

    private void OnPlayerStatsInitialized()
    {
        dashTimer = 0f;
        canDash = true;
        knockBackForce = 3f;
        Debug.Log("[PlayerMovement] Player stats initialized. Ready to use dashInterval: " + stats.dashInterval + ", dashSpeed: " + stats.dashSpeed);
    }

    private void Update()
    {
        dashTimer += Time.deltaTime;

        if (dashTimer >= stats.dashInterval)
        {
            canDash = true;

            if (!didAnimUp)
            {
                LeanTween.scale(gameObject, orgScale * scaleAmount, scaleDur).setEasePunch();
                StartCoroutine(changeSpritToWhite());
                didAnimUp = true;
            }
        }

        if (!isDashing && canDash)
        {
            if (isForPhone)
            {
                if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    Vector3 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
                    touchPos.z = 0;

                    if (touch.phase == TouchPhase.Began)
                    {
                        readyToDash = true;
                        startTouch = touchPos;
                    }
                    else if (touch.phase == TouchPhase.Ended && readyToDash)
                    {
                        endTouch = touchPos;
                        Vector3 swipe = endTouch - startTouch;

                        if (swipe.magnitude > touchThershold)
                        {
                            Vector3 launchDir = swipe.normalized;
                            dashStartPos = transform.position;

                            //  Bepaal of korte dash
                            bool isShortDash = swipe.magnitude < shortDashThreshold;
                            float dashDist = isShortDash ? stats.dashDis * shortDashMultiplier : stats.dashDis;

                            RaycastHit2D hit = Physics2D.Raycast(transform.position, launchDir, dashDist, obstacle);
                            dashTargetPos = hit.collider != null ? hit.point : transform.position + launchDir * dashDist;

                            dashParticles.Play();
                            StartCoroutine(Dash(isShortDash)); //  Gebruik aangepaste dash

                            dashTimer = 0;
                            canDash = false;
                            readyToDash = false;
                        }
                    }
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (canDash)
                    {
                        readyToDash = true;
                        Vector3 mousePos = Input.mousePosition;
                        mousePos.z = 10;
                        startTouch = Camera.main.ScreenToWorldPoint(mousePos);
                        startTouch.z = 0;
                    }
                }

                if (Input.GetMouseButtonUp(0) && readyToDash)
                {
                    if (canDash)
                    {
                        Vector3 mousePos = Input.mousePosition;
                        mousePos.z = 10;
                        endTouch = Camera.main.ScreenToWorldPoint(mousePos);
                        endTouch.z = 0;

                        Vector3 swipe = endTouch - startTouch;

                        if (swipe.magnitude > touchThershold)
                        {
                            Vector3 launchDir = swipe.normalized;
                            dashStartPos = transform.position;

                            //  Bepaal of korte dash
                            bool isShortDash = swipe.magnitude < shortDashThreshold;
                            float dashDist = isShortDash ? stats.dashDis * shortDashMultiplier : stats.dashDis;

                            RaycastHit2D hit = Physics2D.Raycast(transform.position, launchDir, dashDist, obstacle);
                            dashTargetPos = hit.collider != null ? hit.point : transform.position + launchDir * dashDist;

                            dashParticles.Play();
                            StartCoroutine(Dash(isShortDash)); // Gebruik aangepaste dash

                            dashTimer = 0;
                            canDash = false;
                            readyToDash = false;
                        }
                    }
                }
            }
        }
    }

    //  Dash met parameter voor korte of lange dash
    public IEnumerator Dash(bool isShort = false)
    {
        col.enabled = true;

        if (knockbackRoutine != null)
        {
            StopCoroutine(knockbackRoutine);
            knockbackRoutine = null;
            cantKnockBack = false;
        }

        isDashing = true;
        canDash = false;

        float dashDistance = Vector3.Distance(dashStartPos, dashTargetPos);
        float dashDuration = dashDistance / stats.dashSpeed;

        if (isShort)
        {
            dashDuration *= shortDashMultiplier;
        }

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / dashDuration;
            Vector3 lerpPos = Vector3.Lerp(dashStartPos, dashTargetPos, t);
            transform.position = ArenaBounds.Instance.ClampPosition(lerpPos);
            yield return null;
        }

        dashParticles.Stop();
        transform.position = ArenaBounds.Instance.ClampPosition(dashTargetPos);

        isDashing = false;
        lastDashTime = Time.time;
        didAnimUp = false;
    }

    IEnumerator changeSpritToWhite()
    {
        playerSprite.color = Color.white;
        yield return new WaitForSeconds(scaleDur);
        playerSprite.color = orgColor;
    }

    public IEnumerator Knockback(Vector2 direction, float distance, float duration)
    {
        if (isDashing) yield break;

        cantKnockBack = true;
        Vector3 start = transform.position;

        col.enabled = false;

        Vector3 target = GetMaxKnockbackTarget(start, direction, distance);

        float elapsed = 0f;
        StartCoroutine(iFrames());

        while (elapsed < duration)
        {
            Vector3 nextPos = Vector3.Lerp(start, target, elapsed / duration);
            rb.MovePosition(nextPos);
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (projectileBurst != null)
        {
            projectileBurst.TriggerProjectileBurst();
        }

        if (shockwaveUpgrade != null)
        {
            shockwaveUpgrade.TriggerShockwave();
        }

        col.enabled = true;
        rb.MovePosition(target);
        cantKnockBack = false;
    }

    Vector3 GetMaxKnockbackTarget(Vector3 startPos, Vector2 direction, float maxDistance)
    {
        Vector3 potentialTarget = startPos + (Vector3)(direction.normalized * maxDistance);
        Vector3 clampedTarget = ArenaBounds.Instance.ClampPosition(potentialTarget);
        float actualDistance = Vector3.Distance(startPos, clampedTarget);
        return clampedTarget;
    }

    IEnumerator iFrames()
    {
        canTakeDamage = false;
        yield return new WaitForSeconds(.3f);
        canTakeDamage = true;
    }
}
