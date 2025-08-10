using System.Collections;
using TreeEditor;
using UnityEngine;

public class EnemyDash : EnemyMovementSimple
{
    [Header("DashSettings")]
    public float dashMult;
    public float dashDuration;
    public float dashInterval;

    private float timer;
    private bool canDash = true;

    private Vector3 startDashPos;
    private Vector3 targetDashPos;
    
    

    protected override void Start()
    {
        base.Start();
        timer = 0;
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();
        handleDash();
    }

    private void handleDash()
    {
        if (canDash)
            timer += Time.deltaTime;

        if (timer >= dashInterval && canDash)
        {
            Vector3 direction = (player.transform.position - transform.position ).normalized;
            Debug.DrawLine(transform.position, transform.position + direction * 2f, Color.red, 1f);

            startDashPos = transform.position;
            targetDashPos = transform.position + direction * dashMult;

            StartCoroutine(Dash());

            timer = 0;
            canDash = false;
        }
    }
    private IEnumerator Dash()
    {
            isDashing = true;
        
        int i = 0;

        for (i = 0; i <= 3; i++)
        {
            sprite.color = Color.white;
            yield return new WaitForSeconds(.03f);

            sprite.color = orgColor;
            yield return new WaitForSeconds(.03f);
        }

        float t = 0f;

        while (t < 1f)
        {

            t += Time.deltaTime / dashDuration;

            Vector3 lerpPos = Vector3.Lerp(startDashPos, targetDashPos, t);
            transform.position = lerpPos;
            yield return null;
        }

        isDashing = false;

        timer = 0;

        canDash = true;
    }
}
