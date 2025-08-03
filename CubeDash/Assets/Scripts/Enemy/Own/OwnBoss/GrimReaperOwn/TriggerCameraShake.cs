using JetBrains.Annotations;
using System.Collections;

using UnityEngine;
using UnityEngine.Rendering;

public class TriggerCameraShake : MonoBehaviour
{
    private SimpleCamareShake simpleCamShake;
    public float duration;
    private float mag;

    private GameObject player;

    private void Start()
    {
        player = GameObject.Find("Player");
        simpleCamShake = FindFirstObjectByType<SimpleCamareShake>();

        StartCoroutine(CalculateMag());
        
    }

    private IEnumerator CalculateMag()
    {
        StartCoroutine(simpleCamShake.ImpactShake(duration, 0.3f));

        float t = 0f;
        while (t < duration)
        {
            mag = 1/(transform.position - player.transform.position).magnitude;
            simpleCamShake.magnitude = mag;

            simpleCamShake.ApplyIntesity(mag * 2);

            t += Time.deltaTime;
            yield return null;
        }

       
    }

}
