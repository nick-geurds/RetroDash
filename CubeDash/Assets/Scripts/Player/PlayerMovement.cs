using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
   private Rigidbody2D rb;

    [Header("DashSettings")]
    public float dashVelocity = 6f;
    public float dashTimeElapsed = 1f;
    public ParticleSystem dashParticles;
    private bool canDash = true;
    private float dashTimer;
    public float dashInterval = 1.5f;

    private Vector3 startTouch;
    private Vector3 endTouch;

    private Vector3 dashStartPos;
    private Vector3 dashTargetPos;

    public bool isForPhone = false;

    private bool isDashing = false;

    

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        dashTimer += Time.deltaTime;

        if (dashTimer >= dashInterval)
        {
            canDash = true;
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
                    startTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    startTouch.z = 0;
                }

                if (Input.GetMouseButtonUp(0))
                {
                    endTouch = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    endTouch.z = 0;

                    Vector3 launchDirPC = (endTouch - startTouch).normalized;
                    dashStartPos = gameObject.transform.position;
                    dashTargetPos = transform.position + launchDirPC * dashVelocity;
                    dashParticles.Play();

                    StartCoroutine(Dash());

                    
                    dashTimer = 0;
                }
            }
        }
    }

    public IEnumerator Dash()
    {
        isDashing = true;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / dashTimeElapsed;
            gameObject.transform.position = Vector3.Lerp(dashStartPos, dashTargetPos, t);
            
            yield return null;
        }
        dashParticles.Stop();

        gameObject.transform.position = dashTargetPos;

        isDashing = false;
        canDash = false;
    }
}
