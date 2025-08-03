using System.Collections;
using UnityEngine;

public class SimpleShockWave : MonoBehaviour
{
    public GameObject schockWave;

    public AnimationCurve curve;
    public float schockDuration;

    public float schockScale;
    private Vector3 endScale;

    public bool debugMode;

    private void Update()
    {
        if(!debugMode)
            return;

        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(SchokWave());
        }
    }

    public IEnumerator SchokWave()
    {
        GameObject shockWave = Instantiate(schockWave);

        float t = 0;

        endScale = shockWave.transform.localScale * schockScale;

        while (t < schockDuration)
        {
            float animationCurve = curve.Evaluate(t / schockDuration);

            shockWave.transform.localScale = Vector3.LerpUnclamped(Vector3.zero, endScale, animationCurve);
            t += Time.deltaTime;
            yield return null;
        }

        shockWave.transform.localScale = endScale;
        yield return null;
        Destroy(shockWave);
    }
}
