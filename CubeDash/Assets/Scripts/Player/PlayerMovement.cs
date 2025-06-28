using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private SpriteRenderer playerSprite;
    public bool isForPhone = false;

    [Header("DashSettings")]
    public float dashVelocity = 6f;
    public ParticleSystem dashParticles;
    private bool canDash = true;
    private float dashTimer;

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

    private PlayerStats playerStats;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = GetComponent<SpriteRenderer>();
        orgColor = playerSprite.color;
        playerStats = GetComponent<PlayerStats>();
        orgScale = transform.localScale;
    }

    private void Update()
    {
        dashTimer += Time.deltaTime;

        if (dashTimer >= playerStats.dashInterval)
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
                //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
                //{
                //    startTouch = Input.GetTouch(0).position;
                //    startTouch.z = 0;
                //}

                //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
                //{
                //    endTouch = Input.GetTouch(0).position;
                //    endTouch.z = 0;

                //    Vector3 launchDir = endTouch - startTouch;

                //    rb.linearVelocity = Vector3.ClampMagnitude(launchDir * dashVelocity, dashVelocity);
                //}
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
                            dashTargetPos = transform.position + launchDirPC * dashVelocity;
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
        isDashing = true;
        canDash = false;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / playerStats.dashTimeElapsed;
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
    
}
