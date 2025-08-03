using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerMovementSimple : MonoBehaviour
{
    public ParticleSystem dashParticles;
    private PlayerStats playerStats;

    public  float maxDistane = 5;
    public float dashInterval = 1f;
    public float dashDuration = .2f;
    private bool canDash = true;

    private float timer;

    private Vector3 startPos;
    private Vector3 startPosMouse;
    private Vector3 endPos;

    private BoxCollider2D b_collider;
    private Bounds mapBounds;
    private Vector2 minMapBounds;
    private Vector2 maxMapBounds;

    [HideInInspector] public bool isDashing = false;

    private void Start()
    {
        b_collider = GameObject.Find("MapBounds").GetComponent<BoxCollider2D>();
        mapBounds = b_collider.bounds;
        minMapBounds = mapBounds.min;
        maxMapBounds = mapBounds.max;

        playerStats = GetComponent<PlayerStats>();
        InitializeStats();
    }

    private void InitializeStats()
    {
        
    }

    

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer > dashInterval)
        {
            canDash = true;
        }

        if (canDash)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPosMouse = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            }

            if (Input.GetMouseButtonUp(0))
            {
                endPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
                StartCoroutine(DoDash());
            }
        }
    }


    public IEnumerator DoDash()
    {
        if (!canDash)
            yield break;
        isDashing = true;

        dashParticles.Play();

        startPos = transform.position;

        Vector3 posBetweenTouch = endPos - startPosMouse;
        Vector3 launchDir = posBetweenTouch.normalized;

        Vector3 dashTargerPos = startPos + launchDir * maxDistane;

        float t = 0f;

        while (t < dashDuration)
        {
            canDash = false;
            t += Time.deltaTime;

            Vector3 nextpos = Vector3.Lerp(startPos, dashTargerPos, t);

            float clampX = Mathf.Clamp(nextpos.x, minMapBounds.x + .5f, maxMapBounds.x - .5f);
            float clampY = Mathf.Clamp(nextpos.y, minMapBounds.y + .5f, maxMapBounds.y - .5f);

            transform.position = new Vector3(clampX, clampY, 0);

            yield return null;
        }

        dashParticles.Stop();

        timer = 0f;
        isDashing = false;
    }
}
