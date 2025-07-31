using System.Collections;
using UnityEngine;


public class SimpleCamareShake : MonoBehaviour
{
    [HideInInspector] public float magnitude;

    private Vector3 orgPos;

    private void Start()
    {
        orgPos = transform.localPosition;
    }
    public IEnumerator ImpactShake(float duration, float speed)
    {
        Vector3 orgpos = transform.localPosition;

        float t = 0f;

        while(t < duration)
        {
            float x = (Random.Range(-2, 2) * magnitude  * speed);
            float y = (Random.Range(-1, 1) * magnitude  * speed);

            transform.localPosition = orgpos + new Vector3(x, y, 0);

            t += Time.deltaTime;

            yield return null;
        }   

        transform.localPosition = orgpos;

    }

    public void ApplyIntesity(float newMagnitude)
    {
        magnitude = newMagnitude;
    }
}
