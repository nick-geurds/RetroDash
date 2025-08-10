using System.Collections;
using UnityEngine;

public class KnockBack : MonoBehaviour
{
    public float knockBackForce = 4f;
    public float knockBackDuration = .2f;

    private BoxCollider2D b_collider;
    private Bounds mapBounds;
    private Vector2 minMapBounds;
    private Vector2 maxMapBounds;

    private PlayerMovementSimple playerMovSimple;

    private void Start()
    {
        b_collider = GameObject.Find("MapBounds").GetComponent<BoxCollider2D>();
        mapBounds = b_collider.bounds;
        minMapBounds = mapBounds.min;
        maxMapBounds = mapBounds.max;

        playerMovSimple = GetComponent<PlayerMovementSimple>();
    }
    public IEnumerator DoKnockBack(Vector2 direction)
    {
        if (playerMovSimple.isDashing)
            yield break;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + (Vector3)(direction * knockBackForce);

        float t = 0f;

        while (t < knockBackDuration)
        {
            t += Time.deltaTime;
            Vector3 nextpos = Vector3.Lerp(startPos, endPos, t / knockBackDuration);

            float clampedX = Mathf.Clamp(nextpos.x , minMapBounds.x + .5f, maxMapBounds.x - .5f);
            float clampedY = Mathf.Clamp(nextpos.y, minMapBounds.y + .5f, maxMapBounds.y - .5f);

            transform.position = new Vector3(clampedX, clampedY, 0);


            yield return null;
        }
    }
}
