using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public PlayerStatsScriptableObject stats;

    [HideInInspector] public Coroutine knockbackRoutine;

    private Rigidbody2D rb;
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

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        orgColor = playerSprite.color;
        orgScale = transform.localScale;

        cantKnockBack = false;
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
                //  TOUCH INPUT
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

                            RaycastHit2D hit = Physics2D.Raycast(transform.position, launchDir, stats.dashDis, obstacle);

                            dashTargetPos = hit.collider != null ?
                                hit.point :
                                transform.position + launchDir * stats.dashDis;

                            dashParticles.Play();

                            StartCoroutine(Dash());

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
                            Vector3 launchDirPC = swipe.normalized;
                            dashStartPos = gameObject.transform.position;

                            RaycastHit2D hit = Physics2D.Raycast(transform.position, launchDirPC, stats.dashDis, obstacle);


                            if (hit.collider != null)
                            {
                                dashTargetPos = hit.point;
                            }
                            else
                            {
                                dashTargetPos = transform.position + launchDirPC * stats.dashDis;
                            }

                            

                            dashParticles.Play();
                            

                            StartCoroutine(Dash());

                            dashTimer = 0;
                            canDash = false;
                            readyToDash = false;
                        }
                    }
                   
                }
            }
        }
    }

    public IEnumerator Dash()
    {
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

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / dashDuration;
            gameObject.transform.position = Vector3.Lerp(dashStartPos, dashTargetPos, t);

            yield return null;
        }
        dashParticles.Stop();

        gameObject.transform.position = dashTargetPos;

        isDashing = false;
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
        Vector3 target = start + (Vector3)(direction.normalized * distance);

        Debug.Log("Knockback started!");
        Debug.DrawLine(start, target, Color.red, 1f);
        float elapsed = 0f;

        StartCoroutine(iFrames());

        while (elapsed < duration)
        {
            rb.MovePosition(Vector3.Lerp(start, target, elapsed / duration));  // <- FIX
            elapsed += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(target);  // <- zorgt dat hij eindigt op juiste plek
        cantKnockBack = false;
    }

    IEnumerator iFrames()
    {
        canTakeDamage = false;
        Debug.Log("iFrames active");
        yield return new WaitForSeconds(.3f);
        Debug.Log("iFrames de-active");
        canTakeDamage = true;   
    }

}
