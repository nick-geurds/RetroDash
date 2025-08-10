using System.Collections;
using UnityEngine;

public class SimpleChainHolder : MonoBehaviour
{
    private BossStateMachineSimple stateMachineSimple;
    private SimpleCamareShake cameraShake;

    public GameObject objectToSpawn;
    private Transform parent;

    public int numberOfChains;

    public float timeBeforeSpin;
    public float timeAfterSpin;
    public float spinDuration;

    public float spawnRadius = 1f;

    private float timer = 0f;
    private void Start()
    {
        parent = gameObject.transform;
        stateMachineSimple = FindFirstObjectByType<BossStateMachineSimple>();
        if (stateMachineSimple != null )
            stateMachineSimple.activeAttacks.Add(gameObject);

        cameraShake = FindFirstObjectByType<SimpleCamareShake>();

        StartCoroutine(DoSpin());
    }

    public IEnumerator DoSpin()
    {
        int randomNumber = Random.Range(numberOfChains - 1, 0);

        float arcAngle = (360f / 360f) * 2f * Mathf.PI;
        float nextAngle = arcAngle / numberOfChains;
        float angle = Mathf.PI / 2;

        for (int i = 0; i < numberOfChains; i++)
        {
            float x = Mathf.Cos(angle) * spawnRadius;
            float y = Mathf.Sin(angle) * spawnRadius;

            GameObject spawnedObject = Instantiate(objectToSpawn, parent);

            spawnedObject.transform.position = new Vector2(x,y);

            Vector2 directionToCenter = (transform.position - spawnedObject.transform.position).normalized; //AI
            float angleToCenter = Mathf.Atan2(directionToCenter.y, directionToCenter.x) * Mathf.Rad2Deg; //AI

            spawnedObject.transform.rotation = Quaternion.Euler(0,0, angleToCenter + 90);

            angle += nextAngle;
        }


        yield return new WaitForSeconds(timeBeforeSpin);

        cameraShake.magnitude = .5f;
        StartCoroutine(cameraShake.ImpactShake(.5f , .5f));

        LeanTween.cancel(gameObject);
        LeanTween.rotate(gameObject, new Vector3(0, 0, 360 / numberOfChains / 2), spinDuration).setEaseInElastic();

        yield return new WaitForSeconds(timeAfterSpin);

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.zero, 1.5f).setEaseInOutExpo().setOnComplete(() =>
        {
            stateMachineSimple.activeAttacks.Remove(gameObject);
            Destroy(gameObject);
        });
    }
}
