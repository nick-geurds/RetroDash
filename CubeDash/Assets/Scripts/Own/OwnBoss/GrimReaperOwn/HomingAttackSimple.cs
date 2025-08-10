using System.Collections;
using UnityEngine;

public class HomingAttackSimple : MonoBehaviour
{
    public GameObject objectToSpawn;
    [Space(20)]

    public int numberOfObjects;
    public float interval;
    public float spawnRadius;
    [Space(20)]

    public int repeatCount;

    private GameObject player;
    private DamageHandlerSimple dmgHandle;
    private BossStateMachineSimple stateMachine;

    private void Start()
    {
        
        player = GameObject.Find("Player");
        stateMachine = FindFirstObjectByType<BossStateMachineSimple>();
        stateMachine.activeAttacks.Add(gameObject);
        repeatCount = Random.Range(0, repeatCount);
        StartCoroutine(HomingAttack());

    }


    private IEnumerator HomingAttack()
    {
        int randomNumber = Random.Range(numberOfObjects - 1, 0);

        float arcAngle = (360f / 360f) * 2f * Mathf.PI;
        float nextAngle = arcAngle / numberOfObjects;
        float angle = Mathf.PI / 2;
        for (int n = 0; n < repeatCount; n++)
        {
            for (int i = 0; i < numberOfObjects; i++)
            {
                float x = Mathf.Cos(angle) * spawnRadius;
                float y = Mathf.Sin(angle) * spawnRadius;

                GameObject spawnedObject = Instantiate(objectToSpawn);
                spawnedObject.transform.position = new Vector2(transform.position.x + x, transform.position.y + y);
                spawnedObject.transform.rotation = Quaternion.identity;
                dmgHandle = spawnedObject.GetComponent<DamageHandlerSimple>();

                if (i == numberOfObjects - randomNumber)
                {
                    dmgHandle.isHittable = true;
                }
                else
                {
                    dmgHandle.isHittable = false;
                }

                angle += nextAngle;

                yield return new WaitForSeconds(interval);

            }

            yield return new WaitForSeconds(1f);
        }

            stateMachine.activeAttacks.Remove(gameObject);
            Destroy(gameObject);
    }
        
}
