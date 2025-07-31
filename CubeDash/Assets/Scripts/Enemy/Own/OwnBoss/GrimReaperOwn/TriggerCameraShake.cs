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
        //float minDistance = 15f;
        //float currentDistance = (transform.position - player.transform.position).magnitude;

        //while (currentDistance > minDistance)
        //{
            
        //    currentDistance = (transform.position - player.transform.position).magnitude;
        //    yield return null;
        //}
        

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
