using System.Collections;
using UnityEngine;

public class ShockwaveVisual : MonoBehaviour
{
    private float finalScale = 1f;
    private float expandDuration = 0.3f;

    public void Initialize(float radius, float duration)
    {
        finalScale = radius * 2f; // *2 want scale is diameter, radius = halve diameter
        expandDuration = duration;
        StartCoroutine(ExpandAndFade());
    }

    IEnumerator ExpandAndFade()
    {
        float timer = 0f;
        Vector3 startScale = Vector3.zero;
        Vector3 endScale = Vector3.one * finalScale;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color orgColor = sr.color;

        while (timer < expandDuration)
        {
            timer += Time.deltaTime;
            float t = timer / expandDuration;

            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            sr.color = Color.Lerp(orgColor, new Color(orgColor.r, orgColor.g, orgColor.b, 0f), t);

            yield return null;
        }

        Destroy(gameObject);
    }
}
